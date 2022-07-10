using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.DAL.ViewModels
{
    public class TenantModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string TenantName { get; set; }
        public string TenantSchema { get; set; }
        public string TenantConnection { get; set; }
        public string TenantDomain { get; set; }
        public string CreatedBy { get; set; }
        public string RoleName { get; set; }
        public string AddedByName { get; set; }
    }
}
