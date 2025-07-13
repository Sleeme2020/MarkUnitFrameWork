using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkUnitFrameWork.LowLayer.Models
{
    internal class SdnHostRequest
    {
        public string host { get; set; }


    }


    internal class SdnHostsRequest
    {
        public int code { get; set; }
        public string description { get; set; }
        public List<SdnHostRequest> hosts { get; set; }
    }



    public class HealthCheckRequest
    {
        public int code { get; set; }
        public string description { get; set; }
        public int avgTimeMs { get; set; }
    }


}
