using Newtonsoft.Json;
using SuperSmartShopping.BAL.Interfaces;
using SuperSmartShopping.DAL.Common;
using SuperSmartShopping.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using static SuperSmartShopping.DAL.Enums.EnumHelper;

namespace SuperSmartShopping.WEB.Areas.Admin.Controllers
{
    [Compress]
    [AdminAuthorize]
    public class OrderController : BaseController
    {
        readonly string apiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"].ToString();
        readonly int _pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"].ToString());
        private IRiderBusiness _riderBusiness;
        public OrderController(IRiderBusiness riderBusiness)
        {
            _riderBusiness = riderBusiness;
        }
        public ActionResult Index(int page = 1, string searchStr = "")
        {
            List<OrderInfoModel> objOrders = GetOrders(0, string.Empty, string.Empty, false, page, _pageSize, searchStr);
            ViewBag.Pager = new PagerHelper(objOrders.Select(c => c.TotalRows).FirstOrDefault(), page, _pageSize);
            ViewBag.SearchStr = searchStr;
            ViewBag.RiderList = new SelectList(GetRiderList().ToList(), "Id", "FirstName");
            return View(objOrders);
        }

        public List<OrderInfoModel> GetOrders(int orderId, string orderNo, string status, bool IsCurrentDate, int pageNumber, int pageSize, string searchStr)
        {
            using (var client = new HttpClient())
            {
                List<OrderInfoModel> objList = new List<OrderInfoModel>();
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + SessionManager.LoginResponse.AccessToken);
                string url = apiBaseUrl + MethodEnum.GetOrders.GetDescription().ToString() + "?orderId=" + orderId + "&OrderNo=" + orderNo + "&OrderBy=&status=" + status + "&IsCurrentDate=" + IsCurrentDate + "&userId=" + SessionManager.LoginResponse.UserId + "&pageNumber=" + pageNumber + "&pageSize=" + pageSize + "&searchStr=" + searchStr;
                HttpResponseMessage messge = client.GetAsync(url).Result;
                string result = messge.Content.ReadAsStringAsync().Result;
                if (messge.IsSuccessStatusCode)
                {
                    objList = JsonConvert.DeserializeObject<List<OrderInfoModel>>(result);
                }
                return objList;
            }
        }

        [HttpPost]
        public PartialViewResult ViewOrderDetails(int OrderId)
        {
            OrderInfoModel obj = GetOrders(OrderId, string.Empty, string.Empty, false, 1, _pageSize, string.Empty).FirstOrDefault();
            return PartialView("_ViewOrderDetails", obj);
        }

        [HttpPost]
        public JsonResult UpdateStatus(OrderInfoModel model)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + SessionManager.LoginResponse.AccessToken);
                string url = apiBaseUrl + MethodEnum.UpdateOrderStatus.GetDescription().ToString() + "?orderId=" + model.OrderId + "&status=" + model.Status + "&userId=" + SessionManager.LoginResponse.UserId;
                HttpResponseMessage messge = client.PostAsync(url, null).Result;
                string result = messge.Content.ReadAsStringAsync().Result;
                if (messge.IsSuccessStatusCode)
                {
                    //sending order status in sms notification
                    // SMSManager.SendSMSNotification(ViewBag.CountryMobileCode, model.MobileNumber, ConfigurationManager.AppSettings["OrderPlacedMessage"].ToString());
                    return Json(new { res = 1 }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { res = -1 }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpPost]
        public JsonResult ViewPrintOrder(int OrderId, bool IsCustomerCopy)
        {
            string jsonStr = string.Empty;
            try
            {
                OrderInfoModel objOrderInfo = GetOrders(OrderId, string.Empty, string.Empty, false, 1, _pageSize, string.Empty).FirstOrDefault();
                objOrderInfo.IsCustomerBillCopy = IsCustomerCopy;
                ViewBag.billType = (objOrderInfo.IsCustomerBillCopy == false ? "(Store Copy)" : "(Customer Copy)");
                jsonStr = RenderPartialViewToString("_OrderPrint", objOrderInfo);
            }
            catch (Exception ex)
            {
                return Json(new { htmlStr = string.Empty, JsonRequestBehavior.AllowGet });
            }

            return Json(new { htmlStr = jsonStr, JsonRequestBehavior.AllowGet });
        }

        public List<RiderModel> GetRiderList()
        {
            using (var client = new HttpClient())
            {
                List<RiderModel> objList = new List<RiderModel>();
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + SessionManager.LoginResponse.AccessToken);
                string url = apiBaseUrl + MethodEnum.GetStoreLinkedRiders.GetDescription().ToString() + "?storeUserId=" + SessionManager.LoginResponse.UserId;
                HttpResponseMessage messge = client.GetAsync(url).Result;
                string result = messge.Content.ReadAsStringAsync().Result;
                if (messge.IsSuccessStatusCode)
                {
                    objList = JsonConvert.DeserializeObject<List<RiderModel>>(result);
                }
                return objList;
            }
        }

        [HttpPost]
        public JsonResult SendPushNotificationRider(int[] orderIds, int[] riderIds)
        {
            string jsonStr = string.Empty;
            try
            {
                //sending push notification to Rider
                foreach (var item in riderIds)
                {
                    var deviceToken = GetRiderList().Where(c => c.Id == item).Select(c => c.DeviceToken).FirstOrDefault();
                    if (!string.IsNullOrEmpty(deviceToken))
                    {
                        SMSManager.SendPushNotificationMessage(deviceToken, ConfigurationManager.AppSettings["FCMPushNotificationTitle"].ToString(), ConfigurationManager.AppSettings["FCMPushNotificationBody"].ToString());
                    }
                    //saving request
                    foreach (var orderId in orderIds)
                    {
                        _riderBusiness.SendOrderRequestToRider(orderId, item, SessionManager.LoginResponse.UserId);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
            return Json(1, JsonRequestBehavior.AllowGet);
        }

        #region "helper"
        public virtual string RenderPartialViewToString(string viewName, object viewmodel)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                viewName = this.ControllerContext.RouteData.GetRequiredString("action");
            }

            ViewData.Model = viewmodel;

            using (var sw = new StringWriter())
            {
                ViewEngineResult viewResult = System.Web.Mvc.ViewEngines.Engines.FindPartialView(this.ControllerContext, viewName);
                var viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);

                return sw.GetStringBuilder().ToString();
            }
        }
        #endregion
    }
}