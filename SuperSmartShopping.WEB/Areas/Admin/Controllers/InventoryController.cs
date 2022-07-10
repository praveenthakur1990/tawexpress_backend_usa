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
    [Compress]
    [AdminAuthorize]
    public class InventoryController : BaseController
    {
        readonly string apiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"].ToString();
        readonly int _pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"].ToString());
        public ActionResult Index(int id = 0, int page = 1, string searchStr = "")
        {
            if (id == 0)
            {
                return RedirectToAction("Index", "Product", new { area = "admin" });
            }
            ViewBag.ProductId = id.ToString();
            List<StockModel> objList = GetInventoryByProductId(id, page, _pageSize, searchStr);
            ViewBag.Pager = new PagerHelper(objList.Select(c => c.TotalRows).FirstOrDefault(), page, _pageSize);
            return View(objList);
        }
        public ActionResult AddStock(int id)
        {
            StockModel model = new StockModel();
            model.ProductInfo = GetProducts(id, 1, _pageSize, string.Empty).FirstOrDefault();
            return View(model);
        }

        [HttpPost]
        public int AddStock(StockModel model)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    model.AddedBy = SessionManager.LoginResponse.UserId;
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + SessionManager.LoginResponse.AccessToken);
                    string url = apiBaseUrl + MethodEnum.AddStock.GetDescription().ToString();
                    var httpContent = CommonManager.CreateHttpContent(model);
                    HttpResponseMessage messge = client.PostAsync(url, httpContent).Result;
                    string result = messge.Content.ReadAsStringAsync().Result;
                    if (messge.IsSuccessStatusCode)
                    {
                        return model.Id > 0 ? 2 : 1;
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

        public List<StockModel> GetInventoryByProductId(int productId, int pageNumber, int pageSize, string searchStr)
        {
            using (var client = new HttpClient())
            {
                List<StockModel> objList = new List<StockModel>();
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + SessionManager.LoginResponse.AccessToken);
                string url = apiBaseUrl + MethodEnum.GetInventoryByProductId.GetDescription().ToString() + "?productId=" + productId + "&userId=" + SessionManager.LoginResponse.UserId + "&pageNumber=" + pageNumber + "&pageSize=" + pageSize + "&searchStr=" + searchStr;
                HttpResponseMessage messge = client.GetAsync(url).Result;
                string result = messge.Content.ReadAsStringAsync().Result;
                if (messge.IsSuccessStatusCode)
                {
                    objList = JsonConvert.DeserializeObject<List<StockModel>>(result);
                }
                return objList;
            }
        }

    }
}