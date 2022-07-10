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
    public class CategoryBusiness : ICategoryBusiness
    {
        public int AddUpdateCategory(Category model, string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                int response = 0;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPAddUpdateCategory] @Id, @Name, @Description, @ImagePath, @BannerImgPath, @PriorityIndex, @CreatedBy, @ParentId, @Result", connection))
                    {
                        command.Parameters.AddWithValue("@Id", model.Id);
                        command.Parameters.AddWithValue("@Name", model.Name);
                        command.Parameters.AddWithValue("@Description", model.Description);
                        command.Parameters.AddWithValue("@ImagePath", model.ImagePath);
                        command.Parameters.AddWithValue("@BannerImgPath", model.BannerImg);
                        command.Parameters.AddWithValue("@PriorityIndex", model.PriorityIndex);
                        command.Parameters.AddWithValue("@CreatedBy", model.CreatedBy);
                        command.Parameters.AddWithValue("@ParentId", model.ParentId);
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

        public List<Category> GetCategories(string type, string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                List<Category> objCategoryList = new List<Category>();
                Category obj = null;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPGetCategories] @Type", connection))
                    {
                        command.Parameters.AddWithValue("@Type", type == null ? string.Empty : type);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                obj = new Category();
                                obj.Id = Convert.ToInt32(reader["Id"].ToString());
                                obj.Name = reader["Name"].ToString();
                                obj.Description = reader["Description"].ToString();
                                obj.PriorityIndex = Convert.ToInt32(reader["PriorityIndex"].ToString());
                                if (string.IsNullOrEmpty(reader["ImagePath"].ToString()))
                                {
                                    obj.ImagePath = string.Empty;
                                }
                                else
                                {
                                    obj.ImagePath = ConfigurationManager.AppSettings["BackendAPPBaseUrl"].ToString() + reader["ImagePath"].ToString();
                                }
                                if (string.IsNullOrEmpty(reader["BannerImg"].ToString()))
                                {
                                    obj.BannerImg = string.Empty;
                                }
                                else
                                {
                                    obj.BannerImg = ConfigurationManager.AppSettings["BackendAPPBaseUrl"].ToString() + reader["BannerImg"].ToString();
                                }
                                obj.IsActive = Convert.ToBoolean(reader["IsActive"].ToString());
                                obj.CreatedDate = Convert.ToDateTime(reader["CreatedDate"].ToString());
                                if (reader["UpdatedDate"] is DBNull)
                                {
                                    obj.UpdatedDate = null;
                                }
                                else
                                {
                                    obj.UpdatedDate = Convert.ToDateTime(reader["UpdatedDate"].ToString());
                                }
                                obj.ParentId = Convert.ToInt32(reader["ParentId"].ToString());
                                obj.IsDeleted = reader["IsDeleted"] == null ? false : Convert.ToBoolean(reader["IsDeleted"].ToString());
                                objCategoryList.Add(obj);
                            }
                        }
                    }
                    connection.Close();
                    return objCategoryList;
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, type, connectionStr);
                    return new List<Category>();
                }
            }
        }

        public int MarkCategoryAsDeleted(int categoryId, string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                int response = 0;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPSetIsDeletedCategory] @Id, @Result", connection))
                    {
                        command.Parameters.AddWithValue("@Id", categoryId);
                        command.Parameters.Add("@Result", SqlDbType.Int);
                        command.Parameters["@Result"].Direction = ParameterDirection.Output;
                        response = Convert.ToInt32(command.ExecuteScalar());
                    }
                    connection.Close();
                    return response;
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, categoryId, connectionStr);
                    return 0;
                }
            }
        }

        public List<CategoryModel> GetSubCategoriesByCatId(int categoryId, string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                List<CategoryModel> objCategoryList = new List<CategoryModel>();
                CategoryModel obj = null;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPGetSubCategoriesForApp] @categoryId", connection))
                    {
                        command.Parameters.AddWithValue("@categoryId", categoryId);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                obj = new CategoryModel();
                                obj.Id = Convert.ToInt32(reader["Id"].ToString());
                                obj.Name = reader["Name"].ToString();
                                obj.TotalProductCount = Convert.ToInt32(reader["TotalProductCount"].ToString());
                                objCategoryList.Add(obj);
                            }
                        }
                    }
                    connection.Close();
                    return objCategoryList;
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, categoryId, connectionStr);
                    return new List<CategoryModel>();
                }
            }
        }
    }
}
