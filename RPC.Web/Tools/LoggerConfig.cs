using log4net.Core;
using log4net.Layout.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Layout;
using log4net;

namespace RPC.Web.Tools
{
    internal sealed class CustomPatternLayout : PatternLayout
    {
        public CustomPatternLayout()
        {
            this.AddConverter("property", typeof(CustomPatternConverter));
        }
    }

    internal sealed class CustomPatternConverter : PatternLayoutConverter
    {
        protected override void Convert(System.IO.TextWriter writer, LoggingEvent loggingEvent)
        {
            if (this.Option != null)
            {

                if (loggingEvent.MessageObject is Exlogs)
                {
                    var val = typeof(Exlogs).GetProperty(this.Option).GetValue(loggingEvent.MessageObject);
                    WriteObject(writer, loggingEvent.Repository, val);
                    return;
                }
                else if (loggingEvent.MessageObject is Ptlogs)
                {
                    var val = typeof(Ptlogs).GetProperty(this.Option).GetValue(loggingEvent.MessageObject);
                    WriteObject(writer, loggingEvent.Repository, val);
                    return;
                }

            }
            WriteObject(writer, loggingEvent.Repository, loggingEvent.GetProperties());
        }
    }

    public class Exlogs
    {
        public string ServIp { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string QueryUrl { get; set; }
        public string FromData { get; set; }
        public string ExceptionId { get; set; }
        public string ExceptionMsg { get; set; }
        public string UserId { get; set; }
        public string RequestIp { get; set; }
        public DateTime ExceptionTime { get; set; }
    }

    public class Ptlogs
    {
        public string ServIp { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string QueryUrl { get; set; }
        public string FromData { get; set; }
        public string UserId { get; set; }
        public string RequestIp { get; set; }
        public long RunMilliseconds { get; set; }
        public DateTime MonitorTime { get; set; }
    }
}
