using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.DAL.ViewModels
{
   public class ProductVarientsModel
    {
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }
        public string Image { get; set; }
        public bool? IsPublished { get; set; }
        public int TotalRows { get; set; }
        public string CreatedBy { get; set; }
        public ProductModel ProductInfo { get; set; }
    }
}
