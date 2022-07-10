using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.DAL.ViewModels
{
    public class StockModel
    {
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }
        public DateTime? AddedDate { get; set; }
        public string AddedBy { get; set; }
        public ProductModel ProductInfo { get; set; }
        public int TotalRows { get; set; }
    }
}
