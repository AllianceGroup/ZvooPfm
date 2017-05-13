using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mPower.Web.Admin.Models
{
    public class DeploymentModel
    {
        public string Message { get; set; }

        public string BackupFolder { get; set; }

        public string Package { get; set; }

        public List<System.Web.Mvc.SelectListItem> Packages { get; set; }
    }
}