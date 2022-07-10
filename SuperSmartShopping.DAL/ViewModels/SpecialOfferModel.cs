using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.DAL.ViewModels
{
    public class SpecialOfferModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool? IsActive { get; set; }

        public bool? IsDeleted { get; set; }

        public DateTime? DeletedDate { get; set; }

        public DateTime? DeletedBy { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
        public string BannerImagePath { get; set; }
        public int TotalRows { get; set; }
        public List<ProductDashboardModel> ProductList { get; set; }
    }

    public class SpecialOfferDatesModel
    {
        public int Id { get; set; }
        public string SpecialOfferDates { get; set; }
        public string BannerImagePath { get; set; }
    }

    public class SpecialOfferProductCatModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
