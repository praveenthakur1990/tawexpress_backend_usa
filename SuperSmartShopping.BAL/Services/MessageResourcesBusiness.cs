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
using Twilio.Rest.Api.V2010.Account;

namespace SuperSmartShopping.BAL.Services
{
    public class MessageResourcesBusiness : IMessageResourcesBusiness
    {
        public int AddMessageResources(MessageResource objMsgResourceResponse, string userId)
        {
            using (SqlConnection connection = new SqlConnection(CommonManager.GetTenantConnection(userId, string.Empty).FirstOrDefault().TenantConnection))
            {
                try
                {
                    int result = 0;
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPAddMessageResources] @AccountSid, @ApiVersion, @Body, @DateCreated, @DateSent, @DateUpdated, @Direction, @ErrorCode, @ErrorMessage, @From, @Price, @PriceUnit, @Sid, @Status, @To, @Uri, @Result", connection))
                    {
                        command.Parameters.AddWithValue("@AccountSid", objMsgResourceResponse.AccountSid == null ? string.Empty : objMsgResourceResponse.AccountSid);
                        command.Parameters.AddWithValue("@ApiVersion", objMsgResourceResponse.ApiVersion);
                        command.Parameters.AddWithValue("@Body", objMsgResourceResponse.Body);
                        command.Parameters.AddWithValue("@DateCreated", objMsgResourceResponse.DateCreated);
                        if (objMsgResourceResponse.DateSent == null)
                        {
                            command.Parameters.Add("@DateSent", SqlDbType.DateTime).Value = DBNull.Value;

                        }
                        else
                        {
                            command.Parameters.AddWithValue("@DateSent", objMsgResourceResponse.DateSent);
                        }


                        if (objMsgResourceResponse.DateUpdated == null)
                        {
                            command.Parameters.Add("@DateUpdated", SqlDbType.DateTime).Value = DBNull.Value;

                        }
                        else
                        {
                            command.Parameters.AddWithValue("@DateUpdated", objMsgResourceResponse.DateUpdated);
                        }

                        command.Parameters.AddWithValue("@Direction", objMsgResourceResponse.Direction.ToString());
                        command.Parameters.AddWithValue("@ErrorCode", objMsgResourceResponse.ErrorCode.HasValue ? objMsgResourceResponse.ErrorCode.Value.ToString() : "0");
                        command.Parameters.AddWithValue("@ErrorMessage", objMsgResourceResponse.ErrorMessage != null ? objMsgResourceResponse.ErrorMessage : string.Empty);
                        command.Parameters.AddWithValue("@From", objMsgResourceResponse.From.ToString());
                        command.Parameters.AddWithValue("@Price", objMsgResourceResponse.Price == null ? string.Empty : objMsgResourceResponse.Price);
                        command.Parameters.AddWithValue("@PriceUnit", objMsgResourceResponse.PriceUnit);
                        command.Parameters.AddWithValue("@Sid", objMsgResourceResponse.Sid);
                        command.Parameters.AddWithValue("@Status", objMsgResourceResponse.Status.ToString());
                        command.Parameters.AddWithValue("@To", objMsgResourceResponse.To);
                        command.Parameters.AddWithValue("@Uri", objMsgResourceResponse.Uri);
                        command.Parameters.Add("@Result", SqlDbType.Int);
                        command.Parameters["@Result"].Direction = ParameterDirection.Output;
                        result = Convert.ToInt32(command.ExecuteScalar());
                    }
                    connection.Close();
                    return result;
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, JsonConvert.SerializeObject(objMsgResourceResponse), userId);
                    return 0;
                }
            }
        }

        public List<MessageResourcesViewModel> GetMessagesLogs(string userId, int pageNumber, int pageSize, string searchStr)
        {
            using (SqlConnection connection = new SqlConnection(CommonManager.GetTenantConnection(userId, string.Empty).FirstOrDefault().TenantConnection))
            {
                List<MessageResourcesViewModel> objMessageResourcesList = new List<MessageResourcesViewModel>();
                MessageResourcesViewModel objMessageResources = null;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPGetMessagesLogs_v2] @UserId, @PageNumber, @PageSize, @SearchStr", connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);
                        command.Parameters.AddWithValue("@PageNumber", pageNumber);
                        command.Parameters.AddWithValue("@PageSize", pageSize);
                        command.Parameters.AddWithValue("@SearchStr", searchStr);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                objMessageResources = new MessageResourcesViewModel();
                                objMessageResources.Id = Convert.ToInt32(reader["Id"]);
                                objMessageResources.Body = reader["Body"].ToString();
                                objMessageResources.To = reader["To"].ToString();
                                objMessageResources.DateCreated = Convert.ToDateTime(reader["DateCreated"].ToString());
                                if (reader["DateSent"] is DBNull)
                                {
                                    objMessageResources.DateSent = null;
                                }
                                else
                                {
                                    objMessageResources.DateSent = Convert.ToDateTime(reader["DateSent"].ToString());
                                }

                                if (reader["DateUpdated"] is DBNull)
                                {
                                    objMessageResources.DateUpdated = null;
                                }
                                else
                                {
                                    objMessageResources.DateUpdated = Convert.ToDateTime(reader["DateUpdated"].ToString());
                                }

                                objMessageResources.ErrorCode = reader["ErrorCode"].ToString();

                                if (reader["ErrorMessage"] is DBNull)
                                {
                                    objMessageResources.ErrorMessage = null;
                                }
                                else
                                {
                                    objMessageResources.ErrorMessage = reader["ErrorMessage"].ToString();
                                }

                                if (reader["Status"] is DBNull)
                                {
                                    objMessageResources.Status = null;
                                }
                                else
                                {
                                    objMessageResources.Status = reader["Status"].ToString();
                                }

                                objMessageResources.TotalRows = Convert.ToInt32(reader["TotalRows"]);
                                objMessageResourcesList.Add(objMessageResources);
                            }
                        }
                    }
                    connection.Close();                    
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, userId, pageNumber, pageSize, searchStr);
                }
                return objMessageResourcesList;
            }
        }
    }
}
