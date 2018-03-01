using System.Web;
using System.Web.Mvc;
using Recolte.Web.App_Filters;

namespace RPC.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ExceptionFilterAttribute());
        }
    }
}
