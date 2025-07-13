using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkUnitFrameWork.LowLayer
{
    static class Constants
    {
        public static DateTime EndDateTokens = new DateTime(2026, 03, 01);
        public static string SDNINFO = @"/api/v4/true-api/cdn/info";
        public static string SDNHEALTH = @"/api/v4/true-api/cdn/health/check";
        public static string CodeCheck = @"/api/v4/true-api/codes/check";
    }
}
