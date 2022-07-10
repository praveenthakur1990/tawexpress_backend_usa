using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.DAL.Models
{
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }
        public int? OrderId { get; set; }
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
}
