using SuperSmartShopping.BAL.Interfaces;
using SuperSmartShopping.BAL.Services;
using System.Web.Http;
using Unity;
using Unity.WebApi;

namespace SuperSmartShopping.API
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();
            container.RegisterType<IPartnerBusiness, PartnerBusiness>();
            container.RegisterType<IStoreBusiness, StoreBusiness>();
            container.RegisterType<IPlanBusiness, PlanBusiness>();
            container.RegisterType<ICategoryBusiness, CategoryBusiness>();
            container.RegisterType<IProductBusiness, ProductBusiness>();
            container.RegisterType<IProductVarientBusiness, ProductVarientBusiness>();
            container.RegisterType<IBrandBusiness, BrandBusiness>();
            container.RegisterType<IUnitMeasurementBusiness, UnitMeasurementBusiness>();
            container.RegisterType<IInventoryBusiness, InventoryBusiness>();
            container.RegisterType<IDeliveryAddressBusiness, DeliveryAddressBusiness>();
            container.RegisterType<IOrderBusiness, OrderBusiness>();
            container.RegisterType<IUserBusiness, UserBusiness>();
            container.RegisterType<IWeeklyCircularBusiness, WeeklyCircularBusiness>();
            container.RegisterType<ISpecialOfferBusiness, SpecialOfferBusiness>();
            container.RegisterType<IRiderBusiness, RiderBusiness>();
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}