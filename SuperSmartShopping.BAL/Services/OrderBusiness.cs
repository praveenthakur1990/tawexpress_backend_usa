using Newtonsoft.Json;
using SuperSmartShopping.BAL.Interfaces;
using SuperSmartShopping.DAL.Common;
using SuperSmartShopping.DAL.Models;
using SuperSmartShopping.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.BAL.Services
{
    public class OrderBusiness : IOrderBusiness
    {
        public string GenerateOrderNumber(string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                string response = string.Empty;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPGetOrderNumber] @NextOrderNumber", connection))
                    {
                        command.Parameters.Add("@NextOrderNumber", SqlDbType.NVarChar, 20);
                        command.Parameters["@NextOrderNumber"].Direction = ParameterDirection.Output;
                        response = command.ExecuteScalar().ToString();
                    }
                    connection.Close();
                    return response;
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, connectionStr);
                    return string.Empty;
                }
            }
        }

        public List<OrderInfoModel> GetOrder(int orderId, string orderNo, string orderBy, string status, bool IsCurrentDate, PaginationModel pageModel, string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                List<OrderInfoModel> objList = new List<OrderInfoModel>();
                OrderInfoModel obj = null;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPGetOrderList] @OrderId, @OrderNo, @OrderBy, @Status, @IsCurrentDate , @PageNumber, @PageSize, @SearchStr", connection))
                    {
                        command.Parameters.AddWithValue("@OrderId", orderId);
                        command.Parameters.AddWithValue("@OrderNo", orderNo == null ? string.Empty : orderNo);
                        command.Parameters.AddWithValue("@OrderBy", orderBy == null ? string.Empty : orderBy);
                        command.Parameters.AddWithValue("@Status", status == null ? string.Empty : status);
                        command.Parameters.AddWithValue("@IsCurrentDate", IsCurrentDate);
                        command.Parameters.AddWithValue("@PageNumber", pageModel.PageNumber == 0 ? 1 : pageModel.PageNumber);
                        command.Parameters.AddWithValue("@PageSize", pageModel.PageSize);
                        command.Parameters.AddWithValue("@SearchStr", pageModel.SearchStr == null ? string.Empty : pageModel.SearchStr);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                obj = new OrderInfoModel();
                                obj.RiderInfo = new RiderModel();
                                obj.OrderDetail = new List<OrderDetailInfoModel>();
                                obj.ReferenceId = reader["CapturedId"] is DBNull ? string.Empty : reader["CapturedId"].ToString();
                                obj.CapturedAmt = reader["CapturedAmt"] is DBNull ? 0 : Convert.ToDecimal(reader["CapturedAmt"].ToString());
                                obj.AddressType = reader["AddressType"] is DBNull ? string.Empty : reader["AddressType"].ToString();
                                obj.FirstName = reader["FirstName"] is DBNull ? string.Empty : reader["FirstName"].ToString();
                                obj.LastName = reader["LastName"] is DBNull ? string.Empty : reader["LastName"].ToString();
                                obj.Address = reader["Address"] is DBNull ? string.Empty : reader["Address"].ToString();
                                obj.City = reader["City"] is DBNull ? string.Empty : reader["City"].ToString();
                                obj.State = reader["State"] is DBNull ? string.Empty : reader["State"].ToString();
                                obj.ZipCode = reader["ZipCode"] is DBNull ? string.Empty : reader["ZipCode"].ToString();
                                obj.OrderId = Convert.ToInt32(reader["OrderId"]);
                                obj.OrderNo = reader["OrderNo"].ToString();
                                obj.OrderBy = reader["OrderBy"].ToString();
                                obj.OrderType = reader["OrderType"].ToString();

                                if (reader["DeliveryDate"] is DBNull)
                                {
                                    obj.DeliveryDate = null;
                                }
                                else
                                {
                                    obj.DeliveryDate = Convert.ToDateTime(reader["DeliveryDate"].ToString());
                                }

                                if (reader["DeliveryStartTime"] is DBNull)
                                {
                                    obj.DeliveryStartTime = null;
                                }
                                else
                                {
                                    obj.DeliveryStartTime = Convert.ToDateTime(reader["DeliveryStartTime"].ToString());
                                }
                                if (reader["DeliveryEndTime"] is DBNull)
                                {
                                    obj.DeliveryEndTime = null;
                                }
                                else
                                {
                                    obj.DeliveryEndTime = Convert.ToDateTime(reader["DeliveryEndTime"].ToString());
                                }
                                obj.OrderedDate = Convert.ToDateTime(reader["OrderedDate"]);
                                obj.TaxRate = Convert.ToDecimal(reader["TaxRate"]);
                                obj.TaxAmount = Convert.ToDecimal(reader["TaxAmount"]);
                                obj.DeliveryCharges = Convert.ToDecimal(reader["DeliveryCharges"]);
                                obj.TotalRows = Convert.ToInt32(reader["TotalRows"]);
                                obj.OrderDetail = GetOrderDetail(obj.OrderId, connectionStr);
                                obj.SubTotal = obj.OrderDetail.Sum(c => c.TotalPrice);
                                obj.Status = reader["OrderStatus"].ToString();
                                if (reader["OrderStatusChangedDate"] is DBNull)
                                {
                                    obj.StatusChangedDate = null;
                                }
                                else
                                {
                                    obj.StatusChangedDate = Convert.ToDateTime(reader["OrderStatusChangedDate"].ToString());
                                }
                                obj.OrderStatusLogs = GetOrderStatusLogs(obj.OrderId, connectionStr);
                                obj.UserInfoModel = new UserBusiness().GetPersonalInfo(obj.OrderBy);
                                obj.PaymentStatus = reader["PaymentStatus"] is DBNull ? string.Empty : Convert.ToString(reader["PaymentStatus"]);
                                if (reader["TransactionDate"] is DBNull)
                                {
                                    obj.TransactionDate = null;
                                }
                                else
                                {
                                    obj.TransactionDate = Convert.ToDateTime(reader["TransactionDate"]);
                                }
                                obj.RiderInfo.IsRequestSent = Convert.ToBoolean(reader["IsRequestSent"]);
                                obj.RiderInfo.IsAccepted = Convert.ToBoolean(reader["IsAccepted"]);

                                obj.RiderInfo.Id = Convert.ToInt32(reader["RiderId"]);
                                obj.RiderInfo.FirstName = Convert.ToString(reader["RiderName"]);
                                obj.RiderInfo.ContactAddress = Convert.ToString(reader["ContactAddress"]);
                                obj.RiderInfo.Mobile = Convert.ToString(reader["Mobile"]);
                                obj.RiderInfo.EmailAddress = Convert.ToString(reader["EmailAddress"]);
                                if (reader["AcceptedDate"] is DBNull)
                                {
                                    obj.RiderInfo.AcceptedDate = null;
                                }
                                else
                                {
                                    obj.RiderInfo.AcceptedDate = Convert.ToDateTime(reader["AcceptedDate"]);
                                }
                                if (reader["RequestSentDate"] is DBNull)
                                {
                                    obj.RiderInfo.RequestedDate = null;
                                }
                                else
                                {
                                    obj.RiderInfo.RequestedDate = Convert.ToDateTime(reader["RequestSentDate"]);
                                }
                                objList.Add(obj);
                            }
                        }
                    }
                    connection.Close();
                    return objList;
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, orderId, orderNo, status, IsCurrentDate, pageModel, connectionStr);
                    return new List<OrderInfoModel>();
                }
            }
        }

        public List<OrderInfoModel> GetOrderByIds(string orderIds, string orderNo, string orderBy, string status, bool IsCurrentDate, PaginationModel pageModel, string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                List<OrderInfoModel> objList = new List<OrderInfoModel>();
                OrderInfoModel obj = null;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPGetRiderOrderList] @OrderIds, @OrderNo, @OrderBy, @Status, @IsCurrentDate , @PageNumber, @PageSize, @SearchStr", connection))
                    {
                        command.Parameters.AddWithValue("@OrderIds", orderIds);
                        command.Parameters.AddWithValue("@OrderNo", orderNo == null ? string.Empty : orderNo);
                        command.Parameters.AddWithValue("@OrderBy", orderBy == null ? string.Empty : orderBy);
                        command.Parameters.AddWithValue("@Status", status == null ? string.Empty : status);
                        command.Parameters.AddWithValue("@IsCurrentDate", IsCurrentDate);
                        command.Parameters.AddWithValue("@PageNumber", pageModel.PageNumber == 0 ? 1 : pageModel.PageNumber);
                        command.Parameters.AddWithValue("@PageSize", pageModel.PageSize);
                        command.Parameters.AddWithValue("@SearchStr", pageModel.SearchStr == null ? string.Empty : pageModel.SearchStr);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                obj = new OrderInfoModel();
                                obj.RiderInfo = new RiderModel();
                                obj.OrderDetail = new List<OrderDetailInfoModel>();
                                obj.ReferenceId = reader["CapturedId"] is DBNull ? string.Empty : reader["CapturedId"].ToString();
                                obj.CapturedAmt = reader["CapturedAmt"] is DBNull ? 0 : Convert.ToDecimal(reader["CapturedAmt"].ToString());
                                obj.AddressType = reader["AddressType"] is DBNull ? string.Empty : reader["AddressType"].ToString();
                                obj.FirstName = reader["FirstName"] is DBNull ? string.Empty : reader["FirstName"].ToString();
                                obj.LastName = reader["LastName"] is DBNull ? string.Empty : reader["LastName"].ToString();
                                obj.Address = reader["Address"] is DBNull ? string.Empty : reader["Address"].ToString();
                                obj.City = reader["City"] is DBNull ? string.Empty : reader["City"].ToString();
                                obj.State = reader["State"] is DBNull ? string.Empty : reader["State"].ToString();
                                obj.ZipCode = reader["ZipCode"] is DBNull ? string.Empty : reader["ZipCode"].ToString();
                                obj.Latitude = reader["UserLatitude"] is DBNull ? string.Empty : reader["UserLatitude"].ToString();
                                obj.Longitude = reader["UserLongitude"] is DBNull ? string.Empty : reader["UserLongitude"].ToString();
                                obj.OrderId = Convert.ToInt32(reader["OrderId"]);
                                obj.OrderNo = reader["OrderNo"].ToString();
                                obj.OrderBy = reader["OrderBy"].ToString();
                                obj.OrderType = reader["OrderType"].ToString();

                                if (reader["DeliveryDate"] is DBNull)
                                {
                                    obj.DeliveryDate = null;
                                }
                                else
                                {
                                    obj.DeliveryDate = Convert.ToDateTime(reader["DeliveryDate"].ToString());
                                }

                                if (reader["DeliveryStartTime"] is DBNull)
                                {
                                    obj.DeliveryStartTime = null;
                                }
                                else
                                {
                                    obj.DeliveryStartTime = Convert.ToDateTime(reader["DeliveryStartTime"].ToString());
                                }
                                if (reader["DeliveryEndTime"] is DBNull)
                                {
                                    obj.DeliveryEndTime = null;
                                }
                                else
                                {
                                    obj.DeliveryEndTime = Convert.ToDateTime(reader["DeliveryEndTime"].ToString());
                                }
                                obj.OrderedDate = Convert.ToDateTime(reader["OrderedDate"]);
                                obj.TaxRate = Convert.ToDecimal(reader["TaxRate"]);
                                obj.TaxAmount = Convert.ToDecimal(reader["TaxAmount"]);
                                obj.DeliveryCharges = Convert.ToDecimal(reader["DeliveryCharges"]);
                                obj.TotalRows = Convert.ToInt32(reader["TotalRows"]);
                                obj.OrderDetail = GetOrderDetail(obj.OrderId, connectionStr);
                                obj.SubTotal = obj.OrderDetail.Sum(c => c.TotalPrice);
                                obj.Status = reader["OrderStatus"].ToString();
                                if (reader["OrderStatusChangedDate"] is DBNull)
                                {
                                    obj.StatusChangedDate = null;
                                }
                                else
                                {
                                    obj.StatusChangedDate = Convert.ToDateTime(reader["OrderStatusChangedDate"].ToString());
                                }
                                obj.OrderStatusLogs = GetOrderStatusLogs(obj.OrderId, connectionStr);
                                obj.UserInfoModel = new UserBusiness().GetPersonalInfo(obj.OrderBy);
                                obj.PaymentStatus = reader["PaymentStatus"] is DBNull ? string.Empty : Convert.ToString(reader["PaymentStatus"]);
                                if (reader["TransactionDate"] is DBNull)
                                {
                                    obj.TransactionDate = null;
                                }
                                else
                                {
                                    obj.TransactionDate = Convert.ToDateTime(reader["TransactionDate"]);
                                }
                                obj.RiderInfo.IsRequestSent = Convert.ToBoolean(reader["IsRequestSent"]);
                                obj.RiderInfo.IsAccepted = Convert.ToBoolean(reader["IsAccepted"]);

                                obj.RiderInfo.Id = Convert.ToInt32(reader["RiderId"]);
                                obj.RiderInfo.FirstName = Convert.ToString(reader["RiderName"]);
                                obj.RiderInfo.ContactAddress = Convert.ToString(reader["ContactAddress"]);
                                obj.RiderInfo.Mobile = Convert.ToString(reader["Mobile"]);
                                obj.RiderInfo.EmailAddress = Convert.ToString(reader["EmailAddress"]);
                                if (reader["AcceptedDate"] is DBNull)
                                {
                                    obj.RiderInfo.AcceptedDate = null;
                                }
                                else
                                {
                                    obj.RiderInfo.AcceptedDate = Convert.ToDateTime(reader["AcceptedDate"]);
                                }
                                if (reader["RequestSentDate"] is DBNull)
                                {
                                    obj.RiderInfo.RequestedDate = null;
                                }
                                else
                                {
                                    obj.RiderInfo.RequestedDate = Convert.ToDateTime(reader["RequestSentDate"]);
                                }

                                obj.StoreName = Convert.ToString(reader["StoreName"]);
                                obj.StoreAddress = Convert.ToString(reader["StoreAddress"]);
                                obj.StoreUserId = Convert.ToString(reader["StoreUserId"]);
                                obj.StoreLatitude = reader["Latitude"] is DBNull ? string.Empty : reader["Latitude"].ToString();
                                obj.StoreLongitude = reader["Longitude"] is DBNull ? string.Empty : reader["Longitude"].ToString();
                                objList.Add(obj);
                            }
                        }
                    }
                    connection.Close();
                    return objList;
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, orderIds, orderNo, status, IsCurrentDate, JsonConvert.SerializeObject(pageModel), connectionStr);
                    return new List<OrderInfoModel>();
                }
            }
        }

        public List<OrderDetailInfoModel> GetOrderDetail(int orderId, string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                List<OrderDetailInfoModel> objList = new List<OrderDetailInfoModel>();
                OrderDetailInfoModel obj = null;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPGetOrderDetail] @OrderId", connection))
                    {
                        command.Parameters.AddWithValue("@OrderId", orderId);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                obj = new OrderDetailInfoModel();
                                obj.ProductId = Convert.ToInt32(reader["Id"]);
                                obj.ProductName = reader["ProductName"] is DBNull ? string.Empty : reader["ProductName"].ToString();
                                if (string.IsNullOrEmpty(reader["Image"].ToString()))
                                {
                                    obj.ProductImg = string.Empty;
                                }
                                else
                                {
                                    obj.ProductImg = ConfigurationManager.AppSettings["BackendAPPBaseUrl"].ToString() + reader["Image"].ToString();
                                }

                                obj.Price = Convert.ToDecimal(reader["Price"]);
                                obj.Qty = Convert.ToInt32(reader["Qty"]);
                                obj.TotalPrice = Convert.ToDecimal(reader["TotalPrice"]);
                                obj.CategoryName = reader["CategoryName"] is DBNull ? string.Empty : reader["CategoryName"].ToString();
                                obj.SubCategoryName = reader["SubCategoryName"] is DBNull ? string.Empty : reader["SubCategoryName"].ToString();

                                obj.OfferType = reader["OfferType"] is DBNull ? string.Empty : reader["OfferType"].ToString();
                                obj.OfferValue = reader["OfferValue"] is DBNull ? 0 : Convert.ToDecimal(reader["OfferValue"].ToString());
                                obj.FinalValue = reader["FinalValue"] is DBNull ? string.Empty : reader["FinalValue"].ToString();
                                obj.FinalOfferValue = reader["FinalOfferValue"] is DBNull ? string.Empty : reader["FinalOfferValue"].ToString();
                                objList.Add(obj);
                            }
                        }
                    }
                    connection.Close();
                    return objList;
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, orderId, connectionStr);
                    return new List<OrderDetailInfoModel>();
                }
            }
        }

        public List<OrderStatusLogs> GetOrderStatusLogs(int orderId, string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                List<OrderStatusLogs> objList = new List<OrderStatusLogs>();
                OrderStatusLogs obj = null;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPGetOrderStatusLogs] @OrderId", connection))
                    {
                        command.Parameters.AddWithValue("@OrderId", orderId);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                obj = new OrderStatusLogs();
                                obj.OrderId = Convert.ToInt32(reader["OrderId"]);
                                obj.Status = reader["Status"] is DBNull ? string.Empty : reader["Status"].ToString();
                                obj.ChangedDateTime = Convert.ToDateTime(reader["ChangedDateTime"]);
                                obj.ChangedBy = Convert.ToString(reader["ChangedBy"]);
                                objList.Add(obj);
                            }
                        }
                    }
                    connection.Close();
                    return objList;
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, orderId, connectionStr);
                    return new List<OrderStatusLogs>();
                }
            }
        }

        public int SaveOrder(OrderModel model, string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                int response = 0;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPSaveOrder] @OrderNo, @OrderBy, @OrderType, @DeliveryAddressId, @DeliveryDate, @DeliveryStartTime, @DeliveryEndTime, @OrderDate, @TaxRate, @TaxAmount, @DeliveryCharges, @OrderDetails, @PaymentInfoDetails, @Result", connection))
                    {
                        command.Parameters.AddWithValue("@OrderNo", model.OrderNo);
                        command.Parameters.AddWithValue("@OrderBy", model.OrderBy);
                        command.Parameters.AddWithValue("@OrderType", model.OrderType);
                        command.Parameters.AddWithValue("@DeliveryAddressId", model.DeliveryAddressId);
                        if (model.DeliveryDate == null)
                        {
                            var col = command.Parameters.Add("@DeliveryDate", SqlDbType.DateTime);
                            col.Value = DBNull.Value;
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@DeliveryDate", model.DeliveryDate);
                        }

                        if (model.DeliveryStartTime == null)
                        {
                            var col = command.Parameters.Add("@DeliveryStartTime", SqlDbType.DateTime);
                            col.Value = DBNull.Value;
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@DeliveryStartTime", model.DeliveryStartTime);
                        }

                        if (model.DeliveryEndTime == null)
                        {
                            var col = command.Parameters.Add("@DeliveryEndTime", SqlDbType.DateTime);
                            col.Value = DBNull.Value;
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@DeliveryEndTime", model.DeliveryEndTime);
                        }

                        if (model.OrderDate == null)
                        {
                            var col = command.Parameters.Add("@OrderDate", SqlDbType.DateTime);
                            col.Value = DBNull.Value;
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@OrderDate", model.OrderDate);
                        }
                        command.Parameters.AddWithValue("@TaxRate", model.TaxRate);
                        command.Parameters.AddWithValue("@TaxAmount", model.TaxAmount);
                        command.Parameters.AddWithValue("@DeliveryCharges", model.DeliveryCharges);
                        command.Parameters.Add("@OrderDetails", SqlDbType.Structured).Value = model.OrderDetailsTable;
                        command.Parameters["@OrderDetails"].TypeName = "dbo.tb_OrderDetails_UDT";
                        command.Parameters.Add("@PaymentInfoDetails", SqlDbType.Structured).Value = model.PaymentTable;
                        command.Parameters["@PaymentInfoDetails"].TypeName = "dbo.tb_Order_tb_Payments_UDT";

                        command.Parameters.Add("@Result", SqlDbType.Int);
                        command.Parameters["@Result"].Direction = ParameterDirection.Output;
                        response = Convert.ToInt32(command.ExecuteScalar());
                    }
                    connection.Close();
                    return response;
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, JsonConvert.SerializeObject(model), connectionStr);
                    return 0;
                }
            }
        }

        public int UpdateOrderStatus(int orderId, string status, string userId, string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                int response = 0;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPUpdateOrderStatusLogs] @OrderId, @Status, @ChangedBy, @Result", connection))
                    {
                        command.Parameters.AddWithValue("@OrderId", orderId);
                        command.Parameters.AddWithValue("@Status", status);
                        command.Parameters.AddWithValue("@ChangedBy", userId);
                        command.Parameters.Add("@Result", SqlDbType.Int);
                        command.Parameters["@Result"].Direction = ParameterDirection.Output;
                        response = Convert.ToInt32(command.ExecuteScalar());
                    }
                    connection.Close();
                    return response;
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, orderId, status, userId, connectionStr);
                    return 0;
                }
            }
        }
    }
}
