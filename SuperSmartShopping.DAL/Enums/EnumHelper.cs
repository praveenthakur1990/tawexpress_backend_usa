using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.DAL.Enums
{
    public static class EnumHelper
    {
        public enum RolesEnum
        {
            SuperAdmin,
            Admin,
            Partner,
            User,
            Rider
        }

        public enum PlanEnum
        {
            [Description("Free")]
            FREE,
            [Description("Paid")]
            PAID
        }
        public enum MethodEnum
        {
            [Description("/token")]
            Login,
            [Description("/api/Account/Logout")]
            Logout,
            [Description("/api/Account/Register")]
            Register,
            [Description("/api/Account/ForgotPassword")]
            ForgetPassword,
            [Description("/api/Account/ResetPassword")]
            ResetPassword,
            [Description("/api/Account/ChangePassword")]
            ChangePassword,


            [Description("/api/Partner/AddUpdatePartner")]
            AddUpdatePartner,
            [Description("/api/Partner/GetPartnerList")]
            GetPartnerList,
            [Description("/api/Partner/GetPartnerById")]
            GetPartnerById,

            [Description("/api/Plan/AddUpdatePlan")]
            AddUpdatePlan,
            [Description("/api/Plan/GetPlanList")]
            GetPlanList,
            [Description("/api/Plan/GetPlanById")]
            GetPlanById,

            [Description("/api/Store/AddUpdateStore")]
            AddUpdateStore,
            [Description("/api/Store/GetStore")]
            GetStore,

            [Description("/api/Category/AddUpdateCategory")]
            AddUpdateCategory,
            [Description("/api/Category/GetCategories")]
            GetCategories,
            [Description("/api/Category/GetCategoryById")]
            GetCategoryById,
            [Description("/api/Category/MarkeAsDelete")]
            MarkAsDeletedCategory,
            [Description("/api/Category/GetSubCategories")]
            GetSubCategories,


            [Description("/api/Product/AddUpdate")]
            AddUpdate,
            [Description("/api/Product/GetProducts")]
            GetProducts,
            [Description("/api/Product/MarkeAsDelete")]
            MarkeAsDelete,
            [Description("/api/Product/GetTagsList")]
            GetTagsList,
            [Description("/api/Product/GetProductByCategoryIdAPP")]
            GetProductBySubCategoryId,

            [Description("/api/ProductVarient/AddUpdate")]
            AddUpdateProductVarient,
            [Description("/api/ProductVarient/GetProductVarients")]
            GetProductsVarients,

            [Description("/api/Brand/AddUpdateBrand")]
            AddUpdateBrand,
            [Description("/api/Brand/GetBrands")]
            GetBrands,

            [Description("/api/UnitMeasurement/AddUpdateUnitMeasurement")]
            AddUpdateUnitMeasurement,
            [Description("/api/UnitMeasurement/GetUnitMeasurement")]
            GetUnitMeasurement,

            [Description("/api/Inventory/AddStock")]
            AddStock,
            [Description("/api/Inventory/GetInventoryByProductId")]
            GetInventoryByProductId,

            [Description("/api/DeliveryAddress/AddUpdate")]
            AddUpdateDeliveryAddress,
            [Description("/api/DeliveryAddress/Get")]
            GetDeliveryAddresses,

            [Description("/api/Order/SaveOrder")]
            SaveOrder,
            [Description("/api/Order/GetOrderList")]
            GetOrders,
            [Description("/api/Order/UpdateOrderStatus")]
            UpdateOrderStatus,

            [Description("/api/WeeklyCircular/AddUpdate")]
            AddUpdateWeeklyCircular,
            [Description("/api/WeeklyCircular/GetWeeklyCircular")]
            GetWeeklyCircular,
            [Description("/api/WeeklyCircular/AddUpdateProduct")]
            AddUpdateWeeklyCircularProduct,
            [Description("/api/WeeklyCircular/GetWeeklyCircularCatInfo")]
            GetWeeklyCircularCatInfo,
            [Description("/api/WeeklyCircular/GetProductByWeeklyCircularId")]
            GetProductByWeeklyCircularId,
            [Description("/api/WeeklyCircular/GetWeeklyCircularSubscribers")]
            GetWeeklyCircularSubscribers,

            [Description("/api/SpecialOffer/GetSpecialOffer")]
            GetSpecialOffer,
            [Description("/api/SpecialOffer/AddUpdate")]
            AddUpdateSpecialOffer,
            [Description("/api/SpecialOffer/GetProductBySpecialOfferId")]
            GetProductBySpecialOfferId,
            [Description("/api/SpecialOffer/AddUpdateProduct")]
            AddUpdateSpecialOfferProduct,

            [Description("/api/Store/AddBannerImage")]
            AddBannerImage,

            [Description("/api/Rider/GetStores")]
            GetStores,
            [Description("/api/Rider/AddUpdateRider")]
            AddUpdateRider,
            [Description("/api/Rider/GetRiders")]
            GetRiders,
            [Description("/api/Rider/GetStoreLinkedRiders")]
            GetStoreLinkedRiders,
        }

        public enum DirectoryPathEnum
        {
            Upload,
            ErrorLogs,
            StoreDoc,
            GSTDOc,
            Logo,
            CategoryImage,
            ProductImage,
            ProductVarientImage,
            WeeklyCircularPdf,
            WeeklyCircularImg,
            QrCode,
            CategoryBannerImage,
            HomeBannerImage,
            SpecialOfferBannerImage,
            AdvertisementImage
        }

        public enum ExtensionEnum
        {
            [Description(".pdf")]
            PdfExtension,
            [Description(".png")]
            PNGExtension,
            [Description(".cshtml")]
            csHtmlExtension
        }

        public enum OrderStatusEnum
        {
            [Description("Accepted")]
            Accepted,
            [Description("Cancelled")]
            Cancelled,
            [Description("Rejected")]
            Rejected,
            [Description("Out for delivery")]
            Out_for_delivery,
            [Description("Delivered")]
            Delivered
        }
        public static string GetDescription<T>(this T enumValue)
         where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                return null;

            var description = enumValue.ToString();
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

            if (fieldInfo != null)
            {
                var attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (attrs != null && attrs.Length > 0)
                {
                    description = ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            return description;
        }
    }
}
