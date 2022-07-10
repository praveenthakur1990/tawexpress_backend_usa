using Newtonsoft.Json;
using SuperSmartShopping.DAL.Common;
using SuperSmartShopping.DAL.Models;
using SuperSmartShopping.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
    public class RiderController : Controller
    {
        readonly string apiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"].ToString();
        readonly int _pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"].ToString());
        public ActionResult Index(int page = 1, string searchStr = "")
        {           
            List<RiderModel> objList = GetRiderList(0, page, _pageSize, searchStr);
            ViewBag.Pager = new PagerHelper(objList.Select(c => c.TotalRows).FirstOrDefault(), page, _pageSize);
            ViewBag.SearchStr = searchStr;
            return View(objList);
        }
        public ActionResult AddUpdateRider(int id)
        {
            //ViewBag.Stores = GetStoreList();
            RiderModel obj = null;
            ViewBag.Stores = new SelectList(GetStoreList(), "UserId", "DomainName");
            if(id > 0)
            {
                obj = GetRiderList(id, 1, 1, string.Empty).FirstOrDefault();
            }
            else
            {
                obj = new RiderModel();
            }
            
            return View(obj);
        }

        [HttpPost]
        public string AddUpdateRider(FormCollection frm)
        {
            try
            {
                RiderModel obj = new RiderModel();
                obj.Id = Convert.ToInt32(frm["Id"].ToString());
                obj.FirstName = frm["firstName"].ToString();
                obj.LastName = frm["lastName"].ToString();
                obj.Gender = frm["gender"].ToString();
                obj.Mobile = frm["mobile"].ToString();
                obj.EmailAddress = frm["emailAddress"].ToString();
                obj.ContactAddress = frm["contactAddress"].ToString();
                obj.State = frm["state"].ToString();
                obj.City = frm["City"].ToString();
                obj.ZipCode = frm["ZipCode"].ToString();
                obj.CreatedBy = SessionManager.LoginResponse.UserId;
                obj.StoreIds = frm["storeIds"] != string.Empty && frm["storeIds"] != null ? frm["storeIds"].ToString() : string.Empty;

                if (!string.IsNullOrEmpty(obj.StoreIds))
                {
                    RiderStoreLinkingModel objRiderStoreIds = null;
                    var storeIds = obj.StoreIds.Split(',');
                    obj.StoreIdList = new List<RiderStoreLinkingModel>();
                    foreach (var item in storeIds)
                    {
                        objRiderStoreIds = new RiderStoreLinkingModel();
                        objRiderStoreIds.StoreId = item;
                        obj.StoreIdList.Add(objRiderStoreIds);
                    }
                    DataTable table = CommonManager.ToDataTable(obj.StoreIdList);
                    obj.StoreIdTable = table;
                }
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + SessionManager.LoginResponse.AccessToken);
                    string url = apiBaseUrl + MethodEnum.AddUpdateRider.GetDescription().ToString();
                    var httpContent = CommonManager.CreateHttpContent(obj);
                    HttpResponseMessage messge = client.PostAsync(url, httpContent).Result;
                    string result = messge.Content.ReadAsStringAsync().Result;
                    if (messge.IsSuccessStatusCode)
                    {
                        return obj.Id.ToString();
                    }
                    else
                    {
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, frm);
                return ex.Message.ToString();
            }

        }
        public List<StoreListModel> GetStoreList()
        {
            using (var client = new HttpClient())
            {
                List<StoreListModel> objList = new List<StoreListModel>();
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + SessionManager.LoginResponse.AccessToken);
                string url = apiBaseUrl + MethodEnum.GetStores.GetDescription().ToString();
                HttpResponseMessage messge = client.GetAsync(url).Result;
                string result = messge.Content.ReadAsStringAsync().Result;
                if (messge.IsSuccessStatusCode)
                {
                    objList = JsonConvert.DeserializeObject<List<StoreListModel>>(result);
                }
                return objList;
            }
        }

        public List<RiderModel> GetRiderList(int id, int pageNumber, int pageSize, string searchStr)
        {
            using (var client = new HttpClient())
            {
                List<RiderModel> objList = new List<RiderModel>();
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + SessionManager.LoginResponse.AccessToken);
                string url = apiBaseUrl + MethodEnum.GetRiders.GetDescription().ToString() + "?Id=" + id + "&pageNumber=" + pageNumber + "&pageSize=" + pageSize + "&searchStr=" + searchStr;
                HttpResponseMessage messge = client.GetAsync(url).Result;
                string result = messge.Content.ReadAsStringAsync().Result;
                if (messge.IsSuccessStatusCode)
                {
                    objList = JsonConvert.DeserializeObject<List<RiderModel>>(result);
                }
                return objList;
            }
        }
    }
}