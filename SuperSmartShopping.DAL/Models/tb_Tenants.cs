using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.DAL.Models
{
    public class tb_Tenants
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string TenantName { get; set; }
        public string TenantSchema { get; set; }
        public string TenantConnection { get; set; }
        public string TenantDomain { get; set; }
        public string TenantDatabaseName { get; set; }
        public string CreatedBy { get; set; }
    }
}
