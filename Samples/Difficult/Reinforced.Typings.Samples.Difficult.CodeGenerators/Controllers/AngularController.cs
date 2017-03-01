using System;
using System.Web.Mvc;
using Reinforced.Typings.Samples.Difficult.CodeGenerators.Models;
using Reinforced.Typings.Samples.Difficult.CodeGenerators.ReinforcedTypings.Angular;

namespace Reinforced.Typings.Samples.Difficult.CodeGenerators.Controllers
{
    /// <summary>
    /// Controller containig methods to be invoked from AngularJS
    /// </summary>
    public class AngularController : Controller
    {
        /// <summary>
        /// Simple controller action that returns integer method
        /// </summary>
        /// <returns>JSON-ed random integer value</returns>
        [AngularMethod(typeof(int))]
        public ActionResult SimpleIntMethod()
        {
            return Json(new Random().Next(100));
        }

        /// <summary>
        /// Controller action that returns 
        /// </summary>
        /// <param name="num">Number value</param>
        /// <param name="s">String value</param>
        /// <param name="boolValue">Boolean value</param>
        /// <returns>JSON-ed string containing concatenated values</returns>
        [AngularMethod(typeof(string))]
        public ActionResult MethodWithParameters(int num, string s, bool boolValue)
        {
            return Json(string.Format("{0}-{1}-{2}", num, s, boolValue));
        }

        /// <summary>
        /// Controller action that returns our simple object
        /// </summary>
        /// <returns>JSON-ed simple object</returns>
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

        /// <summary>
        ///  Controller action that returns our simple object and consimes parameters
        /// </summary>
        /// <param name="echo">Sample parameter</param>
        /// <returns>JSON-ed simple object</returns>
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

        /// <summary>
        /// Controller action that does not return anything but consumes object as parameter
        /// </summary>
        /// <param name="model">Object parameter</param>
        /// <returns>Nothing</returns>
        [AngularMethod(typeof(void))]
        public ActionResult VoidMethodWithParameters(SampleResponseModel model)
        {
            return null;
        }
    }
}