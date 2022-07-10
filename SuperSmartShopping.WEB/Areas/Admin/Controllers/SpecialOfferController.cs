using Newtonsoft.Json;
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
    public class SpecialOfferController : BaseController
    {
        readonly string apiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"].ToString();
        readonly int _pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"].ToString());

        #region "Add Special Offer"
        public ActionResult Index(int page = 1, string searchStr = "")
        {
            List<SpecialOfferModel> objList = GetSpecialOffer(0, page, _pageSize, searchStr);
            ViewBag.Pager = new PagerHelper(objList.Select(c => c.TotalRows).FirstOrDefault(), page, _pageSize);
            ViewBag.SearchStr = searchStr;
            return View(objList);
        }
        public ActionResult AddUpdate(int id)
        {
            SpecialOfferModel obj = new SpecialOfferModel();
            if (id > 0)
            {
                obj = GetSpecialOffer(id, 1, _pageSize, string.Empty).FirstOrDefault();
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
                    base64StringFSSI = HttpContext.Request["BannerImagePath"] != string.Empty && HttpContext.Request["BannerImagePath"] != null ? HttpContext.Request["BannerImagePath"].Split(',')[1] : null;
                }
                catch (Exception ex)
                {
                    base64StringFSSI = null;
                }

                hdnFSSAIFilePath = base64StringFSSI != null ? base64StringFSSI : HttpContext.Request["hdnBannerImagePath"];         

                SpecialOfferModel obj = new SpecialOfferModel();
                obj.Id = Convert.ToInt32(frm["Id"].ToString());
                obj.Title = frm["Title"].ToString();
                obj.StartDate = Convert.ToDateTime(frm["StartDate"].ToString());
                obj.EndDate = Convert.ToDateTime(frm["EndDate"].ToString());
                obj.BannerImagePath = hdnFSSAIFilePath;
                obj.CreatedBy = SessionManager.LoginResponse.UserId;
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + SessionManager.LoginResponse.AccessToken);
                    string url = apiBaseUrl + MethodEnum.AddUpdateSpecialOffer.GetDescription().ToString();
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
        public List<SpecialOfferModel> GetSpecialOffer(int id, int pageNumber, int pageSize, string searchStr)
        {
            using (var client = new HttpClient())
            {
                List<SpecialOfferModel> objList = new List<SpecialOfferModel>();
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + SessionManager.LoginResponse.AccessToken);
                string url = apiBaseUrl + MethodEnum.GetSpecialOffer.GetDescription().ToString() + "?id=" + id + "&userId=" + SessionManager.LoginResponse.UserId + "&pageNumber=" + pageNumber + "&pageSize=" + pageSize + "&searchStr=" + searchStr;
                HttpResponseMessage messge = client.GetAsync(url).Result;

                string result = messge.Content.ReadAsStringAsync().Result;
                if (messge.IsSuccessStatusCode)
                {
                    objList = JsonConvert.DeserializeObject<List<SpecialOfferModel>>(result);
                }
                else
                {
                    //if(messge.StatusCode==403)
                }
                return objList;
            }
        }
        #endregion

        #region "Add Sepcial Offer Products"
        public ActionResult ViewProduct(int id)
        {
            SpecialOfferModel obj = GetSpecialOffer(id, 1, _pageSize, string.Empty).FirstOrDefault();
            obj.ProductList = GetProductsBySpecialOfferId(0, id);
            if (obj == null)
            {
                return RedirectToAction("Index");
            }
            return View(obj);
        }
        public List<ProductDashboardModel> GetProductsBySpecialOfferId(int categoryId, int specialOfferId = 0)
        {
            using (var client = new HttpClient())
            {
                List<ProductDashboardModel> objList = new List<ProductDashboardModel>();
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + SessionManager.LoginResponse.AccessToken);
                string url = apiBaseUrl + MethodEnum.GetProductBySpecialOfferId.GetDescription().ToString() + "?categoryId=" + (categoryId > 0 ? categoryId.ToString() : string.Empty) + "&userId=" + SessionManager.LoginResponse.UserId + "&specialOfferId=" + specialOfferId;
                HttpResponseMessage messge = client.GetAsync(url).Result;
                string result = messge.Content.ReadAsStringAsync().Result;
                if (messge.IsSuccessStatusCode)
                {
                    objList = JsonConvert.DeserializeObject<List<ProductDashboardModel>>(result);
                }
                return objList;
            }
        }
        [HttpPost]
        public JsonResult GetAddUpdateProduct(int specialOfferId, int catId = 0, int subCatId = 0, int specialOfferCatId = 0)
        {
            WeeklyCircularCatInfoModel obj = new WeeklyCircularCatInfoModel();
            obj.WeeklyCircularCatId = specialOfferCatId;
            obj.WeeklyCircularId = specialOfferId;
            obj.CategoryId = catId;
            obj.SubCategoryId = subCatId;
            obj.Categories = GetCategories().Where(c => c.ParentId == 0 && c.IsActive == true && c.IsDeleted == false).ToList();
            if (obj.CategoryId > 0)
            {
                obj.SubCategories = GetCategories().Where(c => c.ParentId == obj.CategoryId && c.IsActive == true && c.IsDeleted == false).ToList();
            }
            string ret = RenderPartialToString("~/Areas/Admin/Views/SpecialOffer/_AddUpdateProduct.cshtml", obj, ControllerContext);
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
                    string url = apiBaseUrl + MethodEnum.AddUpdateSpecialOfferProduct.GetDescription().ToString();
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
        public JsonResult GetProductsBySubCategories(int catId, int subCatId, int weeklyCircularId)
        {
            var result = GetProducts(catId, subCatId).ToList();
            var weeklyCircularProduct = GetProductsBySpecialOfferId(catId, weeklyCircularId);
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

            string ret = RenderPartialToString("~/Areas/Admin/Views/SpecialOffer/_LoadProductsBySubCat.cshtml", finalResult, ControllerContext);
            return Json(ret, JsonRequestBehavior.AllowGet);
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