using System;
using System.Collections.Generic;
using mPower.Domain.Yodlee.Enums;

namespace mPower.Domain.Yodlee.ContentServiceItem.Data
{
    public class ContentServiceItemData
    {
        public DataUpdateAttemptStatus DataUpdateAttemptStatus { get; set; }
        public ContentServiceRefreshErrorCode ErrorCode { get; set; }
        public UserActionRequiredEnum ActionRequired { get; set; }
        public ResponseCodeEnum ResponseCode { get; set; }
        public string ItemId { get; set; }
        public string UserId { get; set; }
        public string ContentServiceId { get; set; }
        public string DisplayName { get; set; }
        public DateTime LastSuccessfulUpdate { get; set; }
        public DateTime LastUpdateAttempt { get; set; }
        public RefreshStatus RefreshStatus { get; set; }
        public ItemAccessStatusEnum AccessStatus { get; set; }
        public List<ContentServiceItemAccount> Accounts { get; set; }
        
    }
}