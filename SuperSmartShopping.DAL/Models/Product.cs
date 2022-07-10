using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.DAL.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public int? CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        public int BrandId { get; set; }
        public int UnitMeasurementId { get; set; }
        public string Name { get; set; }
        public bool? IsVariants { get; set; }
        public decimal? Price { get; set; }
        public string Description { get; set; }
        public bool IsDescriptionShow { get; set; }
        public string Image { get; set; }
        public string MarkItemAs { get; set; }
        public string VegNonVeg { get; set; }
        public bool? IsPublished { get; set; }
        public string TagIds { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
