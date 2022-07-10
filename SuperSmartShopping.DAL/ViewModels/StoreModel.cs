using SuperSmartShopping.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.DAL.ViewModels
{
    public class StoreModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string CountryCode { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string ContactPersonName { get; set; }
        public string ContactNumber { get; set; }
        public string LogoPath { get; set; }
        public string GSTRegistrationNumber { get; set; }
        public string GSTFilePath { get; set; }
        public string ActivePlan { get; set; }
        public decimal Commision { get; set; }
        public DateTime? PlanActiveDate { get; set; }
        public string CurrencySymbol { get; set; }
        public string TimeZone { get; set; }
        public bool IsActive { get; set; }
        public decimal MinOrderAmt { get; set; }
        public decimal MaxDeliveryAreaInMiles { get; set; }
        public decimal TaxRate { get; set; }
        public decimal DeliveryCharges { get; set; }
        public bool CashOnDeliveryEnable { get; set; }
        public int PlanId { get; set; }
        public bool IsSubscriptionCancelled { get; set; }
        public DateTime? SubscriptionCancelledOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public BusinessHoursModel BusinessHours { get; set; }
        public DateTime? PickUpStartDateTime { get; set; }
        public DateTime? PickUpEndDateTime { get; set; }
        public string PickupAddresses { get; set; }
        public string PickUpDateTimeStr { get; set; }
        public string PickUpDateJsonStr { get; set; }
        public string PickUpTimeJsonStr { get; set; }
        public List<PickUpDateVM> PickUpDateList { get; set; }
        public List<PickUpTimeVM> PickUpTimeList { get; set; }
        public string UserId { get; set; }
        public string StripePublishablekey { get; set; }
        public string StripeSecretkey { get; set; }
        public List<BusinessHoursModel> BusinessHourList { get; set; }
        public string QrCodePath { get; set; }
        public List<BannerImages> BannerList { get; set; }
        public List<BannerImages> AdvImgList { get; set; }
    }

    public class StoreListModel
    {
        public string UserId { get; set; }
        public string DomainName { get; set; }
    }
}
