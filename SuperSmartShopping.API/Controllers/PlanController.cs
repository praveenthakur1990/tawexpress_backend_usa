using SuperSmartShopping.BAL.Interfaces;
using SuperSmartShopping.DAL.Common;
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
    [RoutePrefix("api/Plan")]
    public class PlanController : ApiController
    {
        private IPlanBusiness _planBusiness;
        public PlanController(IPlanBusiness planBusiness)
        {
            _planBusiness = planBusiness;
        }

        [Route("AddUpdatePlan")]
        [HttpPost]
        public HttpResponseMessage AddUpdatePlan(PlanModel model)
        {
            try
            {
                int res = _planBusiness.AddUpdatePlan(model);
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
                CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, model);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message.ToString());
            }
        }

        [Route("GetPlanList")]
        [HttpGet]
        public HttpResponseMessage GetPlanList()
        {
            List<PlanModel> objList = _planBusiness.GetPlans(0);
            return Request.CreateResponse(HttpStatusCode.OK, objList);
        }

        [Route("GetPlanById")]
        [HttpGet]
        public HttpResponseMessage GetPlanById(int planId)
        {
            PlanModel obj = _planBusiness.GetPlans(planId).FirstOrDefault();
            return Request.CreateResponse(HttpStatusCode.OK, obj);
        }
    }
}
