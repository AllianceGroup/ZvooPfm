using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mPower.Web.Admin.Models.Affiliate
{
    public class AffiliateModel
    {
        public AffiliateModel()
        {
            CreateModel = new CreateAffiliateModel();
            UpdateModel = new UpdateAffiliateModel();
        }

        public CreateAffiliateModel CreateModel { get; set; }

        public UpdateAffiliateModel UpdateModel { get; set; }

        public List<SelectListItem> Affiliates { get; set; }

        public int AffiliateId { get; set; }

        public string IisAppPoolName { get; set; }
    }
}