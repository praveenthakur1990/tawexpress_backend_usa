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
    public class StoreBusiness : IStoreBusiness
    {
        public int AddBannerImage(List<BannerImages> model, string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                int response = 0;
                try
                {
                    connection.Open();
                    foreach (var item in model)
                    {
                        using (SqlCommand command = new SqlCommand("Exec [dbo].[USPAddBannerImage] @Id, @BannerType, @ImagePath, @IsActive, @CreatedBy, @Result", connection))
                        {
                            command.Parameters.AddWithValue("@Id", item.Id);
                            command.Parameters.AddWithValue("@BannerType", item.BannerType);
                            command.Parameters.AddWithValue("@ImagePath", item.ImagePath);
                            command.Parameters.AddWithValue("@IsActive", item.IsActive);
                            command.Parameters.AddWithValue("@CreatedBy", item.CreatedBy);
                            command.Parameters.Add("@Result", SqlDbType.Int);
                            command.Parameters["@Result"].Direction = ParameterDirection.Output;
                            response = Convert.ToInt32(command.ExecuteScalar());
                        }
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

        public int AddUpdate(StoreModel model, string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                int response = 0;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPAddUpdateStore] @Id, @Name, @Email, @Mobile, @Address, @State, @City, @ZipCode, @CountryCode, @Latitude, @Longitude, @ContactPersonName, @ContactNumber, @LogoPath, @GSTRegistrationNumber, @GSTFilePath, @ActivePlan, @Commision, @PlanActiveDate, @CurrencySymbol, @TimeZone, @PlanId, @QrCodePath, @CreatedBy, @Result", connection))
                    {
                        command.Parameters.AddWithValue("@Id", model.Id);
                        command.Parameters.AddWithValue("@Name", model.Name);
                        command.Parameters.AddWithValue("@Email", model.Email);
                        command.Parameters.AddWithValue("@Mobile", model.Mobile);
                        command.Parameters.AddWithValue("@Address", model.Address);
                        command.Parameters.AddWithValue("@State", model.State);
                        command.Parameters.AddWithValue("@City", model.City);
                        command.Parameters.AddWithValue("@ZipCode", model.ZipCode);
                        command.Parameters.AddWithValue("@CountryCode", model.CountryCode);
                        command.Parameters.AddWithValue("@Latitude", model.Latitude);
                        command.Parameters.AddWithValue("@Longitude", model.Longitude);
                        command.Parameters.AddWithValue("@ContactPersonName", model.ContactPersonName);
                        command.Parameters.AddWithValue("@ContactNumber", model.ContactNumber);
                        command.Parameters.AddWithValue("@LogoPath", model.LogoPath);
                        command.Parameters.AddWithValue("@GSTRegistrationNumber", model.GSTRegistrationNumber);
                        command.Parameters.AddWithValue("@GSTFilePath", model.GSTFilePath);
                        command.Parameters.AddWithValue("@ActivePlan", model.ActivePlan);
                        command.Parameters.AddWithValue("@Commision", model.Commision);
                        command.Parameters.AddWithValue("@PlanActiveDate", model.PlanActiveDate);
                        command.Parameters.AddWithValue("@CurrencySymbol", model.CurrencySymbol);
                        command.Parameters.AddWithValue("@TimeZone", model.TimeZone);
                        command.Parameters.AddWithValue("@PlanId", model.PlanId);
                        command.Parameters.AddWithValue("@QrCodePath", model.QrCodePath);
                        command.Parameters.AddWithValue("@CreatedBy", model.CreatedBy);
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

        public int AddUpdateStripeAPIKey(string publishablekey, string secretkey, string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                int response = 0;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPAddUpdateStripeAPIKey] @Publishablekey, @Secretkey, @Result", connection))
                    {
                        command.Parameters.AddWithValue("@Publishablekey", publishablekey);
                        command.Parameters.AddWithValue("@Secretkey", secretkey);
                        command.Parameters.Add("@Result", SqlDbType.Int);
                        command.Parameters["@Result"].Direction = ParameterDirection.Output;
                        response = Convert.ToInt32(command.ExecuteScalar());
                    }
                    connection.Close();
                    return response;
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, publishablekey, secretkey, connectionStr);
                    return 0;
                }
            }
        }

        public List<BannerImages> GetBannerImage(int id, string bannerType, string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                List<BannerImages> objList = new List<BannerImages>();
                BannerImages obj = null;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPGetBannerImages] @Id, @BannerType", connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.Parameters.AddWithValue("@BannerType", bannerType);
                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                obj = new BannerImages();
                                obj.Id = Convert.ToInt32(reader["Id"].ToString());
                                obj.BannerType = reader["BannerType"].ToString();
                                if (string.IsNullOrEmpty(reader["ImagePath"].ToString()))
                                {
                                    obj.ImagePath = string.Empty;
                                }
                                else
                                {
                                    obj.ImagePath = ConfigurationManager.AppSettings["BackendAPPBaseUrl"].ToString() + reader["ImagePath"].ToString();
                                }               
                                obj.IsActive = Convert.ToBoolean(reader["IsActive"].ToString());
                                obj.IsDeleted = Convert.ToBoolean(reader["IsDeleted"].ToString());
                                if (reader["DeletedDate"] is DBNull)
                                {
                                    obj.DeletedDate = null;
                                }
                                else
                                {
                                    obj.DeletedDate = Convert.ToDateTime(reader["DeletedDate"].ToString());
                                }

                                obj.CreatedBy = reader["CreatedBy"].ToString();
                                obj.CreatedDate = Convert.ToDateTime(reader["CreatedDate"].ToString());

                                if (reader["UpdatedBy"] is DBNull)
                                {
                                    obj.UpdatedBy = null;
                                }
                                else
                                {
                                    obj.UpdatedBy = reader["UpdatedBy"].ToString();
                                }

                                if (reader["UpdatedDate"] is DBNull)
                                {
                                    obj.UpdatedDate = null;
                                }
                                else
                                {
                                    obj.UpdatedDate = Convert.ToDateTime(reader["UpdatedDate"].ToString());
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
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, connectionStr);
                    return new List<BannerImages>();
                }
            }
        }

        public StoreModel GetStore(string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                StoreModel obj = null;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPGetStore]", connection))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        obj = new StoreModel();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                obj.Id = Convert.ToInt32(reader["Id"].ToString());
                                obj.Name = reader["Name"].ToString();
                                obj.Email = reader["Email"].ToString();
                                obj.Mobile = reader["Mobile"].ToString();
                                obj.Address = reader["Address"].ToString();
                                obj.State = reader["State"].ToString();
                                obj.City = reader["City"].ToString();
                                obj.ZipCode = reader["ZipCode"].ToString();
                                obj.CountryCode = reader["CountryCode"] == null ? string.Empty : reader["CountryCode"].ToString().Trim();
                                if (reader["Latitude"] is DBNull)
                                {
                                    obj.Latitude = string.Empty;
                                }
                                else
                                {
                                    obj.Latitude = reader["Latitude"].ToString();
                                }
                                if (reader["Longitude"] is DBNull)
                                {
                                    obj.Longitude = string.Empty;
                                }
                                else
                                {
                                    obj.Longitude = reader["Longitude"].ToString();
                                }
                                obj.ContactPersonName = reader["ContactPersonName"].ToString();
                                obj.ContactNumber = reader["ContactNumber"].ToString();
                                if (reader["LogoPath"] is DBNull)
                                {
                                    obj.LogoPath = string.Empty;
                                }
                                else
                                {
                                    obj.LogoPath = ConfigurationManager.AppSettings["BackendAPPBaseUrl"] + reader["LogoPath"].ToString();
                                }
                                obj.GSTRegistrationNumber = reader["GSTRegistrationNumber"].ToString();
                                if (string.IsNullOrEmpty(reader["GSTFilePath"].ToString()))
                                {
                                    obj.GSTFilePath = string.Empty;
                                }
                                else
                                {
                                    obj.GSTFilePath = ConfigurationManager.AppSettings["BackendAPPBaseUrl"] + reader["GSTFilePath"].ToString();
                                }
                                obj.ActivePlan = reader["ActivePlan"].ToString();
                                obj.Commision = Convert.ToDecimal(reader["Commision"].ToString());
                                obj.PlanActiveDate = Convert.ToDateTime(reader["PlanActiveDate"].ToString());
                                obj.CurrencySymbol = reader["CurrencySymbol"] == null ? string.Empty : reader["CurrencySymbol"].ToString().Trim();
                                obj.TimeZone = reader["TimeZone"].ToString();
                                obj.MinOrderAmt = Convert.ToDecimal(reader["MinOrderAmt"].ToString());
                                obj.MaxDeliveryAreaInMiles = Convert.ToDecimal(reader["MaxDeliveryAreaInMiles"].ToString());
                                obj.IsActive = Convert.ToBoolean(reader["IsActive"].ToString());
                                obj.CreatedDate = Convert.ToDateTime(reader["CreatedDate"].ToString());
                                obj.BusinessHours = new BusinessHoursModel();
                                obj.BusinessHours.WeekDayId = Convert.ToInt32(reader["WeekDayId"].ToString());
                                obj.BusinessHours.OpenTime = reader["OpenTime"].ToString();
                                obj.BusinessHours.CloseTime = reader["CloseTime"].ToString();
                                obj.BusinessHours.OpenTime12Hour = reader["OpenTime12Hour"].ToString();
                                obj.BusinessHours.CloseTime12Hour = reader["CloseTime12Hour"].ToString();
                                obj.BusinessHours.CurrentTime = reader["CurrentTime"].ToString();
                                obj.BusinessHours.IsClosed = reader["IsClosed"] is DBNull ? false : Convert.ToBoolean(reader["IsClosed"].ToString());
                                //if (reader["PickUpStartDateTime"] is DBNull)
                                //{
                                //    obj.PickUpStartDateTime = null;
                                //}
                                //else
                                //{
                                //    obj.PickUpStartDateTime = Convert.ToDateTime(reader["PickUpStartDateTime"].ToString());
                                //}
                                //if (reader["PickUpEndDateTime"] is DBNull)
                                //{
                                //    obj.PickUpEndDateTime = null;
                                //}
                                //else
                                //{
                                //    obj.PickUpEndDateTime = Convert.ToDateTime(reader["PickUpEndDateTime"].ToString());
                                //}


                                //obj.PickUpDateJsonStr = reader["PickUpDateJsonStr"] is DBNull ? string.Empty : reader["PickUpDateJsonStr"].ToString();
                                //obj.PickUpTimeJsonStr = reader["PickUpTimeJsonStr"] is DBNull ? string.Empty : reader["PickUpTimeJsonStr"].ToString();

                                //obj.PickUpDateList = JsonConvert.DeserializeObject<List<PickUpDateVM>>("[" + reader["PickUpDateJsonStr"].ToString() + "]");

                                //obj.PickUpTimeList = JsonConvert.DeserializeObject<List<PickUpTimeVM>>("[" + reader["PickUpTimeJsonStr"].ToString() + "]");


                                obj.TaxRate = Convert.ToDecimal(reader["TaxRate"].ToString());
                                obj.DeliveryCharges = Convert.ToDecimal(reader["DeliveryCharges"].ToString());
                                obj.CashOnDeliveryEnable = Convert.ToBoolean(reader["CashOnDeliveryEnable"].ToString());

                                obj.PlanId = reader["PlanId"] is DBNull ? 0 : Convert.ToInt32(reader["PlanId"].ToString());
                                obj.IsSubscriptionCancelled = reader["IsSubscriptionCancelled"] is DBNull ? false : Convert.ToBoolean(reader["IsSubscriptionCancelled"].ToString());
                                if (reader["SubscriptionCancelledOn"] is DBNull)
                                {
                                    obj.SubscriptionCancelledOn = null;
                                }
                                else
                                {
                                    obj.SubscriptionCancelledOn = Convert.ToDateTime(reader["SubscriptionCancelledOn"].ToString());
                                }

                                obj.StripePublishablekey = reader["StripePublishablekey"] is DBNull ? string.Empty : reader["StripePublishablekey"].ToString();
                                obj.StripeSecretkey = reader["StripeSecretkey"] is DBNull ? string.Empty : reader["StripeSecretkey"].ToString();

                                obj.BusinessHourList = new BusinessHoursBusiness().GetBusinessHours(connectionStr);
                                obj.QrCodePath = reader["QrCodePath"] is DBNull || reader["QrCodePath"].ToString() == string.Empty ? string.Empty : ConfigurationManager.AppSettings["BackendAPPBaseUrl"].ToString() + reader["QrCodePath"].ToString();
                                return obj;
                            }
                        }
                    }
                    connection.Close();
                    return obj;
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, connectionStr);
                    return new StoreModel();
                }
            }
        }

        public int MarkAsActiveInActive(int id, bool status, string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                int response = 0;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPBannerChangeStatus] @Id, @Status, @Result", connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.Parameters.AddWithValue("@Status", status);
                        command.Parameters.Add("@Result", SqlDbType.Int);
                        command.Parameters["@Result"].Direction = ParameterDirection.Output;
                        response = Convert.ToInt32(command.ExecuteScalar());
                    }
                    connection.Close();
                    return response;
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, id, status, connectionStr);
                    return 0;
                }
            }
        }

        public int UpdateDeliveryAreaSetting(decimal minOrderedAmt, decimal deliveryAreaMiles, string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                int response = 0;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPAddUpdateDeliveryAreaSetting] @MinOrderedAmt, @DeliveryAreaMiles, @Result", connection))
                    {
                        command.Parameters.AddWithValue("@MinOrderedAmt", minOrderedAmt);
                        command.Parameters.AddWithValue("@DeliveryAreaMiles", deliveryAreaMiles);
                        command.Parameters.Add("@Result", SqlDbType.Int);
                        command.Parameters["@Result"].Direction = ParameterDirection.Output;
                        response = Convert.ToInt32(command.ExecuteScalar());
                    }
                    connection.Close();
                    return response;
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, minOrderedAmt, deliveryAreaMiles, connectionStr);
                    return 0;
                }
            }
        }

        public int UpdateDeliveryChargesTaxes(decimal deliveryCharges, decimal tax, bool isEnableCashOnDelivery, string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                int response = 0;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPAddUpdateDeliveryChargesTaxes] @DeliveryCharges, @Tax, @IsEnableCashOnDelivery, @Result", connection))
                    {
                        command.Parameters.AddWithValue("@DeliveryCharges", deliveryCharges);
                        command.Parameters.AddWithValue("@Tax", tax);
                        command.Parameters.AddWithValue("@IsEnableCashOnDelivery", isEnableCashOnDelivery);
                        command.Parameters.Add("@Result", SqlDbType.Int);
                        command.Parameters["@Result"].Direction = ParameterDirection.Output;
                        response = Convert.ToInt32(command.ExecuteScalar());
                    }
                    connection.Close();
                    return response;
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, deliveryCharges, tax, isEnableCashOnDelivery, connectionStr);
                    return 0;
                }
            }
        }

        public int MarkBannerAsDeleted(int id, string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                int response = 0;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPSetIsDeletedBanner] @Id, @Result", connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.Parameters.Add("@Result", SqlDbType.Int);
                        command.Parameters["@Result"].Direction = ParameterDirection.Output;
                        response = Convert.ToInt32(command.ExecuteScalar());
                    }
                    connection.Close();
                    return response;
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, id, connectionStr);
                    return 0;
                }
            }
        }
    }
}
