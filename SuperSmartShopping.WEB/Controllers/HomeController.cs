using SuperSmartShopping.DAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SuperSmartShopping.WEB.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public string SendWeeklyCircularInvite()
        {
            Dictionary<string, string> numbersList = new Dictionary<string, string>();
            numbersList.Add("9876693766","1");
            numbersList.Add("9876540540","2");
            numbersList.Add("9816810805","3");

            string message = "This is test message";         
            var result = SMSManager.SendBulkSMSNotification(numbersList, message, "");
            if (result.ContainsKey("Error"))
            {
                return result["Error"].ToString();
            }
            else
            {
                return "1";
            }
        }
    }
}