using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebSockets;
using Reinforced.Typings.Samples.Difficult.CodeGenerators.Models;
using Reinforced.Typings.Samples.Difficult.CodeGenerators.ReinforcedTypings;

namespace Reinforced.Typings.Samples.Difficult.CodeGenerators.Controllers
{
    /// <summary>
    /// Our sample controller for testing queries made through jQuery
    /// </summary>
    public class JQueryController : Controller
    {
        [TsReturns(typeof(int))]
        public ActionResult SimpleIntControllerMethod()
        {
            return Json(0);
        }

        [TsReturns(typeof(string))]
        public ActionResult ControllerMethodWithParameters(int number, string s, bool boolValue)
        {
            return Json(string.Format("{0}-{1}-{2}", number, s, boolValue));
        }

        [TsReturns(typeof(JQuerySampleResponseModel))]
        public ActionResult ControllerReturningObject()
        {
            var result = new JQuerySampleResponseModel()
            {
                CurrentTime = DateTime.Now.ToLongTimeString(),
                Message = "Hello!",
                Success = true
            };
            return Json(result);
        }

        [TsReturns(typeof(JQuerySampleResponseModel))]
        public ActionResult ControllerReturningObjectWithParameters(string echo)
        {
            var result = new JQuerySampleResponseModel()
            {
                CurrentTime = DateTime.Now.ToLongTimeString(),
                Message = echo,
                Success = true
            };
            return Json(result);
        }

        [TsReturns(typeof(void))]
        public ActionResult VoidMethodWithParameters(JQuerySampleResponseModel model)
        {
            return null;
        }
    }
}