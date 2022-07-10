using Newtonsoft.Json;
using SuperSmartShopping.DAL.Common;
using SuperSmartShopping.DAL.Models;
using SuperSmartShopping.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using static SuperSmartShopping.DAL.Enums.EnumHelper;

namespace SuperSmartShopping.WEB.Areas.Admin.Controllers
{
    [Compress]
    [AdminAuthorize]
    public class ProductController : BaseController
    {
        readonly string apiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"].ToString();
        readonly int _pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"].ToString());
        public ActionResult Index(int page = 1, string searchStr = "")
        {
            List<ProductModel> objList = GetProducts(0, page, _pageSize, searchStr);
            ViewBag.Pager = new PagerHelper(objList.Select(c => c.TotalRows).FirstOrDefault(), page, _pageSize);
            ViewBag.SearchStr = searchStr;
            return View(objList);
        }

        public ActionResult AddUpdate(int id)
        {
            ViewBag.Categories = new SelectList(GetCategories().Where(c => c.ParentId == 0 && c.IsActive == true && c.IsDeleted == false).ToList(), "Id", "Name");
            ViewBag.Brands = new SelectList(GetBrands(0, 1, -1, string.Empty), "Id", "Name");
            ViewBag.UnitMeasurement = new SelectList(GetUnitMeasurement(0, false), "Id", "Name");
            ViewBag.Tags = new SelectList(GetTagsList(id), "Id", "Name");
            ProductModel obj = new ProductModel();
            if (id > 0)
            {
                obj = GetProducts(id, 1, _pageSize, string.Empty).FirstOrDefault();
                ViewBag.SubCategories = new SelectList(GetCategories().Where(c => c.ParentId > 0 && c.ParentId == obj.CategoryId && c.IsActive == true && c.IsDeleted == false).ToList(), "Id", "Name");
            }
            else
            {
                obj.IsPublished = true;
                obj.IsVariants = false;
                obj.MarkItemAs = "New";
                obj.VegNonVeg = "Veg";
            }
            return View(obj);
        }

        [HttpPost]
        public int AddUpdate(FormCollection frm)
        {
            try
            {
                Product obj = new Product();
                obj.Id = Convert.ToInt32(frm["Id"].ToString());
                obj.CategoryId = Convert.ToInt32(frm["CategoryId"].ToString());
                obj.SubCategoryId = Convert.ToInt32(frm["SubCategoryId"].ToString());
                obj.BrandId = !string.IsNullOrEmpty(frm["BrandId"]) ? Convert.ToInt32(frm["BrandId"].ToString()) : 0;
                obj.UnitMeasurementId = !string.IsNullOrEmpty(frm["UnitMeasurementId"]) ? Convert.ToInt32(frm["UnitMeasurementId"].ToString()) : 0;
                obj.Name = frm["Name"].ToString();
                obj.IsVariants = Convert.ToBoolean(frm["IsVariants"].ToString());
                obj.IsDescriptionShow = Convert.ToBoolean(frm["IsDescriptionShow"].ToString());
                obj.Price = obj.IsVariants == true ? 0 : Convert.ToDecimal(frm["Price"].ToString());
                if (!string.IsNullOrEmpty(frm["ImagePath"].ToString()))
                {
                    if (frm["ImagePath"].ToString().Split(',').Length == 1)
                    {
                        if (frm["ImagePath"].ToString().IndexOf("/Upload") >= 0)
                        {
                            obj.Image = frm["ImagePath"].ToString().Substring(frm["ImagePath"].ToString().IndexOf("/Upload"));
                        }
                    }
                    else
                    {
                        if (CommonManager.IsBase64(frm["ImagePath"].ToString().Split(',')[1].ToString()) == true)
                        {
                            obj.Image = frm["ImagePath"].ToString().Split(',')[1].ToString();
                        }
                        else
                        {
                            obj.Image = frm["ImagePath"].ToString().Substring(frm["ImagePath"].ToString().IndexOf("/Upload"));
                        }
                    }
                }
                obj.Description = frm["Description"].ToString();
                obj.MarkItemAs = frm["MarkItemAs"].ToString();
                obj.VegNonVeg = frm["VegNonVeg"].ToString();
                obj.IsPublished = Convert.ToBoolean(frm["IsPublish"].ToString());
                obj.TagIds = !string.IsNullOrEmpty(frm["Tags"]) ? frm["Tags"].ToString() : string.Empty;
                obj.CreatedBy = SessionManager.LoginResponse.UserId;
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + SessionManager.LoginResponse.AccessToken);
                    string url = apiBaseUrl + MethodEnum.AddUpdate.GetDescription().ToString();
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
                return -1;
            }
        }

        public List<ProductModel> GetProducts(int productId, int pageNumber, int pageSize, string searchStr)
        {
            using (var client = new HttpClient())
            {
                List<ProductModel> objList = new List<ProductModel>();
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + SessionManager.LoginResponse.AccessToken);
                string url = apiBaseUrl + MethodEnum.GetProducts.GetDescription().ToString() + "?productId=" + productId + "&userId=" + SessionManager.LoginResponse.UserId + "&pageNumber=" + pageNumber + "&pageSize=" + pageSize + "&searchStr=" + searchStr;
                HttpResponseMessage messge = client.GetAsync(url).Result;
                string result = messge.Content.ReadAsStringAsync().Result;
                if (messge.IsSuccessStatusCode)
                {
                    objList = JsonConvert.DeserializeObject<List<ProductModel>>(result);
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

        [HttpPost]
        public JsonResult GetSubCategories(int id)
        {
            var result = new SelectList(GetCategories().Where(c => c.ParentId == id).ToList(), "Id", "Name");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public List<BrandModel> GetBrands(int brandId, int pageNumber, int pageSize, string searchStr)
        {
            using (var client = new HttpClient())
            {
                List<BrandModel> objList = new List<BrandModel>();
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + SessionManager.LoginResponse.AccessToken);
                string url = apiBaseUrl + MethodEnum.GetBrands.GetDescription().ToString() + "?brandId=" + brandId + "&userId=" + SessionManager.LoginResponse.UserId + "&pageNumber=" + pageNumber + "&pageSize=" + pageSize + "&searchStr=" + searchStr;
                HttpResponseMessage messge = client.GetAsync(url).Result;
                string result = messge.Content.ReadAsStringAsync().Result;
                if (messge.IsSuccessStatusCode)
                {
                    objList = JsonConvert.DeserializeObject<List<BrandModel>>(result);
                }
                return objList;
            }
        }

        public List<UnitMeasurement> GetUnitMeasurement(int id, bool isShowInAdmin)
        {
            using (var client = new HttpClient())
            {
                List<UnitMeasurement> objList = new List<UnitMeasurement>();
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + SessionManager.LoginResponse.AccessToken);
                string url = apiBaseUrl + MethodEnum.GetUnitMeasurement.GetDescription().ToString() + "?id=" + id + "&isShowInAdmin=" + isShowInAdmin + "&userId=" + SessionManager.LoginResponse.UserId;
                HttpResponseMessage messge = client.GetAsync(url).Result;
                string result = messge.Content.ReadAsStringAsync().Result;
                if (messge.IsSuccessStatusCode)
                {
                    objList = JsonConvert.DeserializeObject<List<UnitMeasurement>>(result);
                }
                return objList;
            }
        }
        public List<ProductModel> GetTagsList(int id)
        {
            using (var client = new HttpClient())
            {
                List<ProductModel> objList = new List<ProductModel>();
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + SessionManager.LoginResponse.AccessToken);
                string url = apiBaseUrl + MethodEnum.GetTagsList.GetDescription().ToString() + "?productId=" + id + "&userId=" + SessionManager.LoginResponse.UserId;
                HttpResponseMessage messge = client.GetAsync(url).Result;
                string result = messge.Content.ReadAsStringAsync().Result;
                if (messge.IsSuccessStatusCode)
                {
                    objList = JsonConvert.DeserializeObject<List<ProductModel>>(result);
                }
                return objList.OrderBy(c => c.Name).ToList();
            }
        }
    }
}