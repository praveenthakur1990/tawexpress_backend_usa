using Newtonsoft.Json;
using SuperSmartShopping.BAL.Interfaces;
using SuperSmartShopping.DAL.Common;
using SuperSmartShopping.DAL.Models;
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
    [RoutePrefix("api/UnitMeasurement")]
    public class UnitMeasurementController : ApiController
    {
        private IUnitMeasurementBusiness _unitMeasurementBusiness;
        public UnitMeasurementController(IUnitMeasurementBusiness unitMeasurementBusiness)
        {
            _unitMeasurementBusiness = unitMeasurementBusiness;
        }

        [Route("AddUpdateUnitMeasurement")]
        [HttpPost]
        public HttpResponseMessage AddUpdateUnitMeasurement([FromBody] UnitMeasurement obj)
        {
            try
            {
                int res = _unitMeasurementBusiness.AddUpdateUnitMeasurement(obj, CommonManager.GetTenantConnection(obj.CreatedBy, string.Empty)[0].TenantConnection);
                return Request.CreateResponse(HttpStatusCode.OK, res.ToString());

            }
            catch (Exception ex)
            {
                CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, JsonConvert.SerializeObject(obj));
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message.ToString());
            }
        }

        [Route("GetUnitMeasurement")]
        [HttpGet]
        public HttpResponseMessage GetUnitMeasurement(int id, bool isShowInAdmin, string userId)
        {
            List<UnitMeasurement> objList = new List<UnitMeasurement>();
            objList = _unitMeasurementBusiness.GetUnitMeasurements(id, isShowInAdmin, CommonManager.GetTenantConnection(userId, string.Empty)[0].TenantConnection).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, objList);
        }


    }
}
