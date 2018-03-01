using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RPC.Web.Models;
using RPC.Web.Tools;

namespace RPC.Web.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        public ActionResult Index()
        {
            return View();
        }

        BaiduLogistics GetInformation(string code, string no)
        {
            return PubMethod.BaiduWebClientGet<BaiduLogistics>(ConfigManager.LogisticsApiUrl + "?&appid=4001&nu=" + no);
        }
    }
}