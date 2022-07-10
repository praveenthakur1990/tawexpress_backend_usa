using SuperSmartShopping.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace SuperSmartShopping.DAL.ViewModels
{
    public class OrderModel
    {
        public string OrderNo { get; set; }
        public string OrderBy { get; set; }
        public string OrderType { get; set; }
        public int? DeliveryAddressId { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? DeliveryStartTime { get; set; }
        public DateTime? DeliveryEndTime { get; set; }
        public DateTime? OrderDate { get; set; }
        public decimal? TaxRate { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? DeliveryCharges { get; set; }
        public DataTable OrderDetailsTable { get; set; }
        public DataTable PaymentTable { get; set; }
        public List<OrderDetailModel> OrderDetails { get; set; }
        public string StoreUserId { get; set; }
        public string StripeToken { get; set; }
        public string Email { get; set; }

    }

    public class OrderDetailModel
    {
        public int OrderId { get; set; }
        public int? ProductId { get; set; }
        public int? ProductVarientId { get; set; }
        public string ProductName { get; set; }
        public decimal? Price { get; set; }
        public int? Qty { get; set; }
        public decimal? TotalPrice { get; set; }
        public string OfferType { get; set; }
        public decimal OfferValue { get; set; }
        public string FinalValue { get; set; }
        public string FinalOfferValue { get; set; }
    }

    public class OrderInfoModel
    {
        public string AddressType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public int OrderId { get; set; }
        public string OrderNo { get; set; }
        public string OrderBy { get; set; }
        public string OrderType { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? DeliveryStartTime { get; set; }
        public DateTime? DeliveryEndTime { get; set; }
        public DateTime? OrderedDate { get; set; }
        public decimal TaxRate { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DeliveryCharges { get; set; }
        public int TotalRows { get; set; }
        public List<OrderDetailInfoModel> OrderDetail { get; set; }
        public string ReferenceId { get; set; }
        public decimal CapturedAmt { get; set; }
        public string Status { get; set; }
        public DateTime? StatusChangedDate { get; set; }
        public List<OrderStatusLogs> OrderStatusLogs { get; set; }
        public bool IsCustomerBillCopy { get; set; }
        public PersonalInfoModel UserInfoModel { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime? TransactionDate { get; set; }
        public decimal SubTotal { get; set; }

        public RiderModel RiderInfo { get; set; }
        public string StoreName { get; set; }
        public string StoreAddress { get; set; }
        public string StoreUserId { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }

        public string StoreLatitude { get; set; }
        public string StoreLongitude { get; set; }
    }

    public class OrderDetailInfoModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImg { get; set; }
        public decimal Price { get; set; }
        public int Qty { get; set; }
        public decimal TotalPrice { get; set; }
        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public string OfferType { get; set; }
        public decimal OfferValue { get; set; }
        public string FinalValue { get; set; }
        public string FinalOfferValue { get; set; }
    }


}
