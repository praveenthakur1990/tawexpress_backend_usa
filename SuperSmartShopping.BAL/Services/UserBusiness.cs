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
    public class UserBusiness : IUserBusiness
    {
        public PersonalInfoModel GetPersonalInfo(string userId)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    PersonalInfoModel obj = new PersonalInfoModel();
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPGetUserInfo] @UserId", connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                obj = new PersonalInfoModel();
                                obj.UserId = reader["Id"].ToString();
                                obj.FirstName = reader["FirstName"].ToString();
                                obj.LastName = reader["LastName"].ToString();
                                obj.Email = reader["Email"].ToString();
                                obj.PhoneNumber = reader["PhoneNumber"].ToString();

                                obj.Gender = reader["Gender"].ToString();
                                obj.Mobile = reader["RiderMobile"].ToString();
                                obj.Address = reader["ContactAddress"].ToString();
                                obj.City = reader["City"].ToString();
                                obj.State = reader["State"].ToString();
                                obj.ZipCode = reader["ZipCode"].ToString();
                                obj.StoreNames = reader["StoreNames"].ToString();
                                if (!string.IsNullOrEmpty(obj.StoreNames))
                                {
                                    obj.StoreList = new List<string>();
                                    obj.StoreList = obj.StoreNames.Split(',').ToList();
                                }
                            }
                        }
                    }
                    connection.Close();
                    return obj;
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, userId);
                    return new PersonalInfoModel();
                }
            }
        }

        public List<PersonalInfoModel> GetUsersByRoleName(string roleName)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                List<PersonalInfoModel> objList = new List<PersonalInfoModel>();
                try
                {                  
                    PersonalInfoModel obj = null;
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[GetUsersByRoleName] @RoleName", connection))
                    {
                        command.Parameters.AddWithValue("@RoleName", roleName);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                obj = new PersonalInfoModel();
                                obj.UserId = reader["Id"].ToString();
                                obj.FirstName = reader["FirstName"].ToString();
                                obj.LastName = reader["LastName"].ToString();
                                obj.Email = reader["Email"].ToString();
                                obj.PhoneNumber = reader["PhoneNumber"].ToString();
                                objList.Add(obj);
                            }
                        }
                    }
                    connection.Close();
                    return objList;
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, roleName);
                    return objList;
                }
            }
        }

        public int UpdatePersonalInfo(PersonalInfoModel model)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                int response = 0;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPUpdatePersonalInfo] @UserId, @FirstName, @LastName, @Email, @PhoneNumber, @Result", connection))
                    {
                        command.Parameters.AddWithValue("@UserId", model.UserId);
                        command.Parameters.AddWithValue("@FirstName", model.FirstName);
                        command.Parameters.AddWithValue("@LastName", model.LastName);
                        command.Parameters.AddWithValue("@Email", model.Email);
                        command.Parameters.AddWithValue("@PhoneNumber", model.PhoneNumber);
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
    }
}
