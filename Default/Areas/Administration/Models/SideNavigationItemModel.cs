using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Default.Areas.Administration.Models
{
    public class SideNavigationItemModel
    {
        protected bool? isSelected;

        public string Title { get; set; }

        public string Url 
        {
            get { return Urls == null ? null : Urls.FirstOrDefault(); }
            set { Urls = string.IsNullOrEmpty(value) ? new List<string>() : new List<string> {value}; }
        }

        public List<string> Urls { get; set; }

        public bool IsSelected
        {
            get
            {
                if (!isSelected.HasValue && Url != null)
                {
                    isSelected = Urls.Any(x => HttpContext.Current.Request.Url.LocalPath.StartsWith(x, StringComparison.InvariantCultureIgnoreCase));
                }
                return isSelected ?? false;
            }
        }
    }

    public class SideNavigationSectionModel : SideNavigationItemModel
    {
        protected bool? isOpened;

        public List<SideNavigationItemModel> Items { get; set; }

        public bool HasItems
        {
            get { return Items != null && Items.Any(); }
        }

        public bool IsOpened
        {
            get
            {
                if (!isOpened.HasValue && Items != null)
                {
                    isOpened = Items.Any(x => x.IsSelected);
                }
                return isOpened ?? false;
            }
        }
    }
}