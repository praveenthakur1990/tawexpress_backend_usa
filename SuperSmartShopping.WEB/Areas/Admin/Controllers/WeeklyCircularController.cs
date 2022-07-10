using Newtonsoft.Json;
using SuperSmartShopping.BAL.Interfaces;
using SuperSmartShopping.BAL.Services;
using SuperSmartShopping.DAL.Common;
using SuperSmartShopping.DAL.Models;
using SuperSmartShopping.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using static SuperSmartShopping.DAL.Enums.EnumHelper;

namespace SuperSmartShopping.WEB.Areas.Admin.Controllers
{
    [Compress]
    [AdminAuthorize]
    public class WeeklyCircularController : BaseController
    {
        readonly string apiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"].ToString();
        readonly int _pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"].ToString());
        readonly IMessageResourcesBusiness _messageResourcesBusiness;
        public WeeklyCircularController(IMessageResourcesBusiness messageResourcesBusiness)
        {
            _messageResourcesBusiness = messageResourcesBusiness;
        }
        public ActionResult Index(int page = 1, string searchStr = "")
        {
            List<WeeklyCircularModel> objList = GetWeeklyCircular(0, page, _pageSize, searchStr);
            ViewBag.Pager = new PagerHelper(objList.Select(c => c.TotalRows).FirstOrDefault(), page, _pageSize);
            ViewBag.SearchStr = searchStr;
            return View(objList);
        }

        public ActionResult AddUpdate(int id)
        {
            WeeklyCircularModel obj = new WeeklyCircularModel();
            if (id > 0)
            {
                obj = GetWeeklyCircular(id, 1, _pageSize, string.Empty).FirstOrDefault();
            }
            return View(obj);
        }

        [HttpPost]
        public int AddUpdate(FormCollection frm)
        {
            try
            {
                string base64StringFSSI = string.Empty, thumbnailBase64String = string.Empty, hdnFSSAIFilePath = string.Empty, hdnThumbnailFilePath = string.Empty;
                try
                {
                    base64StringFSSI = HttpContext.Request["PdfFilePath"] != string.Empty && HttpContext.Request["PdfFilePath"] != null ? HttpContext.Request["PdfFilePath"].Split(',')[1] : null;
                }
                catch (Exception ex)
                {
                    base64StringFSSI = null;
                }

                hdnFSSAIFilePath = base64StringFSSI != null ? base64StringFSSI : HttpContext.Request["hdnUploadFile"];

                try
                {
                    thumbnailBase64String = HttpContext.Request["ThubnailFile"] != string.Empty && HttpContext.Request["ThubnailFile"] != null ? HttpContext.Request["ThubnailFile"].Split(',')[1] : null;
                }
                catch (Exception ex)
                {
                    thumbnailBase64String = null;
                }

                hdnThumbnailFilePath = thumbnailBase64String != null ? thumbnailBase64String : HttpContext.Request["hdnThubnailFile"];

                WeeklyCircularModel obj = new WeeklyCircularModel();
                obj.Id = Convert.ToInt32(frm["Id"].ToString());
                obj.Title = frm["Title"].ToString();
                obj.StartDate = Convert.ToDateTime(frm["StartDate"].ToString());
                obj.EndDate = Convert.ToDateTime(frm["EndDate"].ToString());
                obj.PdfFilePath = hdnFSSAIFilePath;
                obj.ThumbnailImgPath = hdnThumbnailFilePath;
                obj.CreatedBy = SessionManager.LoginResponse.UserId;
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + SessionManager.LoginResponse.AccessToken);
                    string url = apiBaseUrl + MethodEnum.AddUpdateWeeklyCircular.GetDescription().ToString();
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
                return 0;
            }
        }

        public ActionResult ViewProduct(int id)
        {
            WeeklyCircularModel obj = GetWeeklyCircular(id, 1, _pageSize, string.Empty).FirstOrDefault();
            obj.WeeklyCircularProductList = GetProductsByWeeklyCircularId(0, id);
            if (obj == null)
            {
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        [HttpPost]
        public JsonResult GetAddUpdateProduct(int weeklyCircularId, int catId = 0, int subCatId = 0, int weeklyCircularCatId = 0)
        {
            WeeklyCircularCatInfoModel obj = new WeeklyCircularCatInfoModel();
            obj.WeeklyCircularCatId = weeklyCircularCatId;
            obj.WeeklyCircularId = weeklyCircularId;
            obj.CategoryId = catId;
            obj.SubCategoryId = subCatId;
            obj.Categories = GetCategories().Where(c => c.ParentId == 0 && c.IsActive == true && c.IsDeleted == false).ToList();
            obj.SubCategories = GetCategories().Where(c => c.ParentId == obj.CategoryId && c.IsActive == true && c.IsDeleted == false).ToList();
            string ret = RenderPartialToString("~/Areas/Admin/Views/WeeklyCircular/_AddUpdateProduct.cshtml", obj, ControllerContext);
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetProductsBySubCategories(int catId, int subCatId, int weeklyCircularId)
        {
            var result = GetProducts(catId, subCatId).ToList();
            var weeklyCircularProduct = GetProductsByWeeklyCircularId(catId, weeklyCircularId);
            var finalResult = (from c in result
                               join d in weeklyCircularProduct on c.Id equals d.Id into ps
                               from d in ps.DefaultIfEmpty()
                               select new ProductDashboardModel
                               {
                                   Id = c.Id,
                                   CategoryId = c.CategoryId,
                                   SubCategoryId = c.SubCategoryId,
                                   ProductName = c.ProductName,
                                   Price = c.Price,
                                   ProductImage = c.ProductImage,
                                   CategoryImage = c.CategoryImage,
                                   SubCategoryName = c.SubCategoryName,
                                   IsVarient = c.IsVarient,
                                   ProductVarients = c.ProductVarients,
                                   DefaultVarientId = c.DefaultVarientId,
                                   Description = c.Description,
                                   IsDescriptionShow = c.IsDescriptionShow,
                                   TagIds = c.TagIds,
                                   WeeklyCircularId = d != null ? d.WeeklyCircularId : 0,
                                   OfferType = d != null ? d.OfferType : "",
                                   OfferValue = d != null ? d.OfferValue : 0,
                                   FinalOfferValue = d != null ? d.FinalOfferValue : "",
                                   FinalValue = d != null ? d.FinalValue : "",
                                   WeeklyCircularCatId = d != null ? d.WeeklyCircularCatId : 0,
                                   WeeklyCircularProductId = d != null ? d.WeeklyCircularProductId : 0
                               }).ToList();

            string ret = RenderPartialToString("~/Areas/Admin/Views/WeeklyCircular/_LoadProductsBySubCat.cshtml", finalResult, ControllerContext);
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public int AddUpdateProduct(FormCollection frm)
        {
            try
            {
                WeeklyCircularCatInfoModel obj = new WeeklyCircularCatInfoModel();
                obj.WeeklyCircularCatId = Convert.ToInt32(frm["WeeklyCircularCatId"].ToString());
                obj.WeeklyCircularId = Convert.ToInt32(frm["WeeklyCircularId"].ToString());
                obj.CategoryId = Convert.ToInt32(frm["CategoryId"].ToString());
                obj.SubCategoryId = Convert.ToInt32(frm["SubCategoryId"].ToString());
                obj.CreatedBy = SessionManager.LoginResponse.UserId;
                obj.ProductList = JsonConvert.DeserializeObject<List<WeeklyCircularProductsModel>>(frm["SelectedProduct"].ToString());
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + SessionManager.LoginResponse.AccessToken);
                    string url = apiBaseUrl + MethodEnum.AddUpdateWeeklyCircularProduct.GetDescription().ToString();
                    var httpContent = CommonManager.CreateHttpContent(obj);
                    System.Net.Http.HttpResponseMessage messge = client.PostAsync(url, httpContent).Result;
                    string result = messge.Content.ReadAsStringAsync().Result;
                    if (messge.IsSuccessStatusCode)
                    {
                        return obj.WeeklyCircularCatId > 0 ? 2 : 1;
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        [HttpPost]
        public JsonResult GetSubCategories(int id)
        {
            var result = new SelectList(GetCategories().Where(c => c.ParentId == id && c.IsActive == true && c.IsDeleted == false).ToList(), "Id", "Name");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public List<Category> GetCategories()
        {
            using (var client = new HttpClient())
            {
                List<Category> objList = new List<Category>();
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + SessionManager.LoginResponse.AccessToken);
                string url = apiBaseUrl + MethodEnum.GetCategories.GetDescription().ToString() + "?type='All'&userId=" + SessionManager.LoginResponse.UserId + "&callingBy='subcat'";
                HttpResponseMessage messge = client.GetAsync(url).Result;
                string result = messge.Content.ReadAsStringAsync().Result;
                if (messge.IsSuccessStatusCode)
                {
                    objList = JsonConvert.DeserializeObject<List<Category>>(result);
                }
                return objList;
            }
        }

        public List<WeeklyCircularModel> GetWeeklyCircular(int id, int pageNumber, int pageSize, string searchStr)
        {
            using (var client = new HttpClient())
            {
                List<WeeklyCircularModel> objList = new List<WeeklyCircularModel>();
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + SessionManager.LoginResponse.AccessToken);
                string url = apiBaseUrl + MethodEnum.GetWeeklyCircular.GetDescription().ToString() + "?id=" + id + "&userId=" + SessionManager.LoginResponse.UserId + "&pageNumber=" + pageNumber + "&pageSize=" + pageSize + "&searchStr=" + searchStr;
                HttpResponseMessage messge = client.GetAsync(url).Result;

                string result = messge.Content.ReadAsStringAsync().Result;
                if (messge.IsSuccessStatusCode)
                {
                    objList = JsonConvert.DeserializeObject<List<WeeklyCircularModel>>(result);
                }
                else
                {
                    //if(messge.StatusCode==403)
                }
                return objList;
            }
        }

        public List<WeeklyCircularModel> GetWeeklyCircularCatInfo(int id, int pageNumber, int pageSize, string searchStr)
        {
            using (var client = new HttpClient())
            {
                List<WeeklyCircularModel> objList = new List<WeeklyCircularModel>();
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + SessionManager.LoginResponse.AccessToken);
                string url = apiBaseUrl + MethodEnum.GetWeeklyCircularCatInfo.GetDescription().ToString() + "?id=" + id + "&userId=" + SessionManager.LoginResponse.UserId + "&pageNumber=" + pageNumber + "&pageSize=" + pageSize + "&searchStr=" + searchStr;
                HttpResponseMessage messge = client.GetAsync(url).Result;

                string result = messge.Content.ReadAsStringAsync().Result;
                if (messge.IsSuccessStatusCode)
                {
                    objList = JsonConvert.DeserializeObject<List<WeeklyCircularModel>>(result);
                }
                else
                {
                    //if(messge.StatusCode==403)
                }
                return objList;
            }
        }

        public List<ProductDashboardModel> GetProducts(int categoryId, int subCategoryIds)
        {
            using (var client = new HttpClient())
            {
                List<ProductDashboardModel> objList = new List<ProductDashboardModel>();
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + SessionManager.LoginResponse.AccessToken);
                string url = apiBaseUrl + MethodEnum.GetProductBySubCategoryId.GetDescription().ToString() + "?categoryId=" + (categoryId > 0 ? categoryId.ToString() : string.Empty) + "&subCategoryIds=" + (subCategoryIds > 0 ? subCategoryIds.ToString() : string.Empty) + "&brandIds=&userId=" + SessionManager.LoginResponse.UserId;
                HttpResponseMessage messge = client.GetAsync(url).Result;
                string result = messge.Content.ReadAsStringAsync().Result;
                if (messge.IsSuccessStatusCode)
                {
                    objList = JsonConvert.DeserializeObject<List<ProductDashboardModel>>(result);
                }
                return objList;
            }
        }

        public List<ProductDashboardModel> GetProductsByWeeklyCircularId(int categoryId, int weeklyCircularId = 0)
        {
            using (var client = new HttpClient())
            {
                List<ProductDashboardModel> objList = new List<ProductDashboardModel>();
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + SessionManager.LoginResponse.AccessToken);
                string url = apiBaseUrl + MethodEnum.GetProductByWeeklyCircularId.GetDescription().ToString() + "?categoryId=" + (categoryId > 0 ? categoryId.ToString() : string.Empty) + "&userId=" + SessionManager.LoginResponse.UserId + "&weeklyCircularId=" + weeklyCircularId;
                HttpResponseMessage messge = client.GetAsync(url).Result;
                string result = messge.Content.ReadAsStringAsync().Result;
                if (messge.IsSuccessStatusCode)
                {
                    objList = JsonConvert.DeserializeObject<List<ProductDashboardModel>>(result);
                }
                return objList;
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

        #region "weeklycircular subscriber"
        public ActionResult Subscriber(int page = 1, string searchStr = "")
        {
            List<WeeklyCircularSubscriberModel> objList = GetSubscribers(page, _pageSize, searchStr);
            ViewBag.Pager = new PagerHelper(objList.Select(c => c.TotalRows).FirstOrDefault(), page, _pageSize);
            ViewBag.SearchStr = searchStr;
            return View(objList);
        }

        public List<WeeklyCircularSubscriberModel> GetSubscribers(int pageNumber, int pageSize, string searchStr)
        {
            using (var client = new HttpClient())
            {
                List<WeeklyCircularSubscriberModel> objList = new List<WeeklyCircularSubscriberModel>();
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + SessionManager.LoginResponse.AccessToken);
                string url = apiBaseUrl + MethodEnum.GetWeeklyCircularSubscribers.GetDescription().ToString() + "?userId=" + SessionManager.LoginResponse.UserId + "&pageNumber=" + pageNumber + "&pageSize=" + pageSize + "&searchStr=" + searchStr;
                HttpResponseMessage messge = client.GetAsync(url).Result;
                string result = messge.Content.ReadAsStringAsync().Result;
                if (messge.IsSuccessStatusCode)
                {
                    objList = JsonConvert.DeserializeObject<List<WeeklyCircularSubscriberModel>>(result);
                }
                return objList;
            }
        }

        [HttpGet]
        public JsonResult GetWeeklyCircularCompose()
        {
            WeeklyCircularComposeModel objCompose = new WeeklyCircularComposeModel();
            string host = string.Empty, callBackUrl = string.Empty, subDomainName = string.Empty;
            host = HttpContext.Request.Url.Host;
            subDomainName = CommonManager.GetTenantConnection(SessionManager.LoginResponse.UserId, string.Empty)[0].TenantDomain;
            if (host.Contains("localhost"))
            {
                callBackUrl = string.Format("{0}.{1}/#/{2}", subDomainName, host, "subscriber/index");
            }
            else
            {
                callBackUrl = string.Format("{0}.{1}/#/{2}", subDomainName, ConfigurationManager.AppSettings["hostName"].ToString(), "subscriber/index");
            }
            UriBuilder baseUri = new UriBuilder("https://" + callBackUrl);
            objCompose.CallBackUrl = baseUri.Uri.ToString();
            objCompose.DefaultMsg = ConfigurationManager.AppSettings["WeeklyCircularMsg"].ToString();
            objCompose.SubscriberList = GetSubscribers(1, -1, string.Empty);
            string ret = RenderPartialToString("~/Areas/Admin/Views/WeeklyCircular/_ComposeWeeklyCircularMessage.cshtml", objCompose, ControllerContext);
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public string SendWeeklyCircularInvite(string[] ids, string message)
        {
            var response = new WeeklyCircularBusiness().GetSubscriberPhoneNumber(ids, CommonManager.GetTenantConnection(SessionManager.LoginResponse.UserId, string.Empty)[0].TenantConnection);
            string host = string.Empty, callBackUrl = string.Empty, subDomainName = string.Empty;
            host = HttpContext.Request.Url.Host;
            subDomainName = CommonManager.GetTenantConnection(SessionManager.LoginResponse.UserId, string.Empty)[0].TenantDomain;
            if (host.Contains("localhost"))
            {
                callBackUrl = string.Format("{0}.{1}/#/{2}", subDomainName, host, "subscriber/index");
            }
            else
            {
                callBackUrl = string.Format("{0}.{1}/#/{2}", subDomainName, ConfigurationManager.AppSettings["hostName"].ToString(), "subscriber/index");
            }
            UriBuilder baseUri = new UriBuilder("https://" + callBackUrl);
            //var messageText =  ConfigurationManager.AppSettings["WeeklyCircularMsg"].ToString();          
            message = string.Format("{0} {1}", message, baseUri.Uri.ToString());
            var result = SMSManager.SendBulkSMSNotification(response, message, SessionManager.LoginResponse.UserId);
            if (result.ContainsKey("Error"))
            {
                return result["Error"].ToString();
            }
            else
            {
                return "1";
            }
        }

        public ActionResult MessageReport(int page = 1, string searchStr = "")
        {
            List<MessageResourcesViewModel> objList = _messageResourcesBusiness.GetMessagesLogs(SessionManager.LoginResponse.UserId, page, _pageSize, searchStr);
            ViewBag.Pager = new PagerHelper(objList.Select(c => c.TotalRows).FirstOrDefault(), page, _pageSize);
            ViewBag.SearchStr = searchStr;
            return View(objList);
        }

        #endregion
    }
}