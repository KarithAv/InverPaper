using System.Web;
using System.Web.Mvc;
using InverPaper.Utilidades;

namespace InverPaper
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
