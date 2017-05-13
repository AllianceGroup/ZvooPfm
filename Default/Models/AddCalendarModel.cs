using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Default.Models
{
    public class AddCalendarModel
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}