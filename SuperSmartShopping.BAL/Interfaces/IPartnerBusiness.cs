﻿using SuperSmartShopping.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.BAL.Interfaces
{
    public interface IPartnerBusiness
    {
        int AddUpdatePartner(PartnerModel model);
        List<PartnerModel> GetAllPartners(int id);
    }
}
