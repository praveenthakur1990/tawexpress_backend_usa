using SuperSmartShopping.BAL.Interfaces;
using SuperSmartShopping.DAL.Common;
using SuperSmartShopping.DAL.Models;
using SuperSmartShopping.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;

namespace SuperSmartShopping.API.Controllers
{
    [Authorize]
    [RoutePrefix("api/DeliveryAddress")]
    public class DeliveryAddressController : ApiController
    {
        private IDeliveryAddressBusiness _deliveryAddressBusiness;
        public DeliveryAddressController(IDeliveryAddressBusiness deliveryAddressBusiness)
        {
            _deliveryAddressBusiness = deliveryAddressBusiness;
        }
        [Route("AddUpdate")]
        [HttpPost]
        public HttpResponseMessage AddUpdate([FromBody] DeliveryAddressesModel obj)
        {
            try
            {
                int res = _deliveryAddressBusiness.AddUpdate(obj, CommonManager.GetTenantConnection(obj.StoreId, string.Empty)[0].TenantConnection);
                if (res > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }

            }
            catch (Exception ex)
            {
                CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, obj);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message.ToString());
            }
        }

        [Route("Get")]
        [HttpGet]
        public HttpResponseMessage Get(int id, string userId, string createdBy)
        {
            List<DeliveryAddressesModel> objList = new List<DeliveryAddressesModel>();
            objList = _deliveryAddressBusiness.Get(id, userId, CommonManager.GetTenantConnection(createdBy, string.Empty)[0].TenantConnection).ToList();
            if (id > 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, objList.FirstOrDefault());
            }
            return Request.CreateResponse(HttpStatusCode.OK, objList);
        }

        [Route("GetDeliverySlotDays")]
        [HttpGet]
        public HttpResponseMessage GetDeliverySlotDays(int days, string createdBy)
        {
            List<DateTime> objList = new List<DateTime>();
            objList = _deliveryAddressBusiness.GetDeliverySlotDays(days, CommonManager.GetTenantConnection(createdBy, string.Empty)[0].TenantConnection).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, objList);
        }

        [Route("GetDeliverySlotTimes")]
        [HttpGet]
        public HttpResponseMessage GetDeliverySlotTimes(int interval, int start, int end, string createdBy)
        {
            List<DeliverySlotTime> objList = new List<DeliverySlotTime>();
            objList = _deliveryAddressBusiness.GetDeliverySlotTime(interval, start, end, CommonManager.GetTenantConnection(createdBy, string.Empty)[0].TenantConnection).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, objList);
        }

        [Route("CalculateDistance")]
        [HttpGet]
        public HttpResponseMessage CalculateDistance(int deliveryAddressId, string userId)
        {
            decimal miles = _deliveryAddressBusiness.CalculateDeliveryAddressDistance(deliveryAddressId, CommonManager.GetTenantConnection(userId, string.Empty)[0].TenantConnection);
            return Request.CreateResponse(HttpStatusCode.OK, miles);
        }
    }
}
