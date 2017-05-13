using System;
using Paralect.Domain;
using mPower.Domain.Yodlee.ContentServiceItem.Data;

namespace mPower.Domain.Yodlee.ContentServiceItem.Events
{
    [Obsolete]
    public class ContentServiceItem_AddedEvent : Event
    {
        public string AuthenticationReferenceId { get; set; }
        public ContentServiceItemData Data { get; set; }
    }
}