using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace Default.ViewModel.Areas.Administration
{
    public class SendMailModel
    {
        public List<SelectListItem> Contents { get; set; }

        [Required(ErrorMessage = "Please, select recipients")]
        public string Ids { get; set; }

        [Required(ErrorMessage = "Please, select email content")]
        public string ContentId { get; set; }

        public List<String> Segments { get; set; }

        public bool HasNotEmptySegments { get { return Segments.Any(); } }

        public SendMailModel()
        {
            Segments = new List<String>();
        }
    }
}
