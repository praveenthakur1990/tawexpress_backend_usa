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

namespace SuperSmartShopping.BAL.Services
{
    public class WeeklyCircularBusiness : IWeeklyCircularBusiness
    {
        public int AddUpdate(WeeklyCircularModel model, string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                int response = 0;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPAddUpdateWeeklyCircular] @Id, @Title, @StartDate, @EndDate, @PdfFilePath, @ThumbnailImgPath, @CreatedBy, @Result", connection))
                    {
                        command.Parameters.AddWithValue("@Id", model.Id);
                        command.Parameters.AddWithValue("@Title", model.Title);
                        command.Parameters.AddWithValue("@StartDate", model.StartDate);
                        command.Parameters.AddWithValue("@EndDate", model.EndDate);
                        command.Parameters.AddWithValue("@PdfFilePath", !string.IsNullOrEmpty(model.PdfFilePath) ? model.PdfFilePath : string.Empty);
                        command.Parameters.AddWithValue("@ThumbnailImgPath", !string.IsNullOrEmpty(model.ThumbnailImgPath) ? model.ThumbnailImgPath : string.Empty);
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
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, model, connectionStr);
                    return 0;
                }
            }
        }
        public int AddUpdateProduct(WeeklyCircularCatInfoModel model, string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                int response = 0;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPAddUpdateWeeklyCircularProduct] @WeeklyCircularCatId, @WeeklyCircularId, @CategoryId, @SubCategoryId, @ProductList, @Result", connection))
                    {
                        command.Parameters.AddWithValue("@WeeklyCircularCatId", model.WeeklyCircularCatId);
                        command.Parameters.AddWithValue("@WeeklyCircularId", model.WeeklyCircularId);
                        command.Parameters.AddWithValue("@CategoryId", model.CategoryId);
                        command.Parameters.AddWithValue("@SubCategoryId", model.SubCategoryId);
                        command.Parameters.Add("@ProductList", SqlDbType.Structured).Value = model.ProductListDataTable;
                        command.Parameters["@ProductList"].TypeName = "dbo.tb_WeeklyCircularProducts_UDT";
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
        public List<WeeklyCircularModel> Get(int id, PaginationModel pageModel, string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                List<WeeklyCircularModel> objList = new List<WeeklyCircularModel>();
                WeeklyCircularModel obj = null;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPGetWeeklyCircular] @Id, @PageNumber, @PageSize, @SearchStr", connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.Parameters.AddWithValue("@PageNumber", pageModel.PageNumber == 0 ? 1 : pageModel.PageNumber);
                        command.Parameters.AddWithValue("@PageSize", pageModel.PageSize);
                        command.Parameters.AddWithValue("@SearchStr", pageModel.SearchStr == null ? string.Empty : pageModel.SearchStr);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                obj = new WeeklyCircularModel();
                                obj.Id = Convert.ToInt32(reader["Id"].ToString());
                                obj.Title = reader["Title"].ToString();
                                obj.StartDate = Convert.ToDateTime(reader["StartDate"].ToString());
                                obj.EndDate = Convert.ToDateTime(reader["EndDate"].ToString());
                                obj.PdfFilePath = !string.IsNullOrEmpty(reader["PdfFilePath"].ToString()) ? ConfigurationManager.AppSettings["BackendAPPBaseUrl"].ToString() + reader["PdfFilePath"].ToString() : string.Empty;
                                obj.ThumbnailImgPath = !string.IsNullOrEmpty(reader["ThumbnailImgPath"].ToString()) ? ConfigurationManager.AppSettings["BackendAPPBaseUrl"].ToString() + reader["ThumbnailImgPath"].ToString() : string.Empty;
                                obj.IsActive = Convert.ToBoolean(reader["IsActive"].ToString());
                                obj.CreatedDate = Convert.ToDateTime(reader["CreatedDate"].ToString());
                                obj.TotalRows = Convert.ToInt32(reader["TotalRows"].ToString());
                                objList.Add(obj);
                            }
                        }
                    }
                    connection.Close();
                    return objList;
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, id, JsonConvert.SerializeObject(pageModel), connectionStr);
                    return new List<WeeklyCircularModel>();
                }
            }
        }
        public List<ProductDashboardModel> GetProductByWeeklyCircularId(string categoryId, int weeklyCircularId, string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                List<ProductDashboardModel> objList = new List<ProductDashboardModel>();
                ProductDashboardModel obj = null;
                try
                {
                    PaginationModel pageModel = new PaginationModel();
                    pageModel.PageNumber = 1;
                    pageModel.PageSize = -1;
                    pageModel.SearchStr = string.Empty;
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPGetProductByWeeklyCircularId] @CategoryId, @WeeklyCircularId", connection))
                    {
                        command.Parameters.AddWithValue("@CategoryId", !string.IsNullOrEmpty(categoryId) ? categoryId : string.Empty);
                        command.Parameters.AddWithValue("@WeeklyCircularId", weeklyCircularId);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                obj = new ProductDashboardModel();
                                obj.Id = Convert.ToInt32(reader["Id"].ToString());
                                obj.CategoryId = Convert.ToInt32(reader["CategoryId"].ToString());
                                obj.SubCategoryId = Convert.ToInt32(reader["SubCategoryId"].ToString());
                                obj.ProductName = reader["Name"].ToString();
                                obj.Price = Convert.ToDecimal(reader["Price"].ToString());
                                obj.ProductImage = reader["Image"] is DBNull ? string.Empty : ConfigurationManager.AppSettings["BackendAPPBaseUrl"].ToString() + reader["Image"].ToString();
                                obj.CategoryName = reader["CategoryName"].ToString();
                                obj.SubCategoryName = reader["SubCategoryName"].ToString();
                                //obj.CategoryImage = reader["CategoryImage"] is DBNull ? string.Empty : ConfigurationManager.AppSettings["BackendAPPBaseUrl"].ToString() + reader["CategoryImage"].ToString();
                                obj.IsVarient = Convert.ToBoolean(reader["IsVariants"].ToString());
                                if (obj.IsVarient)
                                {
                                    obj.ProductVarients = new ProductVarientBusiness().GetProductsVarients(0, obj.Id, pageModel, connectionStr).OrderBy(c => c.Price).ToList();
                                }
                                obj.DefaultVarientId = Convert.ToInt32(reader["DefaultVarientId"].ToString());
                                obj.Description = reader["Description"].ToString();
                                if (reader["IsDescriptionShow"] is DBNull)
                                {
                                    obj.IsDescriptionShow = false;
                                }
                                else
                                {
                                    obj.IsDescriptionShow = Convert.ToBoolean(reader["IsDescriptionShow"].ToString());
                                }
                                obj.TagIds = reader["TagIds"] is DBNull ? string.Empty : reader["TagIds"].ToString();
                                obj.WeeklyCircularId = Convert.ToInt32(reader["WeeklyCircularId"].ToString());
                                obj.OfferType = reader["OfferTypeId"] is DBNull ? string.Empty : reader["OfferTypeId"].ToString();
                                obj.FinalOfferValue = reader["FinalOfferValue"] is DBNull ? string.Empty : reader["FinalOfferValue"].ToString();
                                obj.OfferValue = Convert.ToDecimal(reader["offerValue"].ToString());
                                obj.FinalValue = reader["finalvalue"] is DBNull ? string.Empty : reader["finalvalue"].ToString();

                                obj.WeeklyCircularCatId = Convert.ToInt32(reader["WeeklyCircularCatId"].ToString());
                                obj.WeeklyCircularProductId = Convert.ToInt32(reader["WeeklyCircularProductId"].ToString());
                                objList.Add(obj);
                            }
                        }
                    }
                    connection.Close();
                    return objList;
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, categoryId, weeklyCircularId, connectionStr);
                    return new List<ProductDashboardModel>();
                }
            }
        }
        public List<WeeklyCircularDatesModel> GetWeeklyCircularDates(string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                List<WeeklyCircularDatesModel> objList = new List<WeeklyCircularDatesModel>();
                WeeklyCircularDatesModel obj = null;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPGetWeeklyCircularDatesAPP]", connection))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                obj = new WeeklyCircularDatesModel();
                                obj.Id = Convert.ToInt32(reader["Id"].ToString());
                                obj.WeeklyCircularDates = reader["WeeklyCircularDate"].ToString();
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
                    return new List<WeeklyCircularDatesModel>();
                }
            }
        }
        public List<WeeklyCircularSubscriberModel> GetWeeklyCircularSubscriber(string mobileNumber, PaginationModel pageModel, string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                List<WeeklyCircularSubscriberModel> objList = new List<WeeklyCircularSubscriberModel>();
                WeeklyCircularSubscriberModel obj = null;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPGetWeeklyCircularSubscribers] @MobileNumber, @PageNumber, @PageSize, @SearchStr", connection))
                    {
                        command.Parameters.AddWithValue("@MobileNumber", !string.IsNullOrEmpty(mobileNumber) ? mobileNumber : string.Empty);
                        command.Parameters.AddWithValue("@PageNumber", pageModel.PageNumber);
                        command.Parameters.AddWithValue("@PageSize", pageModel.PageSize);
                        command.Parameters.AddWithValue("@SearchStr", !string.IsNullOrEmpty(pageModel.SearchStr) ? pageModel.SearchStr : string.Empty);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                obj = new WeeklyCircularSubscriberModel();
                                obj.Id = Convert.ToInt32(reader["Id"].ToString());
                                obj.FullName = reader["FullName"].ToString();
                                obj.MobileNumber = reader["MobileNumber"].ToString();
                                obj.Otp = reader["Otp"].ToString();
                                if (reader["OtpGenerateDate"] is DBNull)
                                {
                                    obj.OtpGenerateDate = null;
                                }
                                else
                                {
                                    obj.OtpGenerateDate = Convert.ToDateTime(reader["OtpGenerateDate"].ToString());
                                }

                                obj.CreatedDate = Convert.ToDateTime(reader["CreatedDate"].ToString());
                                obj.IsActive = Convert.ToBoolean(reader["IsActive"].ToString());
                                obj.TotalRows = Convert.ToInt32(reader["TotalRows"].ToString());
                                objList.Add(obj);
                            }
                        }
                    }
                    connection.Close();
                    return objList;
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, mobileNumber, JsonConvert.SerializeObject(pageModel), connectionStr);
                    return new List<WeeklyCircularSubscriberModel>();
                }
            }
        }
        public int AddUpdateSubscriber(WeeklyCircularSubscriberModel model, string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                int response = 0;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPAddWeeklyCircularSubscriber] @Id, @FullName, @MobileNumber, @OTP, @Result", connection))
                    {
                        command.Parameters.AddWithValue("@Id", model.Id);
                        command.Parameters.AddWithValue("@FullName", model.FullName);
                        command.Parameters.AddWithValue("@MobileNumber", model.MobileNumber);
                        command.Parameters.AddWithValue("@OTP", !string.IsNullOrEmpty(model.Otp) ? model.Otp : string.Empty);
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
        public int UpdateSubscriberOTP(string mobileNumber, string otp, string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                int response = 0;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPUpdateSubscriberOTP] @MobileNumber, @OTP, @Result", connection))
                    {
                        command.Parameters.AddWithValue("@MobileNumber", mobileNumber);
                        command.Parameters.AddWithValue("@OTP", otp);
                        command.Parameters.Add("@Result", SqlDbType.Int);
                        command.Parameters["@Result"].Direction = ParameterDirection.Output;
                        response = Convert.ToInt32(command.ExecuteScalar());
                    }
                    connection.Close();
                    return response;
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, mobileNumber, otp, connectionStr);
                    return 0;
                }
            }
        }
        public Dictionary<string, string> GetSubscriberPhoneNumber(string[] subscriberIds, string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                Dictionary<string, string> obj = new Dictionary<string, string>();
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPGetSubscriberPhoneNumber] @Ids", connection))
                    {
                        command.Parameters.AddWithValue("@Ids", string.Join(",", subscriberIds));
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                obj.Add(reader["MobileNumber"].ToString(), reader["FullName"].ToString());
                            }
                        }
                    }
                    connection.Close();
                    return obj;
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, subscriberIds, connectionStr);
                    return obj;
                }
            }
        }
    }
}
