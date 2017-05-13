using System.Collections.Generic;
using System.Linq;
using mPower.Documents.Documents.Membership;
using mPower.Framework.Services;
using System.Web.Mvc;

namespace Default.Areas.Administration.Models
{
    public class UsersListModel
    {
        public PagingInfo Paging { get; set; }

        public List<UserDocument> Users { get; set; }

        public string SearchKey { get; set; }

        public string Affiliate { get; set; }

        public bool CanEdit { get; set; }

        public bool CanDelete { get; set; }

        public IDictionary<string, string> AffiliateName { get; set; }

        public List<SelectListItem> Affiliates
        {
            get
            {
                if (AffiliateName != null)
                {
                    var item = new SelectListItem() {Text = "Any", Value = ""};
                    var result = new List<SelectListItem>();
                    result.Add(item);
                    result.AddRange(AffiliateName.Select(x => new SelectListItem() {Text = x.Value, Value = x.Key}));


                    return result;
                }

                return null;
            }
        }
    }
}
