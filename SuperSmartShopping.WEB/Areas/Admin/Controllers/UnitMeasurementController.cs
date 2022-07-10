using Newtonsoft.Json;
using SuperSmartShopping.DAL.Common;
using SuperSmartShopping.DAL.Models;
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
    public class UnitMeasurementController : BaseController
    {
        readonly string apiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"].ToString();
        public ActionResult Index()
        {
            return View(GetUnitMeasurement(0, true));
        }

        public ActionResult AddUpdate(int id)
        {
            UnitMeasurement obj = new UnitMeasurement();
            if (id > 0)
            {
                obj = GetUnitMeasurement(id, false).FirstOrDefault();
            }
            return View(obj);
        }

        [HttpPost]
        public int AddUpdate(UnitMeasurement model)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    model.CreatedBy = SessionManager.LoginResponse.UserId;
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + SessionManager.LoginResponse.AccessToken);
                    string url = apiBaseUrl + MethodEnum.AddUpdateUnitMeasurement.GetDescription().ToString();
                    var httpContent = CommonManager.CreateHttpContent(model);
                    HttpResponseMessage messge = client.PostAsync(url, httpContent).Result;
                    string result = messge.Content.ReadAsStringAsync().Result;
                    if (messge.IsSuccessStatusCode)
                    {
                        switch (Convert.ToInt32(JsonConvert.DeserializeObject<string>(result)))
                        {
                            case 0:
                                return 0;
                            case 1:
                                return model.Id > 0 ? 2 : 1;
                            case -2:
                                return -2;
                            default:
                                return -1;
                        }
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
    }
}