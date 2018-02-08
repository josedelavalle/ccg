using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ccg.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Jose DeLavalle - CCG";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "CCG Interview App";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "CCG Interview App";

            return View();
        }
    }
}
