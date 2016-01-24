using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Reinforced.Typings.Samples.Difficult.CodeGenerators.Models;
using Reinforced.Typings.Samples.Difficult.CodeGenerators.ReinforcedTypings.Angular;

namespace Reinforced.Typings.Samples.Difficult.CodeGenerators.Controllers
{
    public class AngularController : Controller
    {
        [AngularMethod(typeof(int))]
        public ActionResult SimpleIntMethod()
        {
            return Json(new Random().Next(100));
        }

        [AngularMethod(typeof(string))]
        public ActionResult MethodWithParameters(int num, string s, bool boolValue)
        {
            return Json(string.Format("{0}-{1}-{2}", num, s, boolValue));
        }

        [AngularMethod(typeof(SampleResponseModel))]
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

        [AngularMethod(typeof(SampleResponseModel))]
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

        [AngularMethod(typeof(void))]
        public ActionResult VoidMethodWithParameters(SampleResponseModel model)
        {
            return null;
        }
    }
}