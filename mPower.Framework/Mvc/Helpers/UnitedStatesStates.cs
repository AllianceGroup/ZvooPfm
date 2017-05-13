﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace System.Web.Mvc
{
    public class UnitedStatesStates
    {
        public static readonly IDictionary<string, string> StateDictionary = new Dictionary<string, string> {
        {"ALABAMA", "AL"},
        {"ALASKA", "AK"},
        {"AMERICAN SAMOA", "AS"},
        {"ARIZONA ", "AZ"},
        {"ARKANSAS", "AR"},
        {"CALIFORNIA ", "CA"},
        {"COLORADO ", "CO"},
        {"CONNECTICUT", "CT"},
        {"DELAWARE", "DE"},
        {"DISTRICT OF COLUMBIA", "DC"},
        {"FEDERATED STATES OF MICRONESIA", "FM"},
        {"FLORIDA", "FL"},
        {"GEORGIA", "GA"},
        {"GUAM ", "GU"},
        {"HAWAII", "HI"},
        {"IDAHO", "ID"},
        {"ILLINOIS", "IL"},
        {"INDIANA", "IN"},
        {"IOWA", "IA"},
        {"KANSAS", "KS"},
        {"KENTUCKY", "KY"},
        {"LOUISIANA", "LA"},
        {"MAINE", "ME"},
        {"MARSHALL ISLANDS", "MH"},
        {"MARYLAND", "MD"},
        {"MASSACHUSETTS", "MA"},
        {"MICHIGAN", "MI"},
        {"MINNESOTA", "MN"},
        {"MISSISSIPPI", "MS"},
        {"MISSOURI", "MO"},
        {"MONTANA", "MT"},
        {"NEBRASKA", "NE"},
        {"NEVADA", "NV"},
        {"NEW HAMPSHIRE", "NH"},
        {"NEW JERSEY", "NJ"},
        {"NEW MEXICO", "NM"},
        {"NEW YORK", "NY"},
        {"NORTH CAROLINA", "NC"},
        {"NORTH DAKOTA", "ND"},
        {"NORTHERN MARIANA ISLANDS", "MP"},
        {"OHIO", "OH"},
        {"OKLAHOMA", "OK"},
        {"OREGON", "OR"},
        {"PALAU", "PW"},
        {"PENNSYLVANIA", "PA"},
        {"PUERTO RICO", "PR"},
        {"RHODE ISLAND", "RI"},
        {"SOUTH CAROLINA", "SC"},
        {"SOUTH DAKOTA", "SD"},
        {"TENNESSEE", "TN"},
        {"TEXAS", "TX"},
        {"UTAH", "UT"},
        {"VERMONT", "VT"},
        {"VIRGIN ISLANDS", "VI"},
        {"VIRGINIA", "VA"},
        {"WASHINGTON", "WA"},
        {"WEST VIRGINIA", "WV"},
        {"WISCONSIN", "WI"},
        {"WYOMING", "WY"}
    };

        public static SelectList StateSelectList
        {
            get { return new SelectList(StateDictionary, "Value", "Key"); }
        }
    }
}
