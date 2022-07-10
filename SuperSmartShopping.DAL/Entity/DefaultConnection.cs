using SuperSmartShopping.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.DAL.Entity
{
    public class DefaultConnection : DbContext
    {
        public virtual DbSet<tb_Tenants> TenantControl { get; set; }
        public virtual DbSet<Rider> Rider { get; set; }
    }
}
