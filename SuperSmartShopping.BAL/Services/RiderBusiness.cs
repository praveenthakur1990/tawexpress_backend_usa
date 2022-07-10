using Newtonsoft.Json;
using SuperSmartShopping.BAL.Interfaces;
using SuperSmartShopping.DAL.Common;
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
    public class RiderBusiness : IRiderBusiness
    {
        public int AddUpdate(RiderModel model)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                int response = 0;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPAddUpdateRider] @Id, @FirstName, @LastName, @Mobile, @EmailAddress, @Gender, @ContactAddress, @City, @State, @ZipCode, @CreatedBy, @RiderStoreLinking, @Result", connection))
                    {
                        command.Parameters.AddWithValue("@Id", model.Id);
                        command.Parameters.AddWithValue("@FirstName", model.FirstName);
                        command.Parameters.AddWithValue("@LastName", model.LastName);
                        command.Parameters.AddWithValue("@Mobile", model.Mobile);
                        command.Parameters.AddWithValue("@EmailAddress", model.EmailAddress);
                        command.Parameters.AddWithValue("@Gender", model.Gender);
                        command.Parameters.AddWithValue("@ContactAddress", model.ContactAddress);
                        command.Parameters.AddWithValue("@City", model.City);
                        command.Parameters.AddWithValue("@State", model.State);
                        command.Parameters.AddWithValue("@ZipCode", model.ZipCode);
                        command.Parameters.AddWithValue("@CreatedBy", model.CreatedBy);

                        command.Parameters.Add("@RiderStoreLinking", SqlDbType.Structured).Value = model.StoreIdTable;
                        command.Parameters["@RiderStoreLinking"].TypeName = "dbo.tb_RiderStoreLinking_UDT";
                        command.Parameters.Add("@Result", SqlDbType.Int);
                        command.Parameters["@Result"].Direction = ParameterDirection.Output;
                        response = Convert.ToInt32(command.ExecuteScalar());
                    }
                    connection.Close();
                    return response;
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, JsonConvert.SerializeObject(model));
                    return 0;
                }
            }
        }
        public List<StoreListModel> GeStoreNames()
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                List<StoreListModel> objCategoryList = new List<StoreListModel>();
                StoreListModel obj = null;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPGetStoreList]", connection))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                obj = new StoreListModel();
                                obj.UserId = reader["UserId"].ToString();
                                obj.DomainName = reader["TenantDomain"].ToString();
                                objCategoryList.Add(obj);
                            }
                        }
                    }
                    connection.Close();
                    return objCategoryList;
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex);
                    return new List<StoreListModel>();
                }
            }
        }
        public List<RiderModel> GetRiders(int id, PaginationModel model)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                List<RiderModel> objList = new List<RiderModel>();
                RiderModel obj = null;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPGetRiderList] @Id, @PageNumber, @PageSize, @SearchStr", connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.Parameters.AddWithValue("@PageNumber", model.PageNumber);
                        command.Parameters.AddWithValue("@PageSize", model.PageSize);
                        command.Parameters.AddWithValue("@SearchStr", model.SearchStr);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                obj = new RiderModel();
                                obj.Id = Convert.ToInt32(reader["Id"]);
                                obj.FirstName = Convert.ToString(reader["FirstName"]);
                                obj.LastName = Convert.ToString(reader["LastName"]);
                                obj.Gender = Convert.ToString(reader["Gender"]);
                                obj.EmailAddress = Convert.ToString(reader["EmailAddress"]);
                                obj.Mobile = Convert.ToString(reader["Mobile"]);
                                obj.ContactAddress = Convert.ToString(reader["ContactAddress"]);
                                obj.City = Convert.ToString(reader["City"]);
                                obj.State = Convert.ToString(reader["State"]);
                                obj.ZipCode = Convert.ToString(reader["ZipCode"]);
                                obj.IsActive = Convert.ToBoolean(reader["IsActive"]);
                                obj.IsOnline = reader["IsOnline"] == DBNull.Value ? true : Convert.ToBoolean(reader["IsOnline"]);
                                obj.DeviceToken = Convert.ToString(reader["DeviceToken"]);
                                obj.StoreIds = reader["StoreLinkIds"] == DBNull.Value ? string.Empty : reader["StoreLinkIds"].ToString();
                                objList.Add(obj);
                            }
                        }
                    }
                    connection.Close();
                    return objList;
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, id, JsonConvert.SerializeObject(model));
                    return new List<RiderModel>();
                }
            }
        }
        public int UpdateDeviceToken(string emailAddress, string deviceToken)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                int response = 0;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPUpdateDeviceToken] @EmailAddress, @DeviceToken, @Result", connection))
                    {
                        command.Parameters.AddWithValue("@EmailAddress", emailAddress);
                        command.Parameters.AddWithValue("@DeviceToken", deviceToken);
                        command.Parameters.Add("@Result", SqlDbType.Int);
                        command.Parameters["@Result"].Direction = ParameterDirection.Output;
                        response = Convert.ToInt32(command.ExecuteScalar());
                    }
                    connection.Close();
                    return response;
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, emailAddress, deviceToken);
                    return 0;
                }
            }
        }
        public List<RiderModel> GetStoreLinkedRiders(string storeUserId)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                List<RiderModel> objList = new List<RiderModel>();
                RiderModel obj = null;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPGetStoreLinkedRider] @StoreUserId", connection))
                    {
                        command.Parameters.AddWithValue("@StoreUserId", storeUserId);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                obj = new RiderModel();
                                obj.Id = Convert.ToInt32(reader["Id"]);
                                obj.FirstName = Convert.ToString(reader["FirstName"]);
                                obj.LastName = Convert.ToString(reader["LastName"]);
                                obj.EmailAddress = Convert.ToString(reader["EmailAddress"]);
                                obj.Mobile = Convert.ToString(reader["Mobile"]);
                                obj.ContactAddress = Convert.ToString(reader["ContactAddress"]);
                                obj.DeviceToken = Convert.ToString(reader["DeviceToken"]);
                                objList.Add(obj);
                            }
                        }
                    }
                    connection.Close();
                    return objList;
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, storeUserId);
                    return new List<RiderModel>();
                }
            }
        }
        public int SendOrderRequestToRider(int orderId, int riderId, string storeUserId)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                int response = 0;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPAddRiderOrderRequest] @OrderId, @RiderId, @StoreUserId, @Result", connection))
                    {
                        command.Parameters.AddWithValue("@OrderId", orderId);
                        command.Parameters.AddWithValue("@RiderId", riderId);
                        command.Parameters.AddWithValue("@StoreUserId", storeUserId);
                        command.Parameters.Add("@Result", SqlDbType.Int);
                        command.Parameters["@Result"].Direction = ParameterDirection.Output;
                        response = Convert.ToInt32(command.ExecuteScalar());
                    }
                    connection.Close();
                    return response;
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, orderId, riderId);
                    return 0;
                }
            }
        }
        public List<OrderInfoModel> GetRiderOrders(string userName, string type, PaginationModel model)
        {
            List<OrderInfoModel> objOrderList = new List<OrderInfoModel>();
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                List<RiderOrderModel> objList = new List<RiderOrderModel>();
                RiderOrderModel obj = null;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPGetRiderOrder] @UserName, @Type, @PageNumber, @PageSize", connection))
                    {
                        command.Parameters.AddWithValue("@UserName", userName);
                        command.Parameters.AddWithValue("@Type", !string.IsNullOrEmpty(type) ? type : string.Empty);
                        command.Parameters.AddWithValue("@PageNumber", model.PageNumber);
                        command.Parameters.AddWithValue("@PageSize", model.PageSize);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                obj = new RiderOrderModel();
                                obj.TotalCount = Convert.ToInt32(reader["TotalCount"]);
                                obj.StoreUserId = Convert.ToString(reader["StoreUserId"]);
                                obj.OrderIds = Convert.ToString(reader["OrderIds"]);
                                objList.Add(obj);
                            }
                        }
                    }
                    connection.Close();
                    foreach (var item in objList)
                    {
                        var data = new OrderBusiness().GetOrderByIds(item.OrderIds, string.Empty, string.Empty, type, false, new PaginationModel
                        {
                            PageNumber = 1,
                            PageSize = -1,
                            SearchStr = string.Empty
                        }, CommonManager.GetTenantConnection(item.StoreUserId, string.Empty).FirstOrDefault().TenantConnection);

                        objOrderList.AddRange(data);
                    }

                    return objOrderList;

                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, userName, type, JsonConvert.SerializeObject(model));
                    return new List<OrderInfoModel>();
                }
            }
        }

        public bool IsRiderOnline(string userName)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                bool response = false;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPIsRiderOnline] @UserName, @Result", connection))
                    {
                        command.Parameters.AddWithValue("@UserName", userName);
                        command.Parameters.Add("@Result", SqlDbType.Bit);
                        command.Parameters["@Result"].Direction = ParameterDirection.Output;
                        response = Convert.ToBoolean(command.ExecuteScalar());
                    }
                    connection.Close();
                    return response;
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, userName);
                    return false;
                }
            }
        }

        public int UpdateRiderOnlineStatus(string emailAddress, bool isOnline)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                int response = 0;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPUpdateRiderOnlineStatus] @EmailAddress, @isOnline, @Result", connection))
                    {
                        command.Parameters.AddWithValue("@EmailAddress", emailAddress);
                        command.Parameters.AddWithValue("@isOnline", isOnline);
                        command.Parameters.Add("@Result", SqlDbType.Int);
                        command.Parameters["@Result"].Direction = ParameterDirection.Output;
                        response = Convert.ToInt32(command.ExecuteScalar());
                    }
                    connection.Close();
                    return response;
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, emailAddress, isOnline);
                    return 0;
                }
            }
        }

        public int UpdateRiderOrderStatus(string userName, int orderId, string storeUserId)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                int response = 0;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPUpdateRiderOrderStatus] @UserName, @OrderId, @StoreUserId, @Result", connection))
                    {
                        command.Parameters.AddWithValue("@UserName", userName);
                        command.Parameters.AddWithValue("@OrderId", orderId);
                        command.Parameters.AddWithValue("@StoreUserId", storeUserId);
                        command.Parameters.Add("@Result", SqlDbType.Bit);
                        command.Parameters["@Result"].Direction = ParameterDirection.Output;
                        response = Convert.ToInt32(command.ExecuteScalar());
                    }
                    connection.Close();
                    return response;
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, userName);
                    return 0;
                }
            }
        }
    }
}
