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
    public class QuickPagesBusiness : IQuickPagesBusiness
    {
        public int AddUpdateQuickPages(QuickPageModel model, string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                int response = 0;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPAddUpdateQuickLinks_v2] @Id, @Name, @Link, @PageContent, @IsActive, @CreatedBy, @Result", connection))
                    {
                        command.Parameters.AddWithValue("@Id", model.Id);
                        command.Parameters.AddWithValue("@Name", model.Name);
                        command.Parameters.AddWithValue("@Link", model.Link == null ? DBNull.Value.ToString() : model.Link);
                        command.Parameters.AddWithValue("@PageContent", model.PageContent == null ? DBNull.Value.ToString() : model.PageContent);
                        command.Parameters.AddWithValue("@IsActive", model.IsActive);
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

        public List<QuickPageModel> GetQuickPages(string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                List<QuickPageModel> objList = new List<QuickPageModel>();
                QuickPageModel obj = null;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPGetQuickLinks]", connection))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                obj = new QuickPageModel();
                                obj.Id = Convert.ToInt32(reader["Id"].ToString());
                                obj.Name = reader["Name"].ToString();
                                obj.PageContent = reader["PageContent"].ToString();
                                obj.Link = reader["Link"].ToString();
                                obj.IsActive = Convert.ToBoolean(reader["IsActive"].ToString());
                                obj.IsRedirectToUrl = Convert.ToBoolean(reader["IsRedirectToLink"].ToString());
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
                    return new List<QuickPageModel>();
                }
            }
        }
    }
}
