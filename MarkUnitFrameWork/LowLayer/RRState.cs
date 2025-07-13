using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarkUnitFrameWork.LowLayer.Models;

namespace MarkUnitFrameWork.LowLayer
{
    internal class RRState
    {
        CRPTConnector _cRPTConnector;
        IDictionary<string, object> settings;
        IDictionary<string, string> Heders;
        public RRState(IDictionary<string, object> Settings)
        {
            settings = Settings;
            initHeders();
            _cRPTConnector = new CRPTConnector(Heders, settings["Host"] as string);
        }

        public ResponseCodes TestCodesOnline(RequestCodes requestCodes)
        {
            var response = _cRPTConnector.SenderPost<ResponseCodes, RequestCodes>(requestCodes, Constants.CodeCheck);
            return response;
        }

        void initHeders()
        {
            Heders = new Dictionary<string, string>();
            Heders.Add("X-API-KEY", settings["X-API-KEY"] as string);
        }



    }

    internal class RequestCodes
    {
        public string[] codes;
    }


    public class ResponseCodes
    {
        public int code { get; set; }
        public string description { get; set; }
        public ResponseCode[] codes { get; set; }
        public string reqId { get; set; }
        public long reqTimestamp { get; set; }
    }

    public class ResponseCode
    {
        public string cis { get; set; }
        public bool valid { get; set; }
        public string printView { get; set; }
        public string gtin { get; set; }
        public int[] groupIds { get; set; }
        public bool verified { get; set; }
        public bool found { get; set; }
        public bool realizable { get; set; }
        public bool utilised { get; set; }
        public bool isBlocked { get; set; }
        public DateTime expireDate { get; set; }
        public DateTime productionDate { get; set; }
        public int errorCode { get; set; }
        public bool isTracking { get; set; }
        public bool sold { get; set; }
        public string packageType { get; set; }
        public string producerInn { get; set; }
        public bool grayZone { get; set; }
        public int soldUnitCount { get; set; }
        public int innerUnitCount { get; set; }
    }

}
