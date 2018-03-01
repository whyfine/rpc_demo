using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPC.Web.Models
{

    public class BaiduLogistics
    {

        public string msg { get; set; }
        public int status { get; set; }
        public string error_code { get; set; }
        public BaiduLogisticsData data { get; set; }
    }

    public class BaiduLogisticsData
    {
        public BaiduLogisticsDataInfo info { get; set; }
    }

    public class BaiduLogisticsDataInfo
    {
        public int status { get; set; }
        public string com { get; set; }
        public int state { get; set; }
        public List<BaiduLogisticsDataInfoContext> context { get; set; }
    }

    public class BaiduLogisticsDataInfoContext
    {
        public string time { get; set; }
        public string desc { get; set; }
    }
}
