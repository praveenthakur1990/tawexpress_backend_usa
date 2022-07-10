using Newtonsoft.Json;
using SuperSmartShopping.DAL.Common;
using SuperSmartShopping.DAL.ViewModels;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Web.Mvc;
using static SuperSmartShopping.DAL.Enums.EnumHelper;

namespace SuperSmartShopping.WEB.Areas.SecurePanel.Controllers
{
    [Compress]
    [SuperAdminAuthorize]
    public class PartnerController : Controller
    {
        readonly string apiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"].ToString();
        public ActionResult Index()
        {
            return View(GetPartnerList());
        }
        public ActionResult AddUpdatePartner(int id)
        {
            if (id > 0)
            {
                using (var client = new HttpClient())
                {
                    PartnerModel obj = null;
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + SessionManager.LoginResponse.AccessToken);
                    string url = apiBaseUrl + MethodEnum.GetPartnerById.GetDescription().ToString() + "?partnerId=" + id;
                    HttpResponseMessage messge = client.GetAsync(url).Result;
                    string result = messge.Content.ReadAsStringAsync().Result;
                    if (messge.IsSuccessStatusCode)
                    {
                        obj = JsonConvert.DeserializeObject<PartnerModel>(result);
                        return View(obj);
                    }
                }
            }
            return View(new PartnerModel());
        }

        [HttpPost]
        public int AddUpdatePartner(PartnerModel model)
        {
            model.CreatedBy = SessionManager.LoginResponse.UserId;
            using (var client = new System.Net.Http.HttpClient())
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + SessionManager.LoginResponse.AccessToken);
                string url = apiBaseUrl + MethodEnum.AddUpdatePartner.GetDescription().ToString();
                var httpContent = CommonManager.CreateHttpContent(model);
                HttpResponseMessage messge = client.PostAsync(url, httpContent).Result;
                string result = messge.Content.ReadAsStringAsync().Result;
                if (messge.IsSuccessStatusCode)
                {
                    return model.PartnerID > 0 ? 2 : 1;
                }
                else
                {
                    return -1;
                }
            }
        }
        public List<PartnerModel> GetPartnerList()
        {
            using (var client = new HttpClient())
            {
                List<PartnerModel> objList = new List<PartnerModel>();
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + SessionManager.LoginResponse.AccessToken);
                string url = apiBaseUrl + MethodEnum.GetPartnerList.GetDescription().ToString();
                HttpResponseMessage messge = client.GetAsync(url).Result;
                string result = messge.Content.ReadAsStringAsync().Result;
                if (messge.IsSuccessStatusCode)
                {
                    objList = JsonConvert.DeserializeObject<List<PartnerModel>>(result);
                }
                return objList;
            }
        }
    }
}