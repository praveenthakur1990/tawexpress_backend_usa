using Newtonsoft.Json;
using SuperSmartShopping.DAL.Common;
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
    public class ProductVarientController : BaseController
    {
        readonly string apiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"].ToString();
        readonly int _pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"].ToString());
        public ActionResult Index(int page = 1, string searchStr = "")
        {
            List<ProductVarientsModel> objList = GetProductsVarients(0, 0, page, _pageSize, searchStr);
            ViewBag.Pager = new PagerHelper(objList.Select(c => c.TotalRows).FirstOrDefault(), page, _pageSize);
            ViewBag.SearchStr = searchStr;
            return View(objList);
        }

        public ActionResult AddUpdate(int id)
        {
            ViewBag.VarientProducts = new SelectList(GetProducts(0, 1, -1, string.Empty).Where(c => c.IsVariants == true).ToList(), "Id", "Name");
            ProductVarientsModel obj = new ProductVarientsModel();
            if (id > 0)
            {
                obj = GetProductsVarients(id, 0, 1, _pageSize, string.Empty).FirstOrDefault();
            }
            else
            {
                obj.IsPublished = true;
            }
            return View(obj);
        }

        [HttpPost]
        public int AddUpdate(FormCollection frm)
        {
            try
            {
                ProductVarientsModel obj = new ProductVarientsModel();
                obj.Id = Convert.ToInt32(frm["Id"].ToString());
                obj.ProductId = Convert.ToInt32(frm["ProductId"].ToString());
                obj.Name = frm["Name"].ToString();
                obj.Price = Convert.ToDecimal(frm["Price"].ToString());
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
                obj.IsPublished = Convert.ToBoolean(frm["IsPublish"].ToString());
                obj.CreatedBy = SessionManager.LoginResponse.UserId;
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + SessionManager.LoginResponse.AccessToken);
                    string url = apiBaseUrl + MethodEnum.AddUpdateProductVarient.GetDescription().ToString();
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

        public List<ProductVarientsModel> GetProductsVarients(int id, int productId, int pageNumber, int pageSize, string searchStr)
        {
            using (var client = new HttpClient())
            {
                List<ProductVarientsModel> objList = new List<ProductVarientsModel>();
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + SessionManager.LoginResponse.AccessToken);
                string url = apiBaseUrl + MethodEnum.GetProductsVarients.GetDescription().ToString() + "?id=" + id + "&productId=" + productId + "&userId=" + SessionManager.LoginResponse.UserId + "&pageNumber=" + pageNumber + "&pageSize=" + pageSize + "&searchStr=" + searchStr;
                HttpResponseMessage messge = client.GetAsync(url).Result;
                string result = messge.Content.ReadAsStringAsync().Result;
                if (messge.IsSuccessStatusCode)
                {
                    objList = JsonConvert.DeserializeObject<List<ProductVarientsModel>>(result);
                }
                return objList;
            }
        }
    }
}