using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace mPower.Web.Admin.Models.Affiliate
{
    public class CreateAffiliateModel
    {
        [Required]
        public string AffiliateId { get; set; }

        [Required]
        public string AffiliateName { get; set; }
    }
}