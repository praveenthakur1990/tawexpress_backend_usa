using Newtonsoft.Json;
using SuperSmartShopping.BAL.Interfaces;
using SuperSmartShopping.DAL.Common;
using SuperSmartShopping.DAL.Enums;
using SuperSmartShopping.DAL.Models;
using SuperSmartShopping.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using static SuperSmartShopping.DAL.Enums.EnumHelper;

namespace SuperSmartShopping.WEB.Areas.SecurePanel.Controllers
{
    [Compress]
    [SuperAdminAuthorize]
    public class StoreController : Controller
    {
        #region "private member"
        readonly string apiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"].ToString();
        readonly string appPhysicalPath = ConfigurationManager.AppSettings["BackendPhysicalPath"].ToString();
        private IStoreBusiness _storeBusiness;
        #endregion
        public StoreController(IStoreBusiness storeBusiness)
        {
            _storeBusiness = storeBusiness;
        }

        #region "Store"
        public ActionResult Index()
        {
            List<TenantModel> objList = new List<TenantModel>();
            if (SessionManager.LoginResponse.RoleName == RolesEnum.SuperAdmin.ToString())
            {
                objList = CommonManager.GetTenantConnection(string.Empty, string.Empty);
            }
            else
            {
                objList = CommonManager.GetTenantConnection(string.Empty, string.Empty).Where(c => c.CreatedBy == SessionManager.LoginResponse.UserId).ToList();
            }
            return View(objList);
        }

        public ActionResult AddUpdate(string userId, string subDomain)
        {
            StoreModel response = new StoreModel();
            ViewBag.Plans = new SelectList(GetPlans(), "Id", "NamePrice");
            ViewBag.SubDomain = subDomain == null ? string.Empty : subDomain;
            if (!string.IsNullOrEmpty(userId))
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + SessionManager.LoginResponse.AccessToken);
                    string url = apiBaseUrl + MethodEnum.GetStore.GetDescription().ToString() + "?userId=" + userId;
                    HttpResponseMessage messge = client.GetAsync(url).Result;
                    string result = messge.Content.ReadAsStringAsync().Result;
                    if (messge.IsSuccessStatusCode)
                    {
                        response = JsonConvert.DeserializeObject<StoreModel>(result);
                    }
                }
            }
            return View(response);
        }

        [HttpPost]
        public int AddUpdate(FormCollection frm)
        {
            try
            {
                string base64StringGST = string.Empty, base64StringLogo = string.Empty, fullPath = string.Empty, fileName = string.Empty, uploadedQRPath = string.Empty, callBackUrl = string.Empty;
                if (string.IsNullOrEmpty(frm["QrCodePath"].ToString()))
                {
                    string host = HttpContext.Request.Url.Host;
                    if (host.Contains("localhost"))
                    {
                        callBackUrl = string.Format("{0}.{1}/#/{2}", frm["subDomainName"].ToString(), host, "subscriber/index");
                    }
                    else
                    {
                        callBackUrl = string.Format("{0}.{1}/#/{2}", frm["subDomainName"].ToString(), ConfigurationManager.AppSettings["hostName"].ToString(), "subscriber/index");
                    }

                    CommonManager.LogError(MethodBase.GetCurrentMethod(), new Exception(), callBackUrl, frm);

                    UriBuilder baseUri = new UriBuilder("https://" + callBackUrl);
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), new Exception(), baseUri.ToString(), frm);
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), new Exception(), baseUri.Uri.ToString(), frm);

                    string directoryPath = CommonManager.CreatePathIfMissingFromWeb("/" + DirectoryPathEnum.Upload.ToString() + "/" + frm["Email"].ToString() + "/" + DirectoryPathEnum.QrCode.ToString() + "/");
                    uploadedQRPath = CommonManager.GenerateQrCode(baseUri.Uri.ToString(), directoryPath);
                }
                else
                {
                    uploadedQRPath = frm["QrCodePath"].ToString().Substring(HttpContext.Request["QrCodePath"].ToString().IndexOf("/Upload")); ;
                }

                try
                {
                    base64StringGST = HttpContext.Request["GSTFile"] != string.Empty && HttpContext.Request["GSTFile"] != null ? HttpContext.Request["GSTFile"].Split(',')[1] : null;
                }
                catch (Exception ex)
                {
                    base64StringGST = HttpContext.Request["hdnGSTFile"].ToString().Substring(HttpContext.Request["hdnGSTFile"].ToString().IndexOf("/Upload"));
                }

                try
                {
                    base64StringLogo = HttpContext.Request["LogoFile"] != string.Empty && HttpContext.Request["LogoFile"] != null ? HttpContext.Request["LogoFile"].Split(',')[1] : null;
                }
                catch (Exception ex)
                {
                    base64StringLogo = HttpContext.Request["hdnLogoFile"].ToString().Substring(HttpContext.Request["hdnLogoFile"].ToString().IndexOf("/Upload"));
                }

                string hdnGSTFilePath = base64StringGST != null ? base64StringGST : HttpContext.Request["hdnGSTFile"];
                string hdnLogoFilePath = base64StringLogo != null ? base64StringLogo : HttpContext.Request["hdnLogoFile"];
                StoreModel obj = new StoreModel();
                obj.Id = Convert.ToInt32(frm["Id"].ToString());
                obj.Name = frm["Name"].ToString();
                obj.Email = frm["Email"].ToString();
                if (obj.Id == 0)
                {
                    obj.Email = string.Format("{0}_{1}", obj.Email, frm["subDomainName"].ToString());
                }
                obj.Mobile = frm["Mobile"].ToString();
                obj.Address = frm["Address"].ToString();
                obj.State = frm["state"].ToString();
                obj.City = frm["City"].ToString();
                obj.ZipCode = frm["ZipCode"].ToString();
                obj.CountryCode = frm["CountryCode"].ToString();
                obj.Latitude = frm["Latitude"].ToString();
                obj.Longitude = frm["Longitude"].ToString();
                obj.ContactPersonName = frm["ContactPersonName"].ToString();
                obj.ContactNumber = frm["ContactNumber"].ToString();
                obj.GSTRegistrationNumber = frm["GSTRegistrationNumber"].ToString();
                obj.ActivePlan = frm["ActivePlan"].ToString();
                obj.Commision = Convert.ToDecimal(frm["Commision"].ToString() == string.Empty ? "0" : frm["Commision"].ToString());
                obj.PlanActiveDate = Convert.ToDateTime(frm["PlanActiveDate"].ToString());
                obj.CurrencySymbol = frm["CurrencySymbol"].ToString();
                obj.TimeZone = frm["TimeZone"].ToString();
                obj.CreatedBy = SessionManager.LoginResponse.UserId;
                obj.GSTFilePath = hdnGSTFilePath;
                obj.LogoPath = hdnLogoFilePath;
                obj.PlanId = obj.ActivePlan == PlanEnum.FREE.GetDescription().ToString() ? 0 : !string.IsNullOrEmpty(frm["PlanId"].ToString()) ? Convert.ToInt32(frm["PlanId"].ToString()) : 0;
                obj.QrCodePath = uploadedQRPath;
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + SessionManager.LoginResponse.AccessToken);
                    string url = apiBaseUrl + MethodEnum.AddUpdateStore.GetDescription().ToString();
                    var httpContent = CommonManager.CreateHttpContent(obj);
                    HttpResponseMessage messge = client.PostAsync(url, httpContent).Result;
                    string result = messge.Content.ReadAsStringAsync().Result;
                    if (messge.IsSuccessStatusCode)
                    {
                        return obj.Id > 0 ? 2 : 1;
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
            catch (Exception ex)
            {
                CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, frm);
                return 0;
            }
        }
        #endregion

        #region "Plan"
        public List<PlanModel> GetPlans()
        {
            using (var client = new HttpClient())
            {
                List<PlanModel> objList = new List<PlanModel>();
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + SessionManager.LoginResponse.AccessToken);
                string url = apiBaseUrl + MethodEnum.GetPlanList.GetDescription().ToString();
                HttpResponseMessage messge = client.GetAsync(url).Result;
                string result = messge.Content.ReadAsStringAsync().Result;
                if (messge.IsSuccessStatusCode)
                {
                    objList = JsonConvert.DeserializeObject<List<PlanModel>>(result);
                }
                return objList;
            }
        }
        #endregion

        #region "Store advertisement image"
        public ActionResult AdvertisementImage(string userId)
        {
            List<BannerImages> bannerList = _storeBusiness.GetBannerImage(0, "Advertisement", CommonManager.GetTenantConnection(userId, string.Empty).FirstOrDefault().TenantConnection).Where(c => c.IsDeleted == false).ToList();
            return View(bannerList);
        }
              
        public JsonResult OpenBannerImage()
        {           
            string ret = RenderPartialToString("~/Areas/SecurePanel/Views/Store/_AddBanner.cshtml", null, ControllerContext);
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddUpdateBannerImage(string[] bannerImages, string storeId)
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
                            fileUploadPath = DirectoryPathEnum.Upload.ToString() + "/" + storeId + "/" + DirectoryPathEnum.AdvertisementImage.ToString() + "/";
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

                        obj.BannerType = "Advertisement";
                        obj.IsActive = true;
                        obj.CreatedBy = SessionManager.LoginResponse.UserId;
                        objList.Add(obj);
                    }

                    int res = _storeBusiness.AddBannerImage(objList, CommonManager.GetTenantConnection(storeId, string.Empty)[0].TenantConnection);
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

        [HttpPost]
        public ActionResult MarkAsDelete(int id, string storeId)
        {
            int res = _storeBusiness.MarkBannerAsDeleted(id, CommonManager.GetTenantConnection(storeId, string.Empty)[0].TenantConnection);
            return Content(res.ToString());
        }

        [HttpPost]
        public ActionResult MarkAsActiveInActive(int id, bool status, string storeId)
        {
            int res = _storeBusiness.MarkAsActiveInActive(id, (status == true ? false : true), CommonManager.GetTenantConnection(storeId, string.Empty)[0].TenantConnection);
            return Content(res.ToString());
        }
        #endregion

        #region "helper"
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