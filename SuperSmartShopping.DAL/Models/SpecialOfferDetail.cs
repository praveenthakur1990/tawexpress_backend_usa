using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.DAL.Models
{
    public class SpecialOfferProducts
    {
        [Key]
        public int Id { get; set; }

        public int SpecialOfferCatId { get; set; }

        public int ProductId { get; set; }

        public string OfferType { get; set; }

        public decimal OfferValue { get; set; }
    }
}
