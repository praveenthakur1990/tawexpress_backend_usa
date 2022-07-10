using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.DAL.Models
{
    public class Store
    {
        [Key]
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
        public string StripePublishablekey { get; set; }
        public string StripeSecretkey { get; set; }
        public string QrCodePath { get; set; }
    }
}
