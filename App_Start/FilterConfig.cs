using System.Web;
using System.Web.Mvc;

namespace HoNgocHai_2122110473_ASP.NET
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
