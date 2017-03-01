using System.Web.Mvc;

namespace Reinforced.Typings.Samples.Difficult.CodeGenerators
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
