using Newtonsoft.Json;
using SuperSmartShopping.BAL.Interfaces;
using SuperSmartShopping.DAL.Common;
using SuperSmartShopping.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using static SuperSmartShopping.DAL.Enums.EnumHelper;

namespace SuperSmartShopping.API.Controllers
{
    [Authorize]
    [RoutePrefix("api/Rider")]
    public class RiderController : ApiController
    {
        private IRiderBusiness _riderBusiness;
        public RiderController(IRiderBusiness riderBusiness)
        {
            _riderBusiness = riderBusiness;
        }

        [Route("GetStores")]
        [HttpGet]
        public HttpResponseMessage GetStores()
        {
            List<StoreListModel> objList = _riderBusiness.GeStoreNames();
            return Request.CreateResponse(HttpStatusCode.OK, objList);
        }

        [Route("AddUpdateRider")]
        [HttpPost]
        public HttpResponseMessage AddUpdateRider(RiderModel obj)
        {
            try
            {
                string randomPassword = CommonManager.CreateRandomPassword(6);
                var kvpList = new List<KeyValuePair<string, string>>
            {
            new KeyValuePair<string, string>("Email", obj.EmailAddress),
            new KeyValuePair<string, string>("Password", randomPassword),
            new KeyValuePair<string, string>("ConfirmPassword", randomPassword),
            new KeyValuePair<string, string>("RoleName", RolesEnum.Rider.ToString()),
            new KeyValuePair<string, string>("CreatedBy",obj.CreatedBy),
            new KeyValuePair<string, string>("MobileNumber",CommonManager.RemoveSpecialCharacters(obj.Mobile)),
            new KeyValuePair<string, string>("FirstName",obj.FirstName),
            new KeyValuePair<string, string>("LastName",obj.LastName)

            };
                FormUrlEncodedContent rqstBody = new FormUrlEncodedContent(kvpList);
                using (var client = new HttpClient())
                {
                    HttpResponseMessage messge = null;
                    string result = string.Empty;
                    string url = ConfigurationManager.AppSettings["ApiBaseUrl"].ToString() + MethodEnum.Register.GetDescription().ToString();
                    if (obj.Id == 0)
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
                        if (obj.Id == 0)
                        {
                            string message = "Welcome To TaxExpress!! \nBelow are login credentials: \nUsername:{0}  \nPassword:{1} \n";
                            SMSManager.SendSMSNotification(obj.Mobile, message.Replace("{0}", CommonManager.RemoveSpecialCharacters(obj.Mobile)).Replace("{1}", randomPassword));
                        }
                        _riderBusiness.AddUpdate(obj);
                        return Request.CreateResponse(HttpStatusCode.OK, "success");
                    }
                    else
                    {
                        try
                        {
                            Errorresponse response = JsonConvert.DeserializeObject<Errorresponse>(result);
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, response == null ? "An error occured while registering the store, please try again later." : response.error_description);
                        }
                        catch (Exception ex)
                        {
                            string response = JsonConvert.DeserializeObject<string>(result);
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, response == null ? "An error occured while registering the store, please try again later." : response);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, obj);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message.ToString());
            }
        }

        [Route("GetRiders")]
        [HttpGet]
        public HttpResponseMessage GetRiders(int id, int pageNumber, int pageSize, string searchStr)
        {
            PaginationModel objPagination = new PaginationModel();
            objPagination.PageNumber = pageNumber == 0 ? 1 : pageNumber;
            objPagination.PageSize = pageSize;
            objPagination.SearchStr = searchStr == null ? string.Empty : searchStr;
            List<RiderModel> objList = _riderBusiness.GetRiders(id, objPagination);
            return Request.CreateResponse(HttpStatusCode.OK, objList);
        }

        [Route("GetStoreLinkedRiders")]
        [HttpGet]
        public HttpResponseMessage GetStoreLinkedRiders(string storeUserId)
        {
            List<RiderModel> objList = _riderBusiness.GetStoreLinkedRiders(storeUserId);
            return Request.CreateResponse(HttpStatusCode.OK, objList);
        }


        [Route("GetRiderOrders")]
        [HttpPost]
        public HttpResponseMessage GetRiderOrders(string usename, PaginationModel model, string type = "")
        {
            if (model == null)
            {
                model = new PaginationModel();
                model.SearchStr = string.Empty;
            }
            List<OrderInfoModel> objOrderList = _riderBusiness.GetRiderOrders(usename, type, model);
            return Request.CreateResponse(HttpStatusCode.OK, objOrderList);
        }

        [Route("UpdateRiderOrderStatus")]
        [HttpPost]
        public HttpResponseMessage UpdateRiderOrderStatus(string userName, int orderId, string storeUserId)
        {
            int res = _riderBusiness.UpdateRiderOrderStatus(userName, orderId, storeUserId);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }
    }
}
