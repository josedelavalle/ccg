using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ccg.Controllers
{
    public class TemplatesController : Controller
    {
        public ActionResult NewsCard()
        {
            return PartialView();
        }

        public ActionResult CaseStudiesCard()
        {
            return PartialView();
        }

        public ActionResult LocationsCard()
        {
            return PartialView();
        }

    }
}