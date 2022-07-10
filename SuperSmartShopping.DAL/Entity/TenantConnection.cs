using SuperSmartShopping.DAL.Models;
using SuperSmartShopping.DAL.ViewModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.DAL.Entity
{
    public partial class TenantConnection : DbContext
    {
        private TenantConnection(DbConnection connection, DbCompiledModel model) : base(connection, model, contextOwnsConnection: false)
        {
        }

        public virtual DbSet<DBError> DbErrorModel { get; set; }

        private static ConcurrentDictionary<Tuple<string, string>, DbCompiledModel> modelCache = new ConcurrentDictionary<Tuple<string, string>, DbCompiledModel>();
        public static TenantConnection Create(string tenantSchema, DbConnection connection)
        {
            try
            {
                var compiledModel = modelCache.GetOrAdd(
               Tuple.Create(connection.ConnectionString, tenantSchema),
               t =>
               {
                   var builder = new DbModelBuilder();
                   builder.Conventions.Remove<IncludeMetadataConvention>();

                   builder.Entity<DBError>().ToTable("DB_Errors", tenantSchema);
                   builder.Entity<Brand>().ToTable("tb_Brand", tenantSchema);
                   builder.Entity<BannerImages>().ToTable("tb_BannerImages", tenantSchema);
                   builder.Entity<BusinessHours>().ToTable("tb_BusinessHours", tenantSchema);
                   builder.Entity<Category>().ToTable("tb_Categories", tenantSchema);
                   builder.Entity<DeliveryAddresses>().ToTable("tb_DeliveryAddresses", tenantSchema);
                   builder.Entity<MessageResources>().ToTable("tb_MessageResources", tenantSchema);
                   builder.Entity<OrderDetail>().ToTable("tb_OrderDetails", tenantSchema);
                   builder.Entity<Order>().ToTable("tb_Orders", tenantSchema);
                   builder.Entity<OrderStatusLogs>().ToTable("tb_OrderStatusLogs", tenantSchema);
                   builder.Entity<Payment>().ToTable("tb_Payments", tenantSchema);
                   builder.Entity<Product>().ToTable("tb_Product", tenantSchema);
                   builder.Entity<ProductVariant>().ToTable("tb_ProductVariant", tenantSchema);
                   builder.Entity<QuickPage>().ToTable("tb_QuickLinks", tenantSchema);
                   builder.Entity<SocialMedia>().ToTable("tb_SocialMedia", tenantSchema);
                   builder.Entity<StockIn>().ToTable("tb_StockIn", tenantSchema);
                   builder.Entity<Store>().ToTable("tb_Store", tenantSchema);
                   builder.Entity<UnitMeasurement>().ToTable("tb_UnitMeasurement", tenantSchema);
                   builder.Entity<WeeklyCircular>().ToTable("tb_WeeklyCircular", tenantSchema);
                   builder.Entity<WeeklyCircularCatInfo>().ToTable("tb_WeeklyCircularCatInfo", tenantSchema);
                   builder.Entity<WeeklyCircularProducts>().ToTable("tb_WeeklyCircularProducts", tenantSchema);
                   builder.Entity<WeeklyCircularSubscriber>().ToTable("tb_WeeklyCircularSubscriber", tenantSchema);


                   builder.Entity<SpecialOffer>().ToTable("tb_SpecialOffer", tenantSchema);
                   builder.Entity<SpecialOfferCatInfo>().ToTable("tb_SpecialOfferCatInfo", tenantSchema);
                   builder.Entity<SpecialOfferProducts>().ToTable("tb_SpecialOfferProducts", tenantSchema);
                   var model = builder.Build(connection);
                   return model.Compile();
               });
                return new TenantConnection(connection, compiledModel);
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        /// <summary>
        /// Creates the database and/or tables for a new tenant
        /// </summary>
        public static void ProvisionTenant(string tenantSchema, DbConnection connection)
        {
            using (var ctx = Create(tenantSchema, connection))
            {
                if (!ctx.Database.Exists())
                {
                    ctx.Database.Create();
                }
                CreateStoreProcedureCopy(ctx.Database.Connection.Database);
            }
        }
        public static void CreateStoreProcedureCopy(string tenantDbName)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Exec [dbo].[USPCopySP] @Name", connection))
                    {
                        command.Parameters.AddWithValue("@Name", tenantDbName);
                        SqlDataReader reader = command.ExecuteReader();
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}
