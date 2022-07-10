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
    public class BusinessHoursBusiness : IBusinessHoursBusiness
    {
        public int AddUpdateBusinessHours(string businessHourksonStr, string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                int response = 0;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPAddUpdateBusinessHours] @BusinessHourksonStr, @Result", connection))
                    {
                        command.Parameters.AddWithValue("@BusinessHourksonStr", businessHourksonStr);
                        command.Parameters.Add("@Result", SqlDbType.Int);
                        command.Parameters["@Result"].Direction = ParameterDirection.Output;
                        response = Convert.ToInt32(command.ExecuteScalar());
                    }
                    connection.Close();
                    return response;
                }
                catch (Exception ex)
                {
                    CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, businessHourksonStr, connectionStr);
                    return 0;
                }
            }
        }

        public List<BusinessHoursModel> GetBusinessHours(string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                List<BusinessHoursModel> objList = new List<BusinessHoursModel>();
                BusinessHoursModel obj = null;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPGetBusinessHours]", connection))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                obj = new BusinessHoursModel();
                                obj.Id = Convert.ToInt32(reader["Id"].ToString());
                                obj.WeekDayId = Convert.ToInt32(reader["WeekDayId"].ToString());
                                obj.DayName = obj.WeekDayId == 1 ? "Sunday" : obj.WeekDayId == 2 ? "Monday" : obj.WeekDayId == 3 ? "Tuesday" : obj.WeekDayId == 4 ? "Wednesday" : obj.WeekDayId == 5 ? "Thursday" : obj.WeekDayId == 6 ? "Friday" : "Saturday";
                                obj.OpenTime = reader["OpenTime"].ToString();
                                obj.CloseTime = reader["CloseTime"].ToString();
                                obj.OpenTime12Hour = reader["OpenTime12Hour"].ToString();
                                obj.CloseTime12Hour = reader["CloseTime12Hour"].ToString();
                                obj.IsClosed = Convert.ToBoolean(reader["IsClosed"].ToString());
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
                    return new List<BusinessHoursModel>();
                }
            }
        }
    }
}
