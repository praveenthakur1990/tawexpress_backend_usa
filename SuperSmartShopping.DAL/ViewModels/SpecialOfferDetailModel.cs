using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.DAL.ViewModels
{
    public class SpecialOfferProductsModel
    {
        public int Id { get; set; }

        public int SpecialOfferCatId { get; set; }

        public int ProductId { get; set; }

        public string OfferType { get; set; }

        public decimal OfferValue { get; set; }
    }
}
