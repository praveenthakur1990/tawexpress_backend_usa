using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.DAL.ViewModels
{
    public class ProductModel
    {
        public int Id { get; set; }
        public int? CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        public string Name { get; set; }
        public bool? IsVariants { get; set; }
        public decimal? Price { get; set; }
        public string Description { get; set; }
        public bool IsDescriptionShow { get; set; }
        public string Image { get; set; }
        public string MarkItemAs { get; set; }
        public string VegNonVeg { get; set; }
        public bool? IsPublished { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

        public int TotalRows { get; set; }
        public int BrandId { get; set; }
        public int UnitMeasurementId { get; set; }

        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public string BrandName { get; set; }
        public string UnitMeasurementName { get; set; }
        public int StockInHand { get; set; }
        public bool IsCategoryActive { get; set; }
        public string TagIds { get; set; }

    }
    public class ProductDashboardModel
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public string ProductName { get; set; }
        public decimal? Price { get; set; }
        public string ProductImage { get; set; }
        public string CategoryName { get; set; }
        public string CategoryImage { get; set; }
        public string SubCategoryName { get; set; }
        public bool IsVarient { get; set; }
        public int DefaultVarientId { get; set; }
        public List<ProductVarientsModel> ProductVarients { get; set; }
        public string Description { get; set; }
        public bool IsDescriptionShow { get; set; }
        public string TagIds { get; set; }
        public int WeeklyCircularId { get; set; }
        public string OfferType { get; set; }
        public decimal OfferValue { get; set; }
        public string FinalOfferValue { get; set; }
        public string FinalValue { get; set; }
             
        public int WeeklyCircularCatId { get; set; }
        public int WeeklyCircularProductId { get; set; }


        public int SpecialOfferId { get; set; }
        public int SpecialOfferCatId { get; set; }
        public int SpecialOfferProductId { get; set; }
        public string BannerImg { get; set; }
    }

    public class WeeklyCircularProductCatModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }


}
