using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SuperSmartShopping.WEB.Areas.SecurePanel.Controllers
{
    [Compress]
    [SuperAdminAuthorize]
    public class DashboardController : Controller
    {        
        public ActionResult Index()
        {
            return View();
        }
    }
}