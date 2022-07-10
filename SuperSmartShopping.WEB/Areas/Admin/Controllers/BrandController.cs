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
    public class BrandController : BaseController
    {
        readonly string apiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"].ToString();
        readonly int _pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"].ToString());

        public ActionResult Index(int page = 1, string searchStr = "")
        {
            List<BrandModel> objList = GetBrands(0, page, _pageSize, searchStr);
            ViewBag.Pager = new PagerHelper(objList.Select(c => c.TotalRows).FirstOrDefault(), page, _pageSize);
            ViewBag.SearchStr = searchStr;
            return View(objList);
        }

        public ActionResult AddUpdate(int id)
        {
            BrandModel obj = new BrandModel();
            if (id > 0)
            {
                obj = GetBrands(id, 1, _pageSize, string.Empty).FirstOrDefault();
            }            
            return View(obj);
        }

        [HttpPost]
        public int AddUpdate(BrandModel model)
        {
            try
            {                
                using (var client = new HttpClient())
                {
                    model.CreatedBy = SessionManager.LoginResponse.UserId;
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + SessionManager.LoginResponse.AccessToken);
                    string url = apiBaseUrl + MethodEnum.AddUpdateBrand.GetDescription().ToString();
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
    }
}