using Stripe;
using SuperSmartShopping.BAL.Interfaces;
using SuperSmartShopping.BAL.Services;
using SuperSmartShopping.DAL.Common;
using SuperSmartShopping.DAL.Models;
using SuperSmartShopping.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static SuperSmartShopping.DAL.Enums.EnumHelper;

namespace SuperSmartShopping.API.Controllers
{
    //[AllowAnonymous]
    [RoutePrefix("api/Payment")]
    public class PaymentController : ApiController
    {
        private IOrderBusiness _orderBusiness;
        public PaymentController(IOrderBusiness orderBusiness)
        {
            _orderBusiness = orderBusiness;
        }

        [Route("SaveOrder")]
        [HttpPost]
        public HttpResponseMessage Payment(OrderModel model)
        {
            try
            {
                //string storeUserId, string loggedInUserId, decimal price, string stripeToken = "", string email = ""
                TenantModel objTenantModel = CommonManager.GetTenantConnection(model.StoreUserId, string.Empty).FirstOrDefault();
                StoreModel objStoreModel = new StoreBusiness().GetStore(objTenantModel.TenantConnection);
                //StripeConfiguration.ApiKey = ConfigurationManager.AppSettings["stripeSecretKey"].ToString();
                StripeConfiguration.ApiKey = objStoreModel.StripeSecretkey;
                var customers = new CustomerService();
                var charges = new ChargeService();
                var customer = customers.Create(new CustomerCreateOptions
                {
                    Email = model.Email,
                    Source = model.StripeToken
                });

                decimal totalPrice = model.OrderDetails.Select(c => c.TotalPrice).Sum().Value;
                decimal taxAmt = (decimal)(totalPrice * model.TaxRate / 100);
                decimal totalAmount = (decimal)(totalPrice + taxAmt + model.DeliveryCharges);
                var charge = charges.Create(new ChargeCreateOptions
                {
                    Amount = Convert.ToInt64(Convert.ToDecimal(totalAmount * 100)),
                    Description = "Taw Express order payment",
                    Currency = "USD",
                    Customer = customer.Id
                });
                if (charge.Status == "succeeded")
                {
                    List<PaymentCustomModel> objPaymentList = new List<PaymentCustomModel>();
                    PaymentCustomModel obj = new PaymentCustomModel();
                    obj.OrderResponse = new OrderModel();

                    model.OrderDate = DateTime.UtcNow;
                    model.OrderNo = _orderBusiness.GenerateOrderNumber(CommonManager.GetTenantConnection(model.StoreUserId, string.Empty).FirstOrDefault().TenantConnection);

                    //converting orderdetail to datatable
                    DataTable table = CommonManager.ToDataTable(model.OrderDetails);
                    model.OrderDetailsTable = table;

                    obj.OrderResponse = model;
                    obj.PaymentResponse = new PaymentModel();
                    obj.PaymentResponse.OrderId = 0;
                    obj.PaymentResponse.CapturedId = charge.Id;
                    obj.PaymentResponse.CapturedAmt = Convert.ToDecimal(charge.AmountCaptured) / 100;
                    obj.PaymentResponse.Currency = charge.Currency;
                    obj.PaymentResponse.Email_name = charge.BillingDetails.Name;
                    obj.PaymentResponse.Funding = charge.PaymentMethodDetails.Card.Funding;
                    obj.PaymentResponse.NetWorkStatus = charge.Outcome.NetworkStatus;
                    obj.PaymentResponse.SellerMessage = charge.Outcome.SellerMessage;
                    obj.PaymentResponse.Paid = charge.Paid;
                    obj.PaymentResponse.PaymentMethod = charge.PaymentMethod;
                    obj.PaymentResponse.Card_brand = charge.PaymentMethodDetails.Card.Brand;
                    obj.PaymentResponse.Country = charge.PaymentMethodDetails.Card.Country;
                    obj.PaymentResponse.Network = charge.PaymentMethodDetails.Card.Network;
                    obj.PaymentResponse.Last4 = charge.PaymentMethodDetails.Card.Last4;
                    obj.PaymentResponse.Exp_Month = Convert.ToInt32(charge.PaymentMethodDetails.Card.ExpMonth);
                    obj.PaymentResponse.Exp_Year = Convert.ToInt32(charge.PaymentMethodDetails.Card.ExpYear);
                    obj.PaymentResponse.Status = charge.Status;
                    obj.PaymentResponse.Receipt_url = charge.ReceiptUrl;
                    obj.PaymentResponse.FailureCode = charge.FailureCode;
                    obj.PaymentResponse.FailureMessage = charge.FailureMessage;
                    obj.PaymentResponse.TransactionDate = charge.Created;
                    objPaymentList.Add(obj);

                    //converting paymentInfo into datatable
                    DataTable paymentTable = CommonManager.ToDataTable(objPaymentList.Select(c => c.PaymentResponse).ToList());
                    model.PaymentTable = paymentTable;

                    int res = _orderBusiness.SaveOrder(model, CommonManager.GetTenantConnection(model.StoreUserId, string.Empty).FirstOrDefault().TenantConnection);
                    if (res > 0)
                    {
                        PaginationModel objPagination = new PaginationModel();
                        objPagination.PageNumber = 1; objPagination.PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]); objPagination.SearchStr = string.Empty;
                        OrderInfoModel result = _orderBusiness.GetOrder(res, string.Empty, string.Empty, string.Empty, false, objPagination, objTenantModel.TenantConnection).FirstOrDefault();
                        return Request.CreateResponse(HttpStatusCode.OK, result);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred while saving order details, please contact site administrator");
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred while payment order, please contact site administrator");
                }
            }
            catch (StripeException e)
            {
                switch (e.StripeError.Type)
                {
                    case "card_error":
                        Console.WriteLine("Code: " + e.StripeError.Code);
                        Console.WriteLine("Message: " + e.StripeError.Message);
                        return Request.CreateResponse(HttpStatusCode.BadRequest, e.StripeError.Message);
                    case "api_connection_error":
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "api_connection_error");
                    case "api_error":
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "api_error");
                    case "authentication_error":
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "authentication_error");
                    case "invalid_request_error":
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "invalid_request_error");
                    case "rate_limit_error":
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "rate_limit_error");
                    case "validation_error":
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "validation_error");
                    default:
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "An error occured payment");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message.ToString());
            }

        }
       
    }
}
