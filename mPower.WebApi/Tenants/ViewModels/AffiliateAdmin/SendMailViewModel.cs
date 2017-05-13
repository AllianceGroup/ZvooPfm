using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using mPower.WebApi.Tenants.Model.AffiliateAdmin;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace mPower.WebApi.Tenants.ViewModels.AffiliateAdmin
{
    public class SendMailViewModel
    {
        public List<SelectListItem> Contents { get; set; }

        public List<MailSegmentModel> Segments { get; set; }

        [Required(ErrorMessage = "Please, select recipients")]
        public string Ids { get; set; } 

        [Required(ErrorMessage = "Please, select email content")]
        public string ContentId { get; set; }

        public SendMailViewModel()
        {
            Segments = new List<MailSegmentModel>();
            Contents = new List<SelectListItem>();
        }
    }
}
