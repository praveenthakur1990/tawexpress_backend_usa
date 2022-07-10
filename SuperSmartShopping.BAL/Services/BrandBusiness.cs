using Newtonsoft.Json;
using SuperSmartShopping.BAL.Interfaces;
using SuperSmartShopping.DAL.Common;
using SuperSmartShopping.DAL.Models;
using SuperSmartShopping.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.BAL.Services
{
    public class BrandBusiness : IBrandBusiness
    {
        public int AddUpdate(BrandModel model, string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                int response = 0;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPAddUpdateBrand] @Id, @Name, @CreatedBy, @Result", connection))
                    {
                        command.Parameters.AddWithValue("@Id", model.Id);
                        command.Parameters.AddWithValue("@Name", model.Name);
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


        public List<BrandModel> GetBrands(int productId, PaginationModel pageModel, string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                List<BrandModel> objList = new List<BrandModel>();
                BrandModel obj = null;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPGetBrands] @Id, @PageNumber, @PageSize, @SearchStr", connection))
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
                                obj = new BrandModel();
                                obj.Id = Convert.ToInt32(reader["Id"].ToString());
                                obj.Name = reader["Name"].ToString();
                                obj.IsActive = Convert.ToBoolean(reader["IsActive"].ToString());
                                obj.IsDeleted = Convert.ToBoolean(reader["IsDeleted"].ToString());
                                obj.CreatedBy = reader["CreatedBy"].ToString();
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
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, productId, JsonConvert.SerializeObject(pageModel), connectionStr);
                    return new List<BrandModel>();
                }
            }
        }

        public List<BrandModel> GetBrandsForAPP(int categoryId, string subCategoryIds, string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                List<BrandModel> objList = new List<BrandModel>();
                BrandModel obj = null;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPGetBrandByCategoryId] @categoryId, @SubCategoryIds", connection))
                    {
                        command.Parameters.AddWithValue("@categoryId", categoryId);
                        command.Parameters.AddWithValue("@SubCategoryIds", subCategoryIds);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                obj = new BrandModel();
                                obj.Id = Convert.ToInt32(reader["Id"].ToString());
                                obj.Name = reader["Name"].ToString();
                                obj.TotalProductCount = Convert.ToInt32(reader["TotalProductCount"].ToString());
                                obj.Checked = false;
                                objList.Add(obj);
                            }
                        }
                    }
                    connection.Close();
                    return objList;
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, categoryId, subCategoryIds, connectionStr);
                    return new List<BrandModel>();
                }
            }
        }
    }
}
