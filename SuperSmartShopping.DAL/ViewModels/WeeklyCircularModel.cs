using SuperSmartShopping.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.DAL.ViewModels
{
    public class WeeklyCircularModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }       
        public string PdfFilePath { get; set; }
        public string ThumbnailImgPath { get; set; }
        public bool? IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }     
        public int TotalRows { get; set; }
        public List<ProductDashboardModel> WeeklyCircularProductList { get; set; } 
    }

    public class WeeklyCircularCatInfoModel
    {        
        public int WeeklyCircularCatId { get; set; }
        public int WeeklyCircularId { get; set; }
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public List<Category> Categories { get; set; }
        public List<Category> SubCategories { get; set; }
        public string CreatedBy { get; set; }
        public List<WeeklyCircularProductsModel> ProductList { get; set; }
        public DataTable ProductListDataTable { get; set; }

        public string CatName { get; set; }
        public string SubCatName { get; set; }
        public int TotalRows { get; set; }

    }

    public class WeeklyCircularProductsModel
    {       
        public int Id { get; set; }
        public int WeeklyCircularId { get; set; }
        public int WeeklyCircularCatId { get; set; }
        public int ProductId { get; set; }
        public string OfferType { get; set; }
        public decimal OfferValue { get; set; }
    }
    public class WeeklyCircularDatesModel
    {
        public int Id { get; set; }
        public string WeeklyCircularDates { get; set; }
    }

    public class WeeklyCircularSubscriberModel
    {       
        public int Id { get; set; }
        public string FullName { get; set; }
        public string MobileNumber { get; set; }
        public string Otp { get; set; }
        public DateTime? OtpGenerateDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? IsActive { get; set; }
        public string CreatedBy { get; set; }
        public int TotalRows { get; set; }
    }

    public class WeeklyCircularComposeModel
    {
        public string CallBackUrl { get; set; }
        public string DefaultMsg { get; set; }
        public List<WeeklyCircularSubscriberModel> SubscriberList { get; set; }
    }
}
