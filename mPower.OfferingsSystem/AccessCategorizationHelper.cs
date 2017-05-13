using System;
using System.Collections.Generic;
using System.Linq;

namespace mPower.OfferingsSystem
{
    public static  class AccessCategorizationHelper
    {
        public const string UncategorizedExpense = "Uncategorized Expense";

        public static Dictionary<String, String> AccessToMPowerCategoriesMapping = new Dictionary<string, string>
        {
            {"Automotive", "Auto & Transport"},
            {"Health & Beauty", "Personal Care"},
            {"Auto Service & Repair", "Service & Parts"},
            {"Services", "Shopping"},
            {"Shopping", "Shopping"},//
            {"Fast Food", "Fast Food"},//
            {"Casual & Fine Dining", "Restaurants"},
            {"Entertainment & Recreation", "Entertainment"},
            {"Golf", "Sports"},
            {"Hotel", "Hotel"},//
            {"Cleaning","Home Services"},
            {"Fitness Centers","Gym"},
            {"Specialty Foods & Gifts","Gifts"},
            {"Appliances","Furnishings"},
            {"Family Fun","Kids Activities"},
            {"Car Rental","Rental Car"},
            {"Home & Garden","Home Improvement"},
            {"Other Services",UncategorizedExpense},
            {"Cruise","Travel"},
            {"Movies","Movies"},//
            {"Equipment & Accessories",UncategorizedExpense},
            {"Apparel & Accessories","Clothing"},
            {"Flowers","Gifts"},
            {"Boutique",UncategorizedExpense},
            {"Outdoor Adventure","Amusement"},
            {"Moving & Storage","Travel"},
            {"Sporting Goods","Sporting Goods"},//
            {"Air & Parking","Travel"},
            {"Cycling","Sports"},
            {"Ski & Snowboard","Sports"},
            {"Gifts","Gifts"},//
            {"Health Products","Health & Fitness"},
            {"Vision","Eyecare"},
            {"Windows","Home Improvement"},
            {"Bowling & Arcade","Amusement"},
            {"Travel","Travel"},//
            {"Condo & Resorts","Travel"},
            {"Golf Packages","Sports"},
            {"Sporting Events","Sports"},
            {"Concerts & Events","Amusement"},
            {"Theme Parks","Amusement"},
            {"Other Shopping","Shopping"},
            {"Outdoor Equipment","Home Improvement"},
            {"Luggage & Travel Accessories","Travel"},
            {"Internet Service Provider","Bills & Utilities"},
            {"Museums & Zoos","Amusement"},
            {"Warranties & Financing",UncategorizedExpense},
            {"Locksmith",UncategorizedExpense},
            {"Office Supplies","Books & Supplies"},
            {"Medical","Health & Fitness"},
            {"Bath & Body","Spa & Massage"},
            {"Photography & Photo Printing",UncategorizedExpense},
            {"Jewelry","Gifts"},
            {"Electronics","Home Improvement"},
            {"Tours","Travel"},
            {"Travel Packages","Travel"},
            {"Fitness Equipment","Sports"},
            {"Ski Packages","Sports"},
            {"Flooring","Home Improvement"},
            {"Pets","Pets"},
            {"Convenience Stores",UncategorizedExpense},
            {"Catering","Food & Dining"},
            {"Tanning Salon","Spa & Massage"},
            {"Day Spa","Spa & Massage"},
            {"Golf Courses","Sports"},
            {"Auto Body & Paint","Auto & Transport"},
            {"Fragrance","Personal Care"},
            {"Art","Shopping"},
            {"Department Stores","Shopping"},
            {"Tax Services","Shopping"},
            {"Educational","Education"},
            {"Tuxedo Rental","Clothing"},
            {"Mail/Copy/Print",UncategorizedExpense},
            {"Car Wash & Detail","Auto & Transport"},
            {"Movies, Music & Books","Entertainment"},
            {"Other",UncategorizedExpense},
            {"Dental","Dentist"},
            {"Cosmetics & Skin Care","Health & Fitness"},
            {"Toys","Toys"},
            {"Auto Parts","Service & Parts"},
            {"Furniture","Furnishings"},
            {"Closet Organization","Home Improvement"},
            {"Cable & Satellite","Television"},
            {"Hearing","Health & Fitness"},
            {"Motel","Hotel"},
            {"Dining & Food","Food & Dining"},
            {"Cell Phone","Mobile"},
            {"Publications","Newspaper & Magazines"},
            {"Tires","Auto & Transport"},
            {"Bed & Breakfast","Travel"},
            {"Gear & Rentals","Entertainment"},
            {"Crafts","Shopping"},
            {"Hardware Stores","Home Improvement"},
            {"Bridal","Shopping"},
            {"Resorts & Spas","Travel"},
            {"Weight Management","Personal Care"},
        };

        public static IEnumerable<string> MapMPowerToAccess(string mPowerCategory, bool includeSameName)
        {
            return AccessToMPowerCategoriesMapping.Where(x => x.Value == mPowerCategory && (includeSameName || x.Key != mPowerCategory)).Select(x => x.Key);
        }

        public static string MapAccessToMPower(string accessCategory)
        {
            if (AccessToMPowerCategoriesMapping.ContainsKey(accessCategory))
            {
                return AccessToMPowerCategoriesMapping[accessCategory];
            }
            return UncategorizedExpense;
        }

        public static IEnumerable<IGrouping<string, string>> GetGroups()
        {
            return AccessToMPowerCategoriesMapping.GroupBy(x => x.Value, x => x.Key);
        }
    }
}