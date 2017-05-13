using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace mPower.Framework.Mvc
{
    public static class Extensions
    {
        public static Double ToDouble(this String numberString)
        {
            return Double.Parse(numberString);
        }

        public static string StripNonAlphaCharacters(this string description)
        {
            return Regex.Replace(description, @"[^a-zA-Z]", "");
        }

        public static String DeriveAccountFromDescription(this Dictionary<string, string> keywords, string description)
        {
            if (keywords == null || String.IsNullOrEmpty(description))
                return null;
            
            var cleanDescription = description.StripNonAlphaCharacters().RemoveGeneralPurchaseWords();

            return (from keyword in keywords
                    where cleanDescription.IndexOf(keyword.Key.StripNonAlphaCharacters(), StringComparison.OrdinalIgnoreCase) >= 0
                    select keyword.Value).FirstOrDefault();
        }

        public static String DeriveKeywordFromDescription(this string description)
        {
            var keyword = description.StripNonAlphaCharacters().RemoveGeneralPurchaseWords();

            return keyword.Length > 12 ? keyword.Substring(0, 12) : keyword;
        }

        public static string RemoveGeneralPurchaseWords(this string text)
        {
            var commonPurchaseWords = new[]
                       {
                           "cardpurchase",
                           "debit",
                           "pospurchase",
                           "achwithdrawal",
                           "paymentto",
                           "visacheckpos",
                           "visapurchasenonpin",
                           "checkcardpurchase",
                           "checkcrdpurchase",
                           "chkcard",
                           "atmwithdrawal"
                       };

            text = text.ToLower();


            foreach (var word in commonPurchaseWords)
            {
                text = text.Replace(word, string.Empty);
            }

            return text;
        }


    }
}
