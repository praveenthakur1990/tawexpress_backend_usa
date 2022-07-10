using Newtonsoft.Json;
using SuperSmartShopping.BAL.Interfaces;
using SuperSmartShopping.DAL.Common;
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
    public class InventoryBusiness : IInventoryBusiness
    {
        public int Add(StockModel model, string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                int response = 0;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPAddStock] @ProductId, @Quantity, @AddedBy, @Result", connection))
                    {
                        command.Parameters.AddWithValue("@ProductId", model.Id);
                        command.Parameters.AddWithValue("@Quantity", model.Quantity);
                        command.Parameters.AddWithValue("@AddedBy", model.AddedBy);
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

        public List<StockModel> GetInventorybyProductId(int productId, PaginationModel pageModel, string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                List<StockModel> objList = new List<StockModel>();
                StockModel obj = null;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPGetInventoryByProductId] @ProductId, @PageNumber, @PageSize, @SearchStr", connection))
                    {
                        command.Parameters.AddWithValue("@ProductId", productId);
                        command.Parameters.AddWithValue("@PageNumber", pageModel.PageNumber);
                        command.Parameters.AddWithValue("@PageSize", pageModel.PageSize);
                        command.Parameters.AddWithValue("@SearchStr", pageModel.SearchStr.NullToEmptyString());
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                obj = new StockModel();
                                obj.ProductInfo = new ProductModel();
                                obj.Id = Convert.ToInt32(reader["Id"].ToString());
                                obj.ProductId = Convert.ToInt32(reader["ProductId"].ToString());
                                obj.Quantity = Convert.ToInt32(reader["Quantity"].ToString());
                                obj.AddedDate = Convert.ToDateTime(reader["AddedDate"].ToString());
                                obj.TotalRows = Convert.ToInt32(reader["TotalRows"].ToString());
                                obj.ProductInfo.Name = reader["ProductName"].ToString();
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
                    return new List<StockModel>();
                }
            }
        }

    }
}
