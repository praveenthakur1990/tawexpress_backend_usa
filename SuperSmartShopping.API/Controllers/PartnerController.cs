using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using SuperSmartShopping.BAL.Interfaces;
using SuperSmartShopping.DAL.Common;
using SuperSmartShopping.DAL.Enums;
using SuperSmartShopping.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using static SuperSmartShopping.DAL.Enums.EnumHelper;

namespace SuperSmartShopping.API.Controllers
{
    [Authorize]
   // [MyAuthorize(Roles = "Admin,Rider")]
    [RoutePrefix("api/Partner")]
    public class PartnerController : ApiController
    {
        private IPartnerBusiness _partnerBusiness;
        public PartnerController(IPartnerBusiness partnerBusiness)
        {
            _partnerBusiness = partnerBusiness;
        }

        [Route("AddUpdatePartner")]
        [HttpPost]
        public HttpResponseMessage AddUpdatePartner(PartnerModel model)
        {
            try
            {
                var kvpList = new List<KeyValuePair<string, string>>
            {
            new KeyValuePair<string, string>("Email", model.EmailAddress),
            new KeyValuePair<string, string>("Password", "@Password2"),
            new KeyValuePair<string, string>("ConfirmPassword", "@Password2"),
              new KeyValuePair<string, string>("RoleName", EnumHelper.RolesEnum.Partner.ToString()),
                new KeyValuePair<string, string>("CreatedBy",model.CreatedBy)
            };
                FormUrlEncodedContent rqstBody = new FormUrlEncodedContent(kvpList);
                using (var client = new HttpClient())
                {
                    HttpResponseMessage messge = null;
                    string result = string.Empty;
                    string url = ConfigurationManager.AppSettings["ApiBaseUrl"].ToString() + MethodEnum.Register.GetDescription().ToString();
                    if (model.PartnerID == 0)
                    {
                        messge = client.PostAsync(url, rqstBody).Result;
                        result = messge.Content.ReadAsStringAsync().Result;
                    }
                    else
                    {
                        messge = new HttpResponseMessage();
                        messge = Request.CreateResponse(HttpStatusCode.OK);
                    }

                    if (messge.IsSuccessStatusCode)
                    {
                        if (_partnerBusiness.AddUpdatePartner(model) > 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, "success");
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, "-1");
                        }
                    }
                    else
                    {
                        Errorresponse response = JsonConvert.DeserializeObject<Errorresponse>(result);
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, response.error_description);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, model);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message.ToString());
            }
        }

        [Route("GetPartnerList")]
        [HttpGet]
        public HttpResponseMessage GetPartnerList()
        {
            List<PartnerModel> objList = _partnerBusiness.GetAllPartners(0);
            return Request.CreateResponse(HttpStatusCode.OK, objList);
        }

        [Route("GetPartnerById")]
        [HttpGet]
        public HttpResponseMessage GetPartnerById(int partnerId)
        {
            PartnerModel obj = _partnerBusiness.GetAllPartners(partnerId).FirstOrDefault();
            return Request.CreateResponse(HttpStatusCode.OK, obj);
        }

    }
}
