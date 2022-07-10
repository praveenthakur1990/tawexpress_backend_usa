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
    public class SpecialOfferBusiness : ISpecialOfferBusiness
    {
        public int AddUpdate(SpecialOfferModel model, string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                int response = 0;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPAddUpdateSpecialOffer] @Id, @Title, @StartDate, @EndDate, @BannerImagePath, @CreatedBy, @Result", connection))
                    {
                        command.Parameters.AddWithValue("@Id", model.Id);
                        command.Parameters.AddWithValue("@Title", model.Title);
                        command.Parameters.AddWithValue("@StartDate", model.StartDate);
                        command.Parameters.AddWithValue("@EndDate", model.EndDate);
                        command.Parameters.AddWithValue("@BannerImagePath", model.BannerImagePath);
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

        public List<SpecialOfferModel> Get(int id, PaginationModel model, string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                List<SpecialOfferModel> objList = new List<SpecialOfferModel>();
                SpecialOfferModel obj = null;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPGetSpecialOffer] @Id, @PageNumber, @PageSize, @SearchStr", connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.Parameters.AddWithValue("@PageNumber", model.PageNumber == 0 ? 1 : model.PageNumber);
                        command.Parameters.AddWithValue("@PageSize", model.PageSize);
                        command.Parameters.AddWithValue("@SearchStr", model.SearchStr == null ? string.Empty : model.SearchStr);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                obj = new SpecialOfferModel();
                                obj.Id = Convert.ToInt32(reader["Id"].ToString());
                                obj.Title = reader["Title"].ToString();
                                obj.StartDate = Convert.ToDateTime(reader["StartDate"].ToString());
                                obj.EndDate = Convert.ToDateTime(reader["EndDate"].ToString());
                                obj.IsActive = Convert.ToBoolean(reader["IsActive"].ToString());
                                obj.IsDeleted = Convert.ToBoolean(reader["IsDeleted"].ToString());
                                obj.CreatedBy = reader["CreatedBy"].ToString();
                                obj.CreatedDate = Convert.ToDateTime(reader["CreatedDate"].ToString());
                                if (reader["DeletedDate"] is DBNull)
                                {
                                    obj.DeletedDate = null;
                                }
                                else
                                {
                                    obj.DeletedDate = Convert.ToDateTime(reader["DeletedDate"].ToString());
                                }

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

                                obj.TotalRows = Convert.ToInt32(reader["TotalRows"].ToString());
                                if (reader["BannerImagePath"] is DBNull)
                                {
                                    obj.BannerImagePath = null;
                                }
                                else
                                {
                                    obj.BannerImagePath = reader["BannerImagePath"].ToString();
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
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, id, JsonConvert.SerializeObject(model), connectionStr);
                    return new List<SpecialOfferModel>();
                }
            }
        }

        public List<ProductDashboardModel> GetProductBySpecialOfferId(string categoryId, int specialOfferId, string connectionStr)
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
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPGetProductBySpecialOfferId] @CategoryId, @SpecialOfferId", connection))
                    {
                        command.Parameters.AddWithValue("@CategoryId", !string.IsNullOrEmpty(categoryId) ? categoryId : string.Empty);
                        command.Parameters.AddWithValue("@SpecialOfferId", specialOfferId);
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
                                obj.SpecialOfferId = Convert.ToInt32(reader["SpecialOfferId"].ToString());
                                obj.OfferType = reader["OfferTypeId"] is DBNull ? string.Empty : reader["OfferTypeId"].ToString();
                                obj.FinalOfferValue = reader["FinalOfferValue"] is DBNull ? string.Empty : reader["FinalOfferValue"].ToString();
                                obj.OfferValue = Convert.ToDecimal(reader["offerValue"].ToString());
                                obj.FinalValue = reader["finalvalue"] is DBNull ? string.Empty : reader["finalvalue"].ToString();

                                obj.SpecialOfferCatId = Convert.ToInt32(reader["SpecialOfferCatId"].ToString());
                                obj.SpecialOfferProductId = Convert.ToInt32(reader["SpecialOfferProductId"].ToString());
                                objList.Add(obj);
                            }
                        }
                    }
                    connection.Close();
                    return objList;
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, categoryId, specialOfferId, connectionStr);
                    return new List<ProductDashboardModel>();
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
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPAddUpdateSpecialOfferProduct] @SpecialOfferCatId, @SpecialOfferId, @CategoryId, @SubCategoryId, @ProductList, @Result", connection))
                    {
                        command.Parameters.AddWithValue("@SpecialOfferCatId", model.WeeklyCircularCatId);
                        command.Parameters.AddWithValue("@SpecialOfferId", model.WeeklyCircularId);
                        command.Parameters.AddWithValue("@CategoryId", model.CategoryId);
                        command.Parameters.AddWithValue("@SubCategoryId", model.SubCategoryId);
                        command.Parameters.Add("@ProductList", SqlDbType.Structured).Value = model.ProductListDataTable;
                        command.Parameters["@ProductList"].TypeName = "dbo.tb_SpecialOfferProducts_UDT";
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

        public List<SpecialOfferDatesModel> GetSpecialOfferDates(string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                List<SpecialOfferDatesModel> objList = new List<SpecialOfferDatesModel>();
                SpecialOfferDatesModel obj = null;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPGetSpecialOfferDatesAPP]", connection))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                obj = new SpecialOfferDatesModel();
                                obj.Id = Convert.ToInt32(reader["Id"].ToString());
                                obj.SpecialOfferDates = reader["SpecialOfferDate"].ToString();
                                if (reader["BannerImagePath"] is DBNull)
                                {
                                    obj.BannerImagePath = null;
                                }
                                else
                                {
                                    obj.BannerImagePath = ConfigurationManager.AppSettings["BackendAPPBaseUrl"].ToString() + reader["BannerImagePath"].ToString();
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
                    return new List<SpecialOfferDatesModel>();
                }
            }
        }                
    }
}
