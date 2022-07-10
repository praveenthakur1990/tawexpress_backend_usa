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
   public class PlanBusiness: IPlanBusiness
    {
        public int AddUpdatePlan(PlanModel model)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                int response = 0;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPAddUpdatePlan] @Id, @Name, @Price, @PriceId, @ProductId, @Interval, @CreatedBy, @Result", connection))
                    {
                        command.Parameters.AddWithValue("@Id", model.Id);
                        command.Parameters.AddWithValue("@Name", model.Name);
                        command.Parameters.AddWithValue("@Price", model.Price);
                        command.Parameters.AddWithValue("@PriceId", model.PriceId);
                        command.Parameters.AddWithValue("@ProductId", model.ProductId);
                        command.Parameters.AddWithValue("@Interval", model.Interval);
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
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, JsonConvert.SerializeObject(model));
                    return 0;
                }
            }
        }

        public List<PlanModel> GetPlans(int planId)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                List<PlanModel> objMenuItemList = new List<PlanModel>();
                PlanModel obj = null;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPGetPlans] @PlanId", connection))
                    {
                        command.Parameters.AddWithValue("@PlanId", planId);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                obj = new PlanModel();
                                obj.Id = Convert.ToInt32(reader["Id"].ToString());
                                obj.Name = reader["Name"].ToString();
                                obj.Price = Convert.ToDecimal(reader["Price"].ToString());
                                obj.PriceId = reader["PriceId"].ToString();
                                obj.ProductId = reader["ProductId"].ToString();
                                obj.Interval = reader["Interval"].ToString();
                                obj.NamePrice = reader["NamePrice"].ToString();
                                obj.CreatedBy = reader["CreatedBy"].ToString();
                                obj.CreatedDate = Convert.ToDateTime(reader["CreatedDate"].ToString());
                                objMenuItemList.Add(obj);
                            }
                        }
                    }
                    connection.Close();
                    return objMenuItemList;
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, planId);
                    return new List<PlanModel>();
                }
            }
        }
    }
}
