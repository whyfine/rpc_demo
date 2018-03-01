using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Xml;

namespace RPC.Web.Tools
{
    public static class ConfigManager
    {
        static ConfigManager()
        {
            LocalIp = ConfigurationManager.AppSettings["LocalIp"] ?? "127.0.0.1";

            string filePath = HttpContext.Current.Server.MapPath("~/App_Configs/LogisticsCompanyCode.xml");
            LogisticsApiUrl = XmlHelper.GetXmlNodeByXpath(filePath, "//logistics_apiurl").InnerText;
        }

        public readonly static string LocalIp;


        public readonly static string LogisticsApiUrl;
    }
}