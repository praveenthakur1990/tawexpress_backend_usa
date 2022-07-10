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
    public class ProductVarientBusiness : IProductVarientBusiness
    {
        public int AddUpdate(ProductVarientsModel model, string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                int response = 0;
                try
                {

                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPAddUpdateProductVarient] @Id, @ProductId, @Name, @Price, @ImagePath, @IsPublished, @Result", connection))
                    {
                        command.Parameters.AddWithValue("@Id", model.Id);
                        command.Parameters.AddWithValue("@ProductId", model.ProductId);
                        command.Parameters.AddWithValue("@Name", model.Name);
                        command.Parameters.AddWithValue("@Price", model.Price);
                        command.Parameters.AddWithValue("@ImagePath", model.Image ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@IsPublished", model.IsPublished);
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

        public List<ProductVarientsModel> GetProductsVarients(int id, int productId, PaginationModel pageModel, string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                List<ProductVarientsModel> objList = new List<ProductVarientsModel>();
                ProductVarientsModel obj = null;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPGetProductsVarients] @Id, @ProductId, @PageNumber, @PageSize, @SearchStr", connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.Parameters.AddWithValue("@ProductId", productId);
                        command.Parameters.AddWithValue("@PageNumber", pageModel.PageNumber == 0 ? 1 : pageModel.PageNumber);
                        command.Parameters.AddWithValue("@PageSize", pageModel.PageSize);
                        command.Parameters.AddWithValue("@SearchStr", pageModel.SearchStr == null ? string.Empty : pageModel.SearchStr);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                obj = new ProductVarientsModel();
                                obj.Id = Convert.ToInt32(reader["Id"].ToString());
                                obj.ProductId = Convert.ToInt32(reader["ProductId"].ToString());
                                obj.Name = reader["Name"].ToString();
                                obj.Price = Convert.ToDecimal(reader["Price"].ToString());
                                obj.Image = ConfigurationManager.AppSettings["BackendAPPBaseUrl"].ToString() + reader["Image"].ToString();
                                obj.IsPublished = Convert.ToBoolean(reader["IsPublished"].ToString());
                                obj.TotalRows = Convert.ToInt32(reader["TotalRows"].ToString());
                                obj.ProductInfo = new ProductBusiness().GetProducts(obj.ProductId.Value, new PaginationModel { PageNumber = 1, PageSize = 1, SearchStr = string.Empty }, connectionStr).FirstOrDefault();
                                objList.Add(obj);
                            }
                        }
                    }
                    connection.Close();
                    return objList;
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, id, productId, JsonConvert.SerializeObject(pageModel), connectionStr);
                    return new List<ProductVarientsModel>();
                }
            }
        }
    }
}
