using SuperSmartShopping.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.BAL.Interfaces
{
    public interface ISocialMedialinksBusiness
    {
        List<SocialMedia> GetSocialMediaLinks(string connectionStr);
        int AddUpdateSocialMediaLinks(string SocialMediaLinkJsonStr, string createdBy, string connectionStr);
    }
}
