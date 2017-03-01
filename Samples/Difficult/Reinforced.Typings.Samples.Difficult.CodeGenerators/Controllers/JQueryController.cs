using System;
using System.Web.Mvc;
using Reinforced.Typings.Samples.Difficult.CodeGenerators.Models;
using Reinforced.Typings.Samples.Difficult.CodeGenerators.ReinforcedTypings.jQuery;

namespace Reinforced.Typings.Samples.Difficult.CodeGenerators.Controllers
{
    /// <summary>
    /// Our sample controller for testing queries made through jQuery
    /// </summary>
    public class JQueryController : Controller
    {
        [JQueryMethod(typeof(int))]
        public ActionResult SimpleIntMethod()
        {
            return Json(new Random().Next(100));
        }

        [JQueryMethod(typeof(string))]
        public ActionResult MethodWithParameters(int num, string s, bool boolValue)
        {
            return Json(string.Format("{0}-{1}-{2}", num, s, boolValue));
        }

        [JQueryMethod(typeof(SampleResponseModel))]
        public ActionResult ReturningObject()
        {
            var result = new SampleResponseModel()
            {
                CurrentTime = DateTime.Now.ToLongTimeString(),
                Message = "Hello!",
                Success = true
            };
            return Json(result);
        }

        [JQueryMethod(typeof(SampleResponseModel))]
        public ActionResult ReturningObjectWithParameters(string echo)
        {
            var result = new SampleResponseModel()
            {
                CurrentTime = DateTime.Now.ToLongTimeString(),
                Message = echo,
                Success = true
            };
            return Json(result);
        }

        [JQueryMethod(typeof(void))]
        public ActionResult VoidMethodWithParameters(SampleResponseModel model)
        {
            return Json(true);
        }
    }
}