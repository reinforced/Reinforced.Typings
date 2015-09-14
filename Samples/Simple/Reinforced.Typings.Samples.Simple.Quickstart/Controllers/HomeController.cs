using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Reinforced.Typings.Samples.Simple.Quickstart.Models;

namespace Reinforced.Typings.Samples.Simple.Quickstart.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetOrder(int orderId)
        {
            var orderVm = new OrderViewModel()
            {
                //Address = "88 Coleman Parkway",
                ClientName = "Katherine Perkins",
                IsPaid = false,
                ItemName = "Exametazime",
                Quantity = 38,
                Subtotal = 297.72
            };

            return Json(orderVm, JsonRequestBehavior.AllowGet);
        }
    }
}