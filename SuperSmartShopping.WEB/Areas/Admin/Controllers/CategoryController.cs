using Newtonsoft.Json;
using SuperSmartShopping.DAL.Common;
using SuperSmartShopping.DAL.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using static SuperSmartShopping.DAL.Enums.EnumHelper;

namespace SuperSmartShopping.WEB.Areas.Admin.Controllers
{
    [Compress]
    [AdminAuthorize]
    public class CategoryController : BaseController
    {
        readonly string apiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"].ToString();
        public ActionResult Index()
        {
            return View(GetCategories());
        }

        public ActionResult AddUpdate(int id)
        {
            ViewBag.Categories = new SelectList(GetCategories().Where(c => c.Id != id).ToList(), "Id", "Name");
            if (id > 0)
            {
                using (var client = new HttpClient())
                {
                    Category obj = null;
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + SessionManager.LoginResponse.AccessToken);
                    string url = apiBaseUrl + MethodEnum.GetCategoryById.GetDescription().ToString() + "?categoryId=" + id + "&userId=" + SessionManager.LoginResponse.UserId;
                    HttpResponseMessage messge = client.GetAsync(url).Result;
                    string result = messge.Content.ReadAsStringAsync().Result;
                    if (messge.IsSuccessStatusCode)
                    {
                        obj = JsonConvert.DeserializeObject<Category>(result);
                        return View(obj);
                    }
                }
            }
            return View(new Category());
        }

        [HttpPost]
        public int AddUpdate(FormCollection frm)
        {
            try
            {
                string base64StringFSSI = string.Empty, base64StringbannerImg = string.Empty;
                try
                {
                    base64StringFSSI = HttpContext.Request["UploadFile"] != string.Empty && HttpContext.Request["UploadFile"] != null ? HttpContext.Request["UploadFile"].Split(',')[1] : null;
                }
                catch (Exception ex)
                {
                    base64StringFSSI = null;
                }

                try
                {
                    base64StringbannerImg = HttpContext.Request["UploadBannerFile"] != string.Empty && HttpContext.Request["UploadBannerFile"] != null ? HttpContext.Request["UploadBannerFile"].Split(',')[1] : null;
                }
                catch (Exception ex)
                {
                    base64StringbannerImg = null;
                }


                string hdnFSSAIFilePath = base64StringFSSI != null ? base64StringFSSI : HttpContext.Request["hdnUploadFile"];
                string hdnBannerFilePath = base64StringbannerImg != null ? base64StringbannerImg : HttpContext.Request["hdnBannerUploadFile"];

                Category obj = new Category();
                obj.Id = Convert.ToInt32(frm["Id"].ToString());
                obj.Name = frm["Name"].ToString();
                obj.Description = frm["Description"].ToString();
                obj.ImagePath = hdnFSSAIFilePath;
                obj.BannerImg = hdnBannerFilePath;
                obj.ParentId = string.IsNullOrEmpty(frm["ParentId"].ToString()) ? 0 : Convert.ToInt32(frm["ParentId"].ToString());
                obj.PriorityIndex = Convert.ToInt32(frm["PriorityIndex"].ToString());
                obj.CreatedBy = SessionManager.LoginResponse.UserId;
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + SessionManager.LoginResponse.AccessToken);
                    string url = apiBaseUrl + MethodEnum.AddUpdateCategory.GetDescription().ToString();
                    var httpContent = CommonManager.CreateHttpContent(obj);
                    System.Net.Http.HttpResponseMessage messge = client.PostAsync(url, httpContent).Result;
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

        [HttpPost]
        public int MarkAsDeleted(int id)
        {
            if (id > 0)
            {
                using (var client = new HttpClient())
                {
                    int res = 0;
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + SessionManager.LoginResponse.AccessToken);
                    string url = apiBaseUrl + MethodEnum.MarkAsDeletedCategory.GetDescription().ToString() + "?categoryId=" + id + "&userId=" + SessionManager.LoginResponse.UserId;
                    HttpResponseMessage messge = client.PostAsync(url, null).Result;
                    string result = messge.Content.ReadAsStringAsync().Result;
                    if (messge.IsSuccessStatusCode)
                    {
                        res = JsonConvert.DeserializeObject<int>(result);
                        return res;
                    }
                }
            }
            return -1;
        }

        public List<Category> GetCategories()
        {
            using (var client = new HttpClient())
            {
                List<Category> objList = new List<Category>();
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + SessionManager.LoginResponse.AccessToken);
                string url = apiBaseUrl + MethodEnum.GetCategories.GetDescription().ToString() + "?type='All'&userId=" + SessionManager.LoginResponse.UserId;
                HttpResponseMessage messge = client.GetAsync(url).Result;
                string result = messge.Content.ReadAsStringAsync().Result;
                if (messge.IsSuccessStatusCode)
                {
                    objList = JsonConvert.DeserializeObject<List<Category>>(result);
                }
                return objList;
            }
        }
    }
}