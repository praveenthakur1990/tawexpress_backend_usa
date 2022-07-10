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
using System.Web.Http;

namespace SuperSmartShopping.API.Controllers
{
    [Authorize]
    [RoutePrefix("api/Order")]
    public class OrderController : ApiController
    {
        private IOrderBusiness _orderBusiness;
        public OrderController(IOrderBusiness orderBusiness)
        {
            _orderBusiness = orderBusiness;
        }

        [Route("SaveOrder")]
        [HttpPost]
        public HttpResponseMessage SaveOrder(OrderModel model)
        {
            try
            {
                DataTable table = CommonManager.ToDataTable(model.OrderDetails);
                model.OrderDetailsTable = table;
                model.OrderNo = _orderBusiness.GenerateOrderNumber(CommonManager.GetTenantConnection(model.StoreUserId, string.Empty).FirstOrDefault().TenantConnection);
                int res = _orderBusiness.SaveOrder(model, CommonManager.GetTenantConnection(model.StoreUserId, string.Empty).FirstOrDefault().TenantConnection);
                return Request.CreateResponse(HttpStatusCode.OK, res);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message.ToString());
            }
        }

        [Route("GetOrderList")]
        [HttpGet]
        public HttpResponseMessage GetOrderList(string userId, int orderId = 0, string orderNo = "", string orderBy = "", bool IsCurrentDate = false, int pageNumber = 1, int pageSize = 20, string searchStr = "")
        {
            List<OrderInfoModel> objList = new List<OrderInfoModel>();
            PaginationModel objPagination = new PaginationModel();
            objPagination.PageNumber = pageNumber == 0 ? 1 : pageNumber;
            objPagination.PageSize = pageSize;
            objPagination.SearchStr = searchStr == null ? string.Empty : searchStr;
            objList = _orderBusiness.GetOrder(orderId, orderNo, orderBy, string.Empty, false, objPagination, CommonManager.GetTenantConnection(userId, string.Empty)[0].TenantConnection);
            return Request.CreateResponse(HttpStatusCode.OK, objList);
        }

        [Route("UpdateOrderStatus")]
        [HttpPost]
        public HttpResponseMessage UpdateOrderStatus(int orderId, string status, string userId, string riderUserId = "")
        {
            try
            {
                int res = _orderBusiness.UpdateOrderStatus(orderId, status, (!string.IsNullOrEmpty(riderUserId) ? riderUserId : userId), CommonManager.GetTenantConnection(userId, string.Empty)[0].TenantConnection);
                if (res > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, res);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message.ToString());
            }
        }
    }
}
