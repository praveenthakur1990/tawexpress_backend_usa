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
    public class ProductBusiness : IProductBusiness
    {
        public int AddUpdate(Product model, string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                int response = 0;
                try
                {

                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPAddUpdateProduct] @Id, @CategoryId, @SubCategoryId, @BrandId, @UnitMeasurementId, @Name, @IsVariants, @Price, @ImagePath, @Description, @IsDescriptionShow, @MarkItemAs, @VegNonVeg, @IsPublished, @TagIds, @CreatedBy, @Result", connection))
                    {
                        command.Parameters.AddWithValue("@Id", model.Id);
                        command.Parameters.AddWithValue("@CategoryId", model.CategoryId);
                        command.Parameters.AddWithValue("@SubCategoryId", model.SubCategoryId);
                        command.Parameters.AddWithValue("@BrandId", model.BrandId);
                        command.Parameters.AddWithValue("@UnitMeasurementId", model.UnitMeasurementId);
                        command.Parameters.AddWithValue("@Name", model.Name);
                        command.Parameters.AddWithValue("@IsVariants", model.IsVariants);
                        command.Parameters.AddWithValue("@Price", model.Price);
                        command.Parameters.AddWithValue("@ImagePath", model.Image ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Description", model.Description ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@IsDescriptionShow", model.IsDescriptionShow);
                        command.Parameters.AddWithValue("@MarkItemAs", model.MarkItemAs);
                        command.Parameters.AddWithValue("@VegNonVeg", model.VegNonVeg ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@IsPublished", model.IsPublished);
                        command.Parameters.AddWithValue("@TagIds", model.TagIds);
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

        public List<ProductDashboardModel> GetProductForDashboard(int categoryId, int limit, string connectionStr)
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
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPGetProductForDashBoard_v1] @CategoryId, @Limit", connection))
                    {
                        command.Parameters.AddWithValue("@CategoryId", categoryId);
                        command.Parameters.AddWithValue("@Limit", limit);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                obj = new ProductDashboardModel();
                                obj.Id = Convert.ToInt32(reader["Id"].ToString());
                                obj.CategoryId = Convert.ToInt32(reader["CategoryId"].ToString());
                                obj.Description = reader["Description"].ToString();
                                if (reader["IsDescriptionShow"] is DBNull)
                                {
                                    obj.IsDescriptionShow = false;
                                }
                                else
                                {
                                    obj.IsDescriptionShow = Convert.ToBoolean(reader["IsDescriptionShow"].ToString());
                                }

                                obj.ProductName = reader["Name"].ToString();
                                obj.Price = Convert.ToDecimal(reader["Price"].ToString());
                                obj.ProductImage = reader["ProductImage"] is DBNull ? string.Empty : ConfigurationManager.AppSettings["BackendAPPBaseUrl"].ToString() + reader["ProductImage"].ToString();
                                obj.CategoryName = reader["CategoryName"].ToString();
                                obj.CategoryImage = reader["CategoryImage"] is DBNull ? string.Empty : ConfigurationManager.AppSettings["BackendAPPBaseUrl"].ToString() + reader["CategoryImage"].ToString();
                                obj.IsVarient = Convert.ToBoolean(reader["IsVariants"].ToString());
                                if (obj.IsVarient)
                                {
                                    obj.ProductVarients = new ProductVarientBusiness().GetProductsVarients(0, obj.Id, pageModel, connectionStr).OrderBy(c => c.Price).ToList();
                                }
                                obj.DefaultVarientId = Convert.ToInt32(reader["DefaultVarientId"].ToString());
                                obj.TagIds = reader["TagIds"] is DBNull ? string.Empty : reader["TagIds"].ToString();

                                if (reader["OfferTypeId"] is DBNull)
                                {
                                    obj.OfferType = null;
                                }
                                else
                                {
                                    obj.OfferType = reader["OfferTypeId"].ToString();
                                }

                                if (reader["OfferValue"] is DBNull)
                                {
                                    obj.OfferValue = 0;
                                }
                                else
                                {
                                    obj.OfferValue = Convert.ToDecimal(reader["OfferValue"].ToString());
                                }
                                if (reader["FinalOfferValue"] is DBNull)
                                {
                                    obj.FinalOfferValue = null;
                                }
                                else
                                {
                                    obj.FinalOfferValue = reader["FinalOfferValue"].ToString();
                                }
                                if (reader["FinalValue"] is DBNull)
                                {
                                    obj.FinalValue = null;
                                }
                                else
                                {
                                    obj.FinalValue = reader["FinalValue"].ToString();
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
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, categoryId, limit, connectionStr);
                    return new List<ProductDashboardModel>();
                }
            }
        }

        public List<ProductModel> GetProducts(int productId, PaginationModel pageModel, string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                List<ProductModel> objList = new List<ProductModel>();
                ProductModel obj = null;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPGetProducts] @Id, @PageNumber, @PageSize, @SearchStr", connection))
                    {
                        command.Parameters.AddWithValue("@Id", productId);
                        command.Parameters.AddWithValue("@PageNumber", pageModel.PageNumber == 0 ? 1 : pageModel.PageNumber);
                        command.Parameters.AddWithValue("@PageSize", pageModel.PageSize);
                        command.Parameters.AddWithValue("@SearchStr", pageModel.SearchStr == null ? string.Empty : pageModel.SearchStr);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                obj = new ProductModel();
                                obj.Id = Convert.ToInt32(reader["Id"].ToString());
                                obj.CategoryId = Convert.ToInt32(reader["CategoryId"].ToString());
                                obj.SubCategoryId = Convert.ToInt32(reader["SubCategoryId"].ToString());
                                obj.BrandId = reader["BrandId"] is DBNull ? 0 : Convert.ToInt32(reader["BrandId"].ToString());
                                obj.UnitMeasurementId = reader["UnitMeasurementId"] is DBNull ? 0 : Convert.ToInt32(reader["UnitMeasurementId"].ToString());
                                obj.Name = reader["Name"].ToString();
                                obj.IsVariants = Convert.ToBoolean(reader["IsVariants"].ToString());
                                obj.Price = Convert.ToDecimal(reader["Price"].ToString());
                                obj.Image = ConfigurationManager.AppSettings["BackendAPPBaseUrl"].ToString() + reader["Image"].ToString();
                                obj.Description = reader["Description"].ToString();
                                obj.MarkItemAs = reader["MarkItemAs"].ToString();

                                obj.VegNonVeg = reader["VegNonVeg"].ToString();
                                obj.IsPublished = Convert.ToBoolean(reader["IsPublished"].ToString());
                                obj.CreatedDate = Convert.ToDateTime(reader["CreatedDate"].ToString());
                                obj.CreatedBy = reader["CreatedBy"].ToString();
                                obj.TotalRows = Convert.ToInt32(reader["TotalRows"].ToString());

                                obj.CategoryName = reader["CategoryName"] is DBNull ? "--" : reader["CategoryName"].ToString();
                                obj.SubCategoryName = reader["SubCategoryName"] is DBNull ? "--" : reader["SubCategoryName"].ToString();
                                obj.BrandName = reader["BrandName"] is DBNull ? "--" : reader["BrandName"].ToString();
                                obj.UnitMeasurementName = reader["UnitMeasurementName"] is DBNull ? "--" : reader["UnitMeasurementName"].ToString();
                                obj.StockInHand = Convert.ToInt32(reader["StockInHand"].ToString());
                                obj.Description = reader["Description"].ToString();
                                if (reader["IsDescriptionShow"] is DBNull)
                                {
                                    obj.IsDescriptionShow = false;
                                }
                                else
                                {
                                    obj.IsDescriptionShow = Convert.ToBoolean(reader["IsDescriptionShow"].ToString());
                                }
                                obj.IsCategoryActive = reader["IsCategoryActive"] is DBNull ? true : Convert.ToBoolean(reader["IsCategoryActive"].ToString());
                                obj.TagIds = reader["TagIds"] is DBNull ? string.Empty : reader["TagIds"].ToString();
                                objList.Add(obj);
                            }
                        }
                    }
                    connection.Close();
                    return objList;
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, productId, JsonConvert.SerializeObject(pageModel), connectionStr);
                    return new List<ProductModel>();
                }
            }
        }

        public int MarkProductAsDeleted(int productId, string connectionStr)
        {
            throw new NotImplementedException();
        }

        public List<ProductDashboardModel> GetProductByCategoryIdAPP(string categoryId, string subCategoryIds, string brandIds, string connectionStr)
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
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPGetProductByCategoryId_v1] @CategoryId, @SubCategoryIds, @BrandIds", connection))
                    {
                        command.Parameters.AddWithValue("@CategoryId", !string.IsNullOrEmpty(categoryId) ? categoryId : string.Empty);
                        command.Parameters.AddWithValue("@SubCategoryIds", !string.IsNullOrEmpty(subCategoryIds) ? subCategoryIds : string.Empty);
                        command.Parameters.AddWithValue("@BrandIds", !string.IsNullOrEmpty(brandIds) ? brandIds : string.Empty);
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

                                if (reader["OfferTypeId"] is DBNull)
                                {
                                    obj.OfferType = null;
                                }
                                else
                                {
                                    obj.OfferType = reader["OfferTypeId"].ToString();
                                }

                                if (reader["OfferValue"] is DBNull)
                                {
                                    obj.OfferValue = 0;
                                }
                                else
                                {
                                    obj.OfferValue = Convert.ToDecimal(reader["OfferValue"].ToString());
                                }
                                if (reader["FinalOfferValue"] is DBNull)
                                {
                                    obj.FinalOfferValue = null;
                                }
                                else
                                {
                                    obj.FinalOfferValue = reader["FinalOfferValue"].ToString();
                                }
                                if (reader["FinalValue"] is DBNull)
                                {
                                    obj.FinalValue = null;
                                }
                                else
                                {
                                    obj.FinalValue = reader["FinalValue"].ToString();
                                }
                                if (string.IsNullOrEmpty(reader["BannerImg"].ToString()))
                                {
                                    obj.BannerImg = string.Empty;
                                }
                                else
                                {
                                    obj.BannerImg = ConfigurationManager.AppSettings["BackendAPPBaseUrl"].ToString() + reader["BannerImg"].ToString();
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
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, categoryId, subCategoryIds, brandIds, connectionStr);
                    return new List<ProductDashboardModel>();
                }
            }
        }

        public List<ProductDashboardModel> GetRelatedProductByIdAPP(int productId, int limit, string connectionStr)
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
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPGetRelatedProduct] @ProductId, @Limit", connection))
                    {
                        command.Parameters.AddWithValue("@ProductId", productId);
                        command.Parameters.AddWithValue("@Limit", limit);
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
                                obj.ProductImage = reader["ProductImage"] is DBNull ? string.Empty : ConfigurationManager.AppSettings["BackendAPPBaseUrl"].ToString() + reader["ProductImage"].ToString();
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
                                objList.Add(obj);
                            }
                        }
                    }
                    connection.Close();
                    return objList;
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, productId, limit, connectionStr);
                    return new List<ProductDashboardModel>();
                }
            }
        }
    }
}
