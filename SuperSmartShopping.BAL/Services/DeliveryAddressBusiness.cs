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
    public class DeliveryAddressBusiness : IDeliveryAddressBusiness
    {
        public int AddUpdate(DeliveryAddressesModel model, string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                int response = 0;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPAddUpdateDeliveryAddress] @Id, @UserId, @AddressType, @FirstName, @LastName, @Address, @City, @State, @ZipCode, @Latitude,   @Longitude, @IsSetDefault, @IsActive, @Result", connection))
                    {
                        command.Parameters.AddWithValue("@Id", model.Id);
                        command.Parameters.AddWithValue("@UserId", model.UserId);
                        command.Parameters.AddWithValue("@AddressType", string.IsNullOrEmpty(model.AddressType) ? "H" : model.AddressType);
                        command.Parameters.AddWithValue("@FirstName", model.FirstName);
                        command.Parameters.AddWithValue("@LastName", model.LastName);
                        command.Parameters.AddWithValue("@Address", model.Address);
                        command.Parameters.AddWithValue("@City", model.City);
                        command.Parameters.AddWithValue("@State", model.State);
                        command.Parameters.AddWithValue("@ZipCode", model.ZipCode);
                        command.Parameters.AddWithValue("@Latitude", model.Latitude);
                        command.Parameters.AddWithValue("@Longitude", model.Longitude);
                        command.Parameters.AddWithValue("@IsSetDefault", model.IsSetDefault);
                        command.Parameters.AddWithValue("@IsActive", model.IsActive);
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

        public decimal CalculateDeliveryAddressDistance(int deliveyAddress, string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                decimal response = 0;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPCalculateOrderDistance] @DeliveryAddress, @Result", connection))
                    {
                        command.Parameters.AddWithValue("@DeliveryAddress", deliveyAddress);
                        command.Parameters.Add("@Result", SqlDbType.Decimal);
                        command.Parameters["@Result"].Direction = ParameterDirection.Output;
                        response = Convert.ToDecimal(command.ExecuteScalar());
                    }
                    connection.Close();
                    return response;
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, deliveyAddress, connectionStr);
                    return response;
                }
            }
        }

        public List<DeliveryAddressesModel> Get(int deliveryAddressId, string userId, string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                List<DeliveryAddressesModel> objList = new List<DeliveryAddressesModel>();
                DeliveryAddressesModel obj = null;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPGetDeliveryAddress] @Id, @UserId", connection))
                    {
                        command.Parameters.AddWithValue("@Id", deliveryAddressId);
                        command.Parameters.AddWithValue("@UserId", userId);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                obj = new DeliveryAddressesModel();
                                obj.Id = Convert.ToInt32(reader["Id"].ToString());
                                obj.UserId = reader["UserId"].ToString();
                                obj.AddressType = reader["AddressType"].ToString();
                                obj.FirstName = reader["FirstName"].ToString();
                                obj.LastName = reader["LastName"].ToString();
                                obj.Address = reader["Address"].ToString();
                                obj.City = reader["City"].ToString();
                                obj.State = reader["State"].ToString();
                                obj.ZipCode = reader["ZipCode"].ToString();
                                obj.Latitude = reader["Latitude"].ToString();
                                obj.Longitude = reader["Longitude"].ToString();
                                obj.IsSetDefault = Convert.ToBoolean(reader["IsSetDefault"].ToString());
                                obj.IsActive = Convert.ToBoolean(reader["IsActive"].ToString());
                                obj.IsDeleted = reader["IsDeleted"] is DBNull ? false : Convert.ToBoolean(reader["IsDeleted"].ToString());
                                objList.Add(obj);
                            }
                        }
                    }
                    connection.Close();
                    return objList;
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, deliveryAddressId, userId, connectionStr);
                    return new List<DeliveryAddressesModel>();
                }
            }
        }

        public List<DateTime> GetDeliverySlotDays(int days, string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                List<DateTime> objList = new List<DateTime>();
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPCalculateDeliverySlotDays] @days", connection))
                    {
                        command.Parameters.AddWithValue("@days", days);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                objList.Add(Convert.ToDateTime(reader["Date"].ToString()));
                            }
                        }
                    }
                    connection.Close();
                    return objList;
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, days, connectionStr);
                    return new List<DateTime>();
                }
            }
        }

        public List<DeliverySlotTime> GetDeliverySlotTime(int interval, int start, int end, string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                List<DeliverySlotTime> objList = new List<DeliverySlotTime>();
                DeliverySlotTime obj = null;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPGenerateDeliverySlot] @Interval, @StartTime, @EndTime", connection))
                    {
                        command.Parameters.AddWithValue("@Interval", interval);
                        command.Parameters.AddWithValue("@StartTime", start);
                        command.Parameters.AddWithValue("@EndTime", end);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                obj = new DeliverySlotTime();
                                obj.StartTime = Convert.ToDateTime(reader["StartTime"].ToString());
                                obj.EndTime = Convert.ToDateTime(reader["EndTime"].ToString());
                                objList.Add(obj);
                            }
                        }
                    }
                    connection.Close();
                    return objList;
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, interval, start, end, connectionStr);
                    return new List<DeliverySlotTime>();
                }
            }
        }
    }
}
