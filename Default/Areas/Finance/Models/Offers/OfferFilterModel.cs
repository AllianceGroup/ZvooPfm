using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using mPower.Framework.Geo;
using mPower.Framework.Services;
using mPower.TempDocuments.Server.DocumentServices;

namespace Default.Areas.Finance.Controllers
{
    public class OfferFilterModel : OfferFilter
    {
        public IEnumerable<SelectListItem> ItemsPerPage { get; set; }

        public IEnumerable<SelectListItem> RadiusSelectList { get; set; }

        public bool ShowPaging { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string LocationSearchString { get; set; }

        public override Location? GeoLocation
        {
            get
            {
                try
                {
                    return GetLocation();
                }
                catch 
                {
                    return base.GeoLocation;
                }
            }
            set
            {
                base.GeoLocation = value;
            }
        }

        public string SelectedOfferId { get; set; }

        public string SelectedCategoryTitle { get; set; }

        public string CategoryInJoined { get { return string.Join(",", CategoryNameIn ?? new List<string>()); } set
        {
            SelectedCategoryTitle = value;
            CategoryNameIn = value.HasValue() ? value.Split(',').ToList() : null;
        } }

        public Location GetLocation()
        {
            return Location.Parse(Latitude, Longitude);
        }

        public OfferFilterModel()
        {
            ShowPaging = true;
            PagingInfo = new PagingInfo
            {
                CurrentPage = 1,
                ItemsPerPage = 20
            };
            //SelectedCategoryTitle = "Food & Dining";
            //CategoryNameIn = new List<string>() {"Fast Food", "Casual & Fine Dining"};
            ItemsPerPage = new SelectList(new[] {10, 20, 50, 100});
            var raduisList = new SelectList(new[] { 10, 20, 50, 100, 200, 500 }).ToList();
            raduisList.Add(new SelectListItem{Text = "No limit",Value = "", Selected = true});
            RadiusSelectList = raduisList;
            Radius = 20;
        }
    }
}