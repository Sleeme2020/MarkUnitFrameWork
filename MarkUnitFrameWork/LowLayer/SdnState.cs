using MarkUnitFrameWork.LowLayer.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace MarkUnitFrameWork.LowLayer
{
    

    internal class SDNDataHosts
    {
        public DateTime LastUpdateSDN { get; private set; }
        public DateTime LastUpdateSDNList { get; private set; }
        public List<SDNHost> hosts { get; }
        public SdnHostsRequest hostsRequest
        {
            set
            {
                parseHosts(value);
            }
        }

        public SDNDataHosts()
        {    
           hosts = new List<SDNHost>();            
        }
        
        
        public class SDNHost
        {
            public string Host { get; set; }
            public int latency { get; set; }
            public int ping { get; set; }
            public bool isBlocked { get; set; }
            public void Upd(HealthCheckRequest request)
            {
                latency = request.avgTimeMs;
                ping = 0;
                isBlocked = request.code==0?false:true;
                
            }
        }

        public void UpdateLastUpdateSDN()
        {
            LastUpdateSDN = DateTime.Now;
        }

        void UpdateLastUpdateSDNList()
        {
            LastUpdateSDNList = DateTime.Now;
        }
        
        void parseHosts(SdnHostsRequest request)
        {
            foreach (var host in request.hosts)
            {
                hosts.Add(new SDNHost()
                {
                    Host = host.host,
                    latency = 0,
                    ping = 0,
                    isBlocked = false
                });
            }
            UpdateLastUpdateSDNList();
        }

    }

    class SdnState
    {
        IDictionary<string, object> settings;        
        SDNDataHosts sdnDataHosts;
        public SdnState(IDictionary<string, object> Settings)
        {
            settings = Settings;
            validate();                       
            sdnDataHosts= new SDNDataHosts();
        }

        public SDNDataHosts GetSDN()
        {
            UpdSdn();
            return sdnDataHosts;
        }

        void UpdSdn()
        { 
            if(( DateTime.Now - sdnDataHosts.LastUpdateSDNList) > TimeSpan.FromHours(6) )
            {
                var headers = new Dictionary<string, string>();
                ConfigureHeaders(headers);
                var connector = new CRPTConnector(headers, settings["Host"] as string);
                sdnDataHosts.hostsRequest = connector.SenderGet<SdnHostsRequest>(Constants.SDNINFO);
                                
            }

            if(( DateTime.Now - sdnDataHosts.LastUpdateSDN) > TimeSpan.FromMinutes(30))
            {
                var headers = new Dictionary<string, string>();
                ConfigureHeaders(headers);
                var connector = new CRPTConnector(headers, settings["Host"] as string);
                var lst = sdnDataHosts.hosts;
                foreach (var host in lst)
                {

                    var conn = new CRPTConnector(headers, host.Host);
                    var req = conn.SenderGet<HealthCheckRequest>(Constants.SDNHEALTH);
                    host.ping = PerformPing(host.Host);
                    host.Upd(req);
                    sdnDataHosts.UpdateLastUpdateSDN();
                }
                
            }

        }

        private int PerformPing(string host)
        {
            try
            {
                using (var ping = new Ping())
                {
                    var reply = ping.Send(host);
                    if (reply.Status == IPStatus.Success)
                    {
                        return (int)reply.RoundtripTime; // Время отклика в миллисекундах
                    }
                }
            }
            catch
            {
                // Если пинг не удался, возвращаем -1
                return -1;
            }

            return -1; // Если статус не успешен
        }

        void validate()
        {
            if (!settings.ContainsKey("Host"))
                throw new Exception("Host not found in settings");
            if (!settings.ContainsKey("X-API-KEY"))
                throw new Exception("X-API-KEY not found in settings");
        }

        void ConfigureHeaders(IDictionary<string,string> headers)
        {
            //headers.Add("Content-Type", "application/json");
            if(Constants.EndDateTokens > DateTime.Now)
                headers.Add("X-API-KEY", settings["X-API-KEY"] as string);
            else
                throw new Exception("Latest time static Token, please upd module to dynamic tokens");
        }
    }
}
