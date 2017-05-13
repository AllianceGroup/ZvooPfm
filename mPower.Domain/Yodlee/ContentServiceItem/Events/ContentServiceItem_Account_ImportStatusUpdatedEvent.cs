using System;
using Paralect.Domain;
using mPower.Domain.Accounting.Enums;

namespace mPower.Domain.Yodlee.ContentServiceItem.Events
{
    [Obsolete]
    public class ContentServiceItem_Account_ImportStatusUpdatedEvent : Event
    {
        public string LedgerAccountId { get; set; }
        public string LedgerId { get; set; }
        public string ContentServiceItemAccountId { get; set; }
        public string ItemId { get; set; }
        public ImportStatusEnum ImportStatus { get; set; }
    }
}