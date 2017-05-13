using System;
using mPower.Framework.Services;

namespace mPower.Documents.DocumentServices.Membership.Filters
{
    public enum UserLoginsSortEnum
    {
        LoginDate = 1,
        AffiliateName
    }

    public class UserLoginsFilter : BaseFilter
    {
        public string UserId { get; set; }

        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        /// <summary>
        /// user email, user name or affiliate name
        /// </summary>
        public string SearchKey { get; set; }

        public UserLoginsSortEnum SortByField { get; set; }
    }
}
