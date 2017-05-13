using System;
using System.Collections.Generic;
using Paralect.Domain;
using mPower.Domain.Yodlee.ContentServiceItem.Data;
using mPower.Domain.Yodlee.Enums;

namespace mPower.Domain.Yodlee.ContentServiceItem.Events
{
    [Obsolete]
    public class ContentServiceItem_UpdatedEvent : Event
    {
        public DataUpdateAttemptStatus DataUpdateAttemptStatus { get; set; }
        public string DisplayName { get; set; }
        public ResponseCodeEnum ResponseCode { get; set; }
        public ContentServiceRefreshErrorCode ErrorCode { get; set; }
        public List<ContentServiceItemAccount> Accounts { get; set; }
        public ItemAccessStatusEnum AccessStatus { get; set; }
        public UserActionRequiredEnum ActionRequired { get; set; }
        public string ItemId { get; set; }
        public DateTime LastSuccessfulUpdate { get; set; }
        public DateTime LastUpdateAttempt { get; set; }
        
        [Obsolete("Does not get updated via a content service item update event")]
        public RefreshStatus RefreshStatus { get; set; } // D
        
    }
}