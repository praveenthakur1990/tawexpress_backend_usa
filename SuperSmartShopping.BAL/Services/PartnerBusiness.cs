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
    public class PartnerBusiness : IPartnerBusiness
    {
        public int AddUpdatePartner(PartnerModel model)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                int response = 0;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPAddUpdatePartner] @PartnerID, @Name, @EmailAddress, @ContactNo, @Address, @City, @State, @Country, @ZipCode, @Commisson, @CreatedBy, @Result", connection))
                    {
                        command.Parameters.AddWithValue("@PartnerID", model.PartnerID);
                        command.Parameters.AddWithValue("@Name", model.Name);
                        command.Parameters.AddWithValue("@EmailAddress", model.PartnerID > 0 ? string.Empty : model.EmailAddress);
                        command.Parameters.AddWithValue("@ContactNo", model.ContactNo);
                        command.Parameters.AddWithValue("@Address", model.Address);
                        command.Parameters.AddWithValue("@State", model.State);
                        command.Parameters.AddWithValue("@City", model.City);
                        command.Parameters.AddWithValue("@ZipCode", model.ZipCode);
                        command.Parameters.AddWithValue("@Country", model.Country);
                        command.Parameters.AddWithValue("@Commisson", model.Commision);
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

        public List<PartnerModel> GetAllPartners(int id)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                List<PartnerModel> objMenuItemList = new List<PartnerModel>();
                PartnerModel obj = null;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPGetPartners] @PartnerID", connection))
                    {
                        command.Parameters.AddWithValue("@PartnerID", id);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                obj = new PartnerModel();
                                obj.PartnerID = Convert.ToInt32(reader["PartnerID"].ToString());
                                obj.Name = reader["Name"].ToString();
                                obj.EmailAddress = reader["EmailAddress"].ToString();
                                obj.ContactNo = reader["ContactNo"].ToString();
                                obj.Address = reader["Address"].ToString();
                                obj.City = reader["City"].ToString();
                                obj.State = reader["State"].ToString();
                                obj.Country = reader["Country"].ToString();
                                obj.ZipCode = reader["ZipCode"].ToString();
                                obj.Commision = Convert.ToDecimal(reader["Commision"].ToString());
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
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, id);
                    return new List<PartnerModel>();
                }
            }
        }
    }
}
