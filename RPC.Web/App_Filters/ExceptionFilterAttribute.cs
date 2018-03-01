using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RPC.Web.Tools;

namespace Recolte.Web.App_Filters
{
    [AttributeUsageAttribute(AttributeTargets.Class | AttributeTargets.Method,
   Inherited = true, AllowMultiple = true)]
    public class ExceptionFilterAttribute : HandleErrorAttribute
    {
        private static ILog txtex_logger = LogManager.GetLogger("txtloggerExLogs");
        private static ILog dbex_logger = LogManager.GetLogger("dbloggerExLogs");
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
                return;
            var httpException = new HttpException(null, filterContext.Exception);
            if (httpException != null && httpException.GetHttpCode().Equals(500))
            {
                filterContext.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;  //System.Net.HttpStatusCode.InternalServerError;
                string exid = Guid.NewGuid().ToString();
                //捕获内部错误
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new JsonResult()
                    {
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        Data = new  { Status = 0, Code = "-1", Data = exid }
                    };
                }
                else
                {
                    filterContext.Result = new ViewResult() { ViewName = "~/Views/Shared/Error.cshtml", TempData = new TempDataDictionary() { { "exid", exid } } };
                }
                //记录日志
                string queryUrl = filterContext.HttpContext.Request.Url.PathAndQuery.ToLower();
                filterContext.HttpContext.Request.InputStream.Position = 0;
                var formdata = new System.IO.StreamReader(filterContext.HttpContext.Request.InputStream).ReadToEnd();
                //string formdata = JsonModelHelper.ObjectJson(filterContext.HttpContext.Request.Form.AllKeys.Select(v => new { Key = v, Value = filterContext.HttpContext.Request.Form[v] }));
                string controller = filterContext.RouteData.Values["controller"].ToString().ToLower();
                string action = filterContext.RouteData.Values["action"].ToString().ToLower();
                txtex_logger.Error(string.Format("请求地址{0}异常\r\n异常ID：{1}\r\nPOST参数：{2}\r\n异常信息：{3}", queryUrl, exid, formdata, filterContext.Exception));
                string requestip = PubMethod.GetRequestIP(filterContext.HttpContext.Request);
                dbex_logger.Info(new Exlogs() { ServIp = ConfigManager.LocalIp, Controller = controller, Action = action, QueryUrl = queryUrl, FromData = formdata, ExceptionId = exid, ExceptionMsg = filterContext.Exception.Message.Length > 500 ? filterContext.Exception.Message.Substring(0, 500) : filterContext.Exception.Message, UserId = filterContext.HttpContext.User.Identity.Name, RequestIp = requestip });
            }
            filterContext.ExceptionHandled = true;
        }
    }
}