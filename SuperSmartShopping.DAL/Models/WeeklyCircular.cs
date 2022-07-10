using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.DAL.Models
{
    public class WeeklyCircular
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string PdfFilePath { get; set; }
        public string ThumbnailImgPath { get; set; }
        public bool? IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsDeleted { get; set; }
        public string DeletedBy { get; set; }
    }

    public class WeeklyCircularCatInfo
    {
        [Key]
        public int WeeklyCircularCatId { get; set; }
        public int WeeklyCircularId { get; set; }
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
    }

    public class WeeklyCircularProducts
    {
        [Key]
        public int Id { get; set; }
        public int WeeklyCircularId { get; set; }
        public int WeeklyCircularCatId { get; set; }
        public int ProductId { get; set; }
        public string OfferType { get; set; }
        public decimal OfferValue { get; set; }
    }

    public class WeeklyCircularSubscriber
    {
        [Key]
        public int Id { get; set; }
        public string FullName { get; set; }
        public string MobileNumber { get; set; }
        public string Otp { get; set; }
        public DateTime? OtpGenerateDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? IsActive { get; set; }
    }
}
