using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RPC.Web.Tools;
using Newtonsoft.Json;
using log4net;

namespace Recolte.Web.App_Filters
{
    [AttributeUsageAttribute(AttributeTargets.Class | AttributeTargets.Method,
    Inherited = true, AllowMultiple = true)]
    public class TraceActionFilterAttribute : ActionFilterAttribute
    {
        private static ILog dbpt_logger = LogManager.GetLogger("dbloggerPtLogs");
        public TraceActionFilterAttribute(int traceMilliseconds = 0)
        {
            this.traceMilliseconds = traceMilliseconds;
        }
        private int traceMilliseconds;
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            this.StopwatchEnabledStart(filterContext);
        }
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            this.StopwatchEnabledEnd(filterContext);
        }

        private void StopwatchEnabledStart(ActionExecutingContext filterContext)
        {
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            filterContext.RouteData.Values.Add("Stopwatch", watch);
            watch.Start();
        }

        private void StopwatchEnabledEnd(ActionExecutedContext filterContext)
        {
            object value;
            if (filterContext.RouteData.Values.TryGetValue("Stopwatch", out value))
            {
                System.Diagnostics.Stopwatch watch = value as System.Diagnostics.Stopwatch;
                if (watch != null && watch.IsRunning)
                {
                    watch.Stop();
                    if (watch.ElapsedMilliseconds >= this.traceMilliseconds)
                    {
                        filterContext.HttpContext.Request.InputStream.Position = 0;
                        var formdata = new System.IO.StreamReader(filterContext.HttpContext.Request.InputStream).ReadToEnd();
                        //string formdata = JsonModelHelper.ObjectJson(filterContext.HttpContext.Request.Form.AllKeys.Select(v => new { Key = v, Value = filterContext.HttpContext.Request.Form[v] }));
                        string controller = filterContext.RouteData.Values["controller"].ToString().ToLower();
                        string action = filterContext.RouteData.Values["action"].ToString().ToLower();
                        string requestip = PubMethod.GetRequestIP(filterContext.HttpContext.Request);
                        dbpt_logger.Info(new Ptlogs() { ServIp = ConfigManager.LocalIp, Controller = controller, Action = action, QueryUrl = filterContext.HttpContext.Request.Url.PathAndQuery.ToLower(), FromData = JsonConvert.SerializeObject(formdata), UserId = filterContext.HttpContext.User.Identity.Name, RequestIp = requestip, RunMilliseconds = watch.ElapsedMilliseconds });
                    }
                }
            }
        }

    }
}