using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using mPower.Documents.Documents.Membership;

namespace mPower.Web.Admin.Models
{
    public class UserLoginsModel
    {
        public List<UserLoginDocument> Logins { get; set; }

        public List<SelectListItem> DateOptions { get; set; }

        public string SearchKey { get; set; }

        public List<SelectListItem> SortOptions { get; set; }
    }
}