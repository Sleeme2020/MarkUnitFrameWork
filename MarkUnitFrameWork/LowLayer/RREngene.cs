using MarkUnitFrameWork.LowLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkUnitFrameWork.LowLayer
{

    public interface IMarkUnitEntity
    {
        void UpdSDN();
        ResponseRR GetSdnOnline(SenderRR senderRRQuery);
        ResponseRR GetSdnOfline(SenderRR senderRRQuery);
        List<string> GetSDN();
        DateTime GetSDNDate();
    }

    public class ResponseRR
    {
        public string CodeRequest { get;  set; }
        public string Description { get;  set; }
        public string ReqId { get;  set; }
        public List<CodesRequest> codes { get; set; }
        public bool AllValid()
        {
            return codes != null && codes.All(c => c.Valid && c.Verified && c.Realizable && !c.CodeExpire);
        }

    }
    public class CodesRequest
    {
        public string Code { get; set; }
        public string PrintCode { get; set; }
        public bool Valid { get;  set; }
        public bool Verified { get; set; }
        public bool Realizable { get; set; }
        public bool Utilised { get; set; }
        public DateTime ExpireDate { get; set; }
        public bool CodeExpire {
            get => (ExpireDate > DateTime.Now) || ExpireDate == DateTime.MinValue;
        }
    }


    public class SenderRR
    {
        string[] _ArrayCodes;
        public SenderRR(string[] arrayCodes)
        {
            if (arrayCodes == null || arrayCodes.Length == 0)
                throw new ArgumentException("Array of codes cannot be null or empty", nameof(arrayCodes));
            _ArrayCodes = arrayCodes;
        }
        public int CountCodes => _ArrayCodes.Length;
        public string[] Codes=> _ArrayCodes;

    }

    

    public interface IMarkUnitSettingsEntity
    {
        void Initial(Dictionary<string, object> valuePairs);
        void Set(string key, object value);
        object Get(string key);
        bool Exist(string key);
        bool Initialed();
        Dictionary<string, object> GetValues();
        Dictionary<string, object> GetValuesDefault();
    }

    internal class RREngene: IMarkUnitEntity
    {
        private readonly IMarkUnitSettingsEntity _settingsEntity;
        private  SDNDataHosts _sdnDataHosts;
        private readonly SdnState _SDNState;        
        public RREngene(IMarkUnitSettingsEntity settingsEntity)
        {
            if (settingsEntity == null)
                throw new ArgumentNullException(nameof(settingsEntity), "Settings entity cannot be null");
            if (!settingsEntity.Initialed())
                throw new InvalidOperationException("Settings entity is not initialized");
            _settingsEntity = settingsEntity;
            _SDNState = new SdnState(settingsEntity.GetValues());
            UpdSDN();
        }

        public ResponseRR GetSdnOnline(SenderRR senderRRQuery)
        {
            ResponseCodes result = null;
            
            foreach(var sdn in _SDNState.GetSDN().hosts)
            {
                var setting = GenerateSettingOnline(sdn.Host);
                RRState rRState = new RRState(setting);
                var res = rRState.TestCodesOnline(new RequestCodes() { codes = senderRRQuery.Codes });
                if (res != null) { 
                    result = res;
                    break;
                }
            }
            if (result == null)
                throw new InvalidOperationException("Операция запроса на онлайн сервисы завершена не успешно, выполните офлайн проверку!");
            return new ResponseRR()
            {
                CodeRequest = result.code.ToString(),
                Description = result.description,
                ReqId = result.reqId,
                codes = result.codes.Select(x=>new CodesRequest()
                                            { 
                                              Code = x.cis.ToString(),
                                              ExpireDate = x.expireDate,
                                              Realizable = x.realizable,
                                              PrintCode = x.printView,
                                              Utilised = x.utilised,
                                              Valid = x.valid,
                                              Verified = x.verified,
                                            }
                                            ).ToList(),

            };
        }



        IDictionary<string, object> GenerateSettingOnline(string Host)
        {
            IDictionary<string, object> Setting  = new Dictionary<string, object>();
            foreach (var set in _settingsEntity.GetValues()) 
            { 
                if(set.Key=="Host")
                    Setting[set.Key] = Host;
                else
                    Setting[set.Key] = set.Value;
            }
            return Setting;
        }

        public ResponseRR GetSdnOfline(SenderRR senderRRQuery)
        {
            throw new NotImplementedException();
        }

        public void UpdSDN()
        {
            _sdnDataHosts = _SDNState.GetSDN();
        }

        public List<string> GetSDN()
        {
            return _SDNState.GetSDN().hosts.Select(x => x.Host.ToString()).ToList();
        }

        public DateTime GetSDNDate()
        {
            return _SDNState.LastUpdSdnDate();
        }
    }
}
