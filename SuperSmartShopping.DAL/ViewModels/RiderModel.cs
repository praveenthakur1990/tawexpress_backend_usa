using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.DAL.ViewModels
{
    public class RiderModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public string EmailAddress { get; set; }
        public string Gender { get; set; }
        public string ContactAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsOnline { get; set; }
        public string DeviceToken { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string StoreIds { get; set; } 
        public List<RiderStoreLinkingModel> StoreIdList { get; set; }
        public DataTable StoreIdTable { get; set; }
        public int TotalRows { get; set; }

        public bool IsRequestSent { get; set; }
        public bool IsAccepted { get; set; }
        public DateTime? AcceptedDate { get; set; }
        public DateTime? RequestedDate { get; set; }
    }

    public class RiderStoreLinkingModel
    {
        public int RiderId { get; set; }
        public string StoreId { get; set; }
    }

    public class RiderOrderModel
    {
        public string StoreUserId { get; set; }
        public string OrderIds { get; set; }
        public int TotalCount { get; set; }
    }
}
