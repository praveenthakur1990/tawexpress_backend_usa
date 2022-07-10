using Newtonsoft.Json;
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
    [RoutePrefix("api/Inventory")]
    public class InventoryController : ApiController
    {
        private IInventoryBusiness _inventoryBusiness;
        public InventoryController(IInventoryBusiness inventoryBusiness)
        {
            _inventoryBusiness = inventoryBusiness;
        }

        [Route("AddStock")]
        [HttpPost]
        public HttpResponseMessage AddStock([FromBody] StockModel obj)
        {
            try
            {
                int res = _inventoryBusiness.Add(obj, CommonManager.GetTenantConnection(obj.AddedBy, string.Empty)[0].TenantConnection);
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
                CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, JsonConvert.SerializeObject(obj));
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message.ToString());
            }
        }
        [Route("GetInventoryByProductId")]
        [HttpGet]
        public HttpResponseMessage GetInventoryByProductId(int productId, string userId, int pageNumber, int pageSize, string searchStr)
        {
            PaginationModel obj = new PaginationModel();
            obj.PageNumber = pageNumber;
            obj.PageSize = pageSize;
            obj.SearchStr = searchStr;
            List<StockModel> objList = _inventoryBusiness.GetInventorybyProductId(productId, obj, CommonManager.GetTenantConnection(userId, string.Empty)[0].TenantConnection);
            return Request.CreateResponse(HttpStatusCode.OK, objList);
        }

    }
}
