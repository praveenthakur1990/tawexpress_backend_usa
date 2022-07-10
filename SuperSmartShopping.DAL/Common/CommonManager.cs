using Newtonsoft.Json;
using QRCoder;
using SuperSmartShopping.DAL.Enums;
using SuperSmartShopping.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using static SuperSmartShopping.DAL.Enums.EnumHelper;

namespace SuperSmartShopping.DAL.Common
{
    public static class CommonManager
    {
        public static System.Net.Http.HttpContent CreateHttpContent(object content)
        {
            System.Net.Http.HttpContent httpContent = null;

            if (content != null)
            {
                var ms = new System.IO.MemoryStream();
                SerializeJsonIntoStream(content, ms);
                ms.Seek(0, System.IO.SeekOrigin.Begin);
                httpContent = new System.Net.Http.StreamContent(ms);
                httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            }

            return httpContent;
        }
        public static void SerializeJsonIntoStream(object value, System.IO.Stream stream)
        {
            using (var sw = new System.IO.StreamWriter(stream, new System.Text.UTF8Encoding(false), 1024, true))
            using (var jtw = new Newtonsoft.Json.JsonTextWriter(sw) { Formatting = Newtonsoft.Json.Formatting.None })
            {
                var js = new Newtonsoft.Json.JsonSerializer();
                js.Serialize(jtw, value);
                jtw.Flush();
            }
        }
        public static string CreatePathIfMissing(string path)
        {
            bool folderExists = System.IO.Directory.Exists(path);
            if (!folderExists)
                System.IO.Directory.CreateDirectory(path);
            return path;
        }
        public static string CreatePathIfMissingFromWeb(string path)
        {
            bool folderExists = Directory.Exists(HttpContext.Current.Server.MapPath(path));
            if (!folderExists)
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath(path));
            return path;
        }
        public static string CreateUniqueFileName(string type)
        {
            return string.Format(@"{0}", DateTime.Now.Ticks + type);
        }
        public static void LogError(MethodBase method, Exception ex, params object[] values)
        {
            string appPhysicalPath = ConfigurationManager.AppSettings["BackendPhysicalPath"].ToString();
            ParameterInfo[] parms = method.GetParameters();
            object[] namevalues = new object[2 * parms.Length];
            string msg = "Error in " + string.Format("{0} {1}", method.Name, "method") + "(";
            for (int i = 0, j = 0; i < parms.Length; i++, j += 2)
            {
                msg += "{" + j + "}={" + (j + 1) + "}, ";
                namevalues[j] = parms[i].Name;
                if (i < (values.Length)) namevalues[j + 1] = values[i];
            }
            msg += "exception=" + ex.Message + ")";
            try
            {
                string fileUploadPath = DirectoryPathEnum.Upload.ToString() + "/" + DirectoryPathEnum.ErrorLogs.ToString() + "/";
                string directoryPath = CreatePathIfMissing(appPhysicalPath + "/" + fileUploadPath);
                string filepath = directoryPath + DateTime.Today.ToString("dd-MM-yy") + ".txt";
                if (!File.Exists(filepath))
                {
                    File.Create(filepath).Dispose();
                }
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    string error = "Log Written Date:" + " " + DateTime.Now.ToString();
                    sw.WriteLine("-------------------------------------------------------------------------------------");
                    sw.WriteLine(error);
                    sw.WriteLine(string.Format(msg, namevalues));

                    sw.WriteLine("--------------------------------*End*------------------------------------------");
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception e)
            {
                e.ToString();
            }
        }
        public static bool IsBase64(this string base64String)
        {
            if (string.IsNullOrEmpty(base64String) || base64String.Length % 4 != 0
               || base64String.Contains(" ") || base64String.Contains("\t") || base64String.Contains("\r") || base64String.Contains("\n"))
                return false;
            try
            {
                Convert.FromBase64String(base64String);
                return true;
            }
            catch (Exception exception)
            {
                // Handle the exception
            }
            return false;
        }
        public static List<TenantModel> GetTenantConnection(string userId, string email)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                List<TenantModel> objList = new List<TenantModel>();
                TenantModel objTenants = null;
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPGetTenantConnection] @UserId, @Email", connection))
                    {
                        command.Parameters.AddWithValue("@UserId", (userId == null ? string.Empty : userId));
                        command.Parameters.AddWithValue("@Email", (email == null ? string.Empty : email));
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                objTenants = new TenantModel();
                                objTenants.Id = Convert.ToInt32(reader["Id"]);
                                objTenants.UserId = reader["UserId"].ToString();
                                objTenants.TenantName = reader["TenantName"].ToString();
                                objTenants.TenantSchema = reader["TenantSchema"].ToString();
                                objTenants.TenantConnection = reader["TenantConnection"].ToString();
                                objTenants.TenantDomain = reader["TenantDomain"].ToString();
                                objTenants.CreatedBy = reader["CreatedBy"] is DBNull ? string.Empty : reader["CreatedBy"].ToString();
                                objTenants.RoleName = reader["RoleName"] is DBNull ? string.Empty : reader["RoleName"].ToString();
                                objTenants.AddedByName = reader["AddedByName"] is DBNull ? string.Empty : reader["AddedByName"].ToString();
                                objList.Add(objTenants);
                            }
                        }
                    }
                    connection.Close();
                    return objList;
                }
                catch (Exception ex)
                {
                    return new List<TenantModel>();
                }
            }
        }
        public static string NullToEmptyString(this string value)
        {
            if (value == null) value = string.Empty;
            return value;
        }
        public static string RemoveSpecialCharacters(this string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
        public static int GenerateRandomNo()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }
        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
        public static string GenerateQrCode(string message, string directoryPath)
        {
            try
            {
                string fileName = string.Empty, fullPath = string.Empty;
                QRCodeGenerator ObjQr = new QRCodeGenerator();
                QRCodeData qrCodeData = ObjQr.CreateQrCode(message, QRCodeGenerator.ECCLevel.Q);
                Bitmap bitMap = new QRCode(qrCodeData).GetGraphic(20);
                using (MemoryStream ms = new MemoryStream())
                {
                    bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    byte[] byteImage = ms.ToArray();
                    string[] files = Directory.GetFiles(HttpContext.Current.Server.MapPath(directoryPath));
                    foreach (string file in files)
                    {
                        System.IO.File.Delete(file);
                    }
                    fileName = CommonManager.CreateUniqueFileName(EnumHelper.ExtensionEnum.PNGExtension.GetDescription().ToString());
                    fullPath = directoryPath + fileName;
                    System.IO.File.WriteAllBytes(HttpContext.Current.Server.MapPath(fullPath), byteImage);
                    return fullPath;
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        public static string CreateRandomPassword(int length = 15)
        {
            // Create a string of characters, numbers, special characters that allowed in the password  
            string validChars = "0123456789";
            Random random = new Random();

            // Select one random character at a time from the string  
            // and create an array of chars  
            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = validChars[random.Next(0, validChars.Length)];
            }
            return new string(chars);
        }


    }
}
