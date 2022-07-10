using Newtonsoft.Json;
using SuperSmartShopping.BAL.Interfaces;
using SuperSmartShopping.DAL.Common;
using SuperSmartShopping.DAL.Models;
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
    public class SettingController : BaseController
    {
        #region "private member"
        readonly string apiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"].ToString();
        readonly string googleMapAPIKey = ConfigurationManager.AppSettings["GoogleMapAPIKey"].ToString();
        private IStoreBusiness _storeBusiness;
        private IBusinessHoursBusiness _businessHoursBusiness;
        private IQuickPagesBusiness _quickPagesBusiness;
        private ISocialMedialinksBusiness _socialMedialinksBusiness;
        readonly string appPhysicalPath = ConfigurationManager.AppSettings["BackendPhysicalPath"].ToString();
        #endregion

        #region "constructor"
        public SettingController(IStoreBusiness storeBusiness, IBusinessHoursBusiness businessHoursBusiness, IQuickPagesBusiness quickPagesBusiness, ISocialMedialinksBusiness socialMedialinksBusiness)
        {
            _storeBusiness = storeBusiness;
            _businessHoursBusiness = businessHoursBusiness;
            _quickPagesBusiness = quickPagesBusiness;
            _socialMedialinksBusiness = socialMedialinksBusiness;
        }
        #endregion

        #region "profile"
        public ActionResult Index()
        {
            List<BannerImages> bannerList = _storeBusiness.GetBannerImage(0, "HOME", CommonManager.GetTenantConnection(SessionManager.LoginResponse.UserId, string.Empty).FirstOrDefault().TenantConnection).Where(c=>c.IsDeleted==false).ToList();
            StoreModel obj = new StoreModel();
            obj = GetStore();
            obj.BannerList = bannerList;
            return View(obj);
        }
        #endregion

        #region "configuration"
        public ActionResult Configuration()
        {
            ViewBag.GoogleMapAPIKey = googleMapAPIKey;
            return View(GetStore());
        }

        [HttpPost]
        public ActionResult AddUpdateStripeKey(string publishablekey, string secretkey)
        {
            int res = _storeBusiness.AddUpdateStripeAPIKey(publishablekey, secretkey, CommonManager.GetTenantConnection(SessionManager.LoginResponse.UserId, string.Empty).FirstOrDefault().TenantConnection);
            return Content(res.ToString());
        }

        [HttpPost]
        public ActionResult AddUpdateDeliveryChargesTaxes(decimal tax, decimal charges, bool isCashOnDelivery)
        {
            int res = _storeBusiness.UpdateDeliveryChargesTaxes(charges, tax, isCashOnDelivery, CommonManager.GetTenantConnection(SessionManager.LoginResponse.UserId, string.Empty).FirstOrDefault().TenantConnection);
            return Content(res.ToString());
        }

        [HttpPost]
        public ActionResult AddUpdateDeliveryAreaSetting(decimal minOrderAmt, decimal maxDeliveryAreaInMiles)
        {
            int res = _storeBusiness.UpdateDeliveryAreaSetting(minOrderAmt, maxDeliveryAreaInMiles, CommonManager.GetTenantConnection(SessionManager.LoginResponse.UserId, string.Empty).FirstOrDefault().TenantConnection);
            return Content(res.ToString());
        }
        #endregion

        #region "business hour"
        public ActionResult BusinessHour()
        {
            List<BusinessHoursModel> objList = _businessHoursBusiness.GetBusinessHours(CommonManager.GetTenantConnection(SessionManager.LoginResponse.UserId, string.Empty).FirstOrDefault().TenantConnection);
            return View(objList);
        }

        [HttpPost]
        public ActionResult AddUpdateBusinessHours(List<BusinessHoursModel> weekDays)
        {
            var jsonStr = JsonConvert.SerializeObject(weekDays);
            int res = _businessHoursBusiness.AddUpdateBusinessHours(jsonStr, CommonManager.GetTenantConnection(SessionManager.LoginResponse.UserId, string.Empty)[0].TenantConnection);
            return Content(res.ToString());
        }
        #endregion

        #region "static pages"
        public ActionResult AddUpdateQuickPages()
        {
            ViewBag.UserId = SessionManager.LoginResponse.UserId;
            List<QuickPageModel> objList = _quickPagesBusiness.GetQuickPages(CommonManager.GetTenantConnection(SessionManager.LoginResponse.UserId, string.Empty)[0].TenantConnection);
            return View(objList);
        }

        [HttpPost]
        public ActionResult AddUpdateQuickPages(List<QuickPageModel> model)
        {
            try
            {
                string conn = CommonManager.GetTenantConnection(SessionManager.LoginResponse.UserId, string.Empty)[0].TenantConnection;
                foreach (var item in model)
                {
                    int res = _quickPagesBusiness.AddUpdateQuickPages(item, conn);
                }

                return Content("1");
            }
            catch (Exception ex)
            {
                return Content(ex.Message.ToString());
            }
        }
        #endregion

        #region "social media link"
        public ActionResult AddUpdateSocialMediaLinks()
        {
            List<SocialMedia> objList = _socialMedialinksBusiness.GetSocialMediaLinks(CommonManager.GetTenantConnection(SessionManager.LoginResponse.UserId, string.Empty)[0].TenantConnection);
            return View(objList);
        }

        [HttpPost]
        public ActionResult AddUpdateSocialMedia(List<SocialMedia> model)
        {
            var jsonStr = JsonConvert.SerializeObject(model);
            int res = _socialMedialinksBusiness.AddUpdateSocialMediaLinks(jsonStr, SessionManager.LoginResponse.UserId, CommonManager.GetTenantConnection(SessionManager.LoginResponse.UserId, string.Empty)[0].TenantConnection);
            return Content(res.ToString());
        }
        #endregion

        #region "subscription"
        public ActionResult Subscription()
        {
            return View();
        }
        #endregion

        #region "banner"
        [HttpPost]
        public ActionResult AddUpdateBannerImage(string[] bannerImages)
        {
            try
            {
                BannerImages obj = null;
                List<BannerImages> objList = new List<BannerImages>();
                if (bannerImages != null && bannerImages.Length > 0)
                {
                    foreach (var item in bannerImages)
                    {
                        string fileName = string.Empty, fullPath = string.Empty, directoryPath = string.Empty, fileUploadPath = string.Empty;
                        obj = new BannerImages();
                        obj.CreatedBy = SessionManager.LoginResponse.UserId;
                        obj.ImagePath = item.Split(',')[1];
                        if (CommonManager.IsBase64(obj.ImagePath))
                        {
                            byte[] imageBytes = Convert.FromBase64String(obj.ImagePath);
                            fileUploadPath = DirectoryPathEnum.Upload.ToString() + "/" + obj.CreatedBy + "/" + DirectoryPathEnum.HomeBannerImage.ToString() + "/";
                            directoryPath = CommonManager.CreatePathIfMissing(appPhysicalPath + "/" + fileUploadPath);
                            fileName = CommonManager.CreateUniqueFileName(ExtensionEnum.PNGExtension.GetDescription().ToString());
                            fullPath = directoryPath + fileName;
                            System.IO.File.WriteAllBytes(fullPath, imageBytes);
                            obj.ImagePath = "/" + fileUploadPath + fileName;
                        }
                        else if (!string.IsNullOrEmpty(obj.ImagePath))
                        {
                            obj.ImagePath = null;
                        }

                        obj.BannerType = "Home";
                        obj.IsActive = true;
                        obj.CreatedBy = SessionManager.LoginResponse.UserId;
                        objList.Add(obj);
                    }

                    int res = _storeBusiness.AddBannerImage(objList, CommonManager.GetTenantConnection(SessionManager.LoginResponse.UserId, string.Empty)[0].TenantConnection);
                    return Content(res.ToString());
                }
                else
                {
                    return Content("-1");
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message.ToString());
            }
        }

        [HttpGet]
        public JsonResult OpenBannerImage()
        {
            string ret = RenderPartialToString("~/Areas/Admin/Views/Setting/_AddBanner.cshtml", null, ControllerContext);
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult MarkAsDelete(int id)
        {
            int res = _storeBusiness.MarkBannerAsDeleted(id, CommonManager.GetTenantConnection(SessionManager.LoginResponse.UserId, string.Empty)[0].TenantConnection);
            return Content(res.ToString());
        }

        [HttpPost]
        public ActionResult MarkAsActiveInActive(int id, bool status)
        {
            int res = _storeBusiness.MarkAsActiveInActive(id, (status == true ? false : true), CommonManager.GetTenantConnection(SessionManager.LoginResponse.UserId, string.Empty)[0].TenantConnection);
            return Content(res.ToString());
        }

        #endregion

        #region "Change Password"
        public ActionResult ChangePassword()
        {
            return View(new ChangePasswordModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            using (var client = new HttpClient())
            {
                string res = string.Empty;
                string url = apiBaseUrl + MethodEnum.ChangePassword.GetDescription().ToString();
                var httpContent = CommonManager.CreateHttpContent(model);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + SessionManager.LoginResponse.AccessToken);
                HttpResponseMessage messge = client.PostAsync(url, httpContent).Result;
                string result = messge.Content.ReadAsStringAsync().Result;
                if (messge.IsSuccessStatusCode)
                {
                    res = "1";
                }
                else
                {
                    res = JsonConvert.DeserializeObject<string>(result);
                }
                return Content(res);
            }
        }
        #endregion

        #region "helper"
        public StoreModel GetStore()
        {
            using (var client = new HttpClient())
            {
                StoreModel obj = new StoreModel();
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + SessionManager.LoginResponse.AccessToken);
                string url = apiBaseUrl + MethodEnum.GetStore.GetDescription().ToString() + "?userId=" + SessionManager.LoginResponse.UserId;
                HttpResponseMessage messge = client.GetAsync(url).Result;
                string result = messge.Content.ReadAsStringAsync().Result;
                if (messge.IsSuccessStatusCode)
                {
                    obj = JsonConvert.DeserializeObject<StoreModel>(result);
                }
                return obj;
            }
        }
        public static string RenderPartialToString(string viewName, object model, ControllerContext ControllerContext)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");
            ViewDataDictionary ViewData = new ViewDataDictionary();
            TempDataDictionary TempData = new TempDataDictionary();
            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }

        }
        #endregion

    }
}