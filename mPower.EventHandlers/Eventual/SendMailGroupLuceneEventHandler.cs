using mPower.Documents.ExternalServices.FullTextSearch;
using mPower.Documents.Segments;
using mPower.Domain.Application.Affiliate.Data;
using mPower.Domain.Application.Affiliate.Events;
using mPower.Domain.Membership.User.Events;
using mPower.Domain.Membership.User.Messages;
using Paralect.ServiceBus;
using System.Collections.Generic;
using System.Linq;

namespace mPower.EventHandlers.Eventual
{
    public class SendMailGroupLuceneEventHandler :
        IMessageHandler<User_CreatedEvent>,
        IMessageHandler<User_UpdatedMessage>,
        IMessageHandler<User_DeletedEvent>,
        IMessageHandler<Affiliate_Segment_AddedEvent>,
        IMessageHandler<Affiliate_Segment_ChangedEvent>,
        IMessageHandler<Affiliate_Segment_DeletedEvent>
    {
        private readonly SendMailGroupLuceneService _sendMailGroupLuceneService;
        private readonly SegmentAggregationService _segmentAggregationService;

        public SendMailGroupLuceneEventHandler(SendMailGroupLuceneService sendMailGroupLuceneService,
            SegmentAggregationService segmentAggregationService)
        {
            _sendMailGroupLuceneService = sendMailGroupLuceneService;
            _segmentAggregationService = segmentAggregationService;
        }

        public void Handle(User_CreatedEvent message)
        {
            _sendMailGroupLuceneService.AddUser(message.UserId, message.Email, message.FirstName, message.LastName, message.ApplicationId);
        }

        public void Handle(User_UpdatedMessage message)
        {
            _sendMailGroupLuceneService.UpdateUser(message.UserId, message.Email, message.FirstName, message.LastName, message.ApplicationId);
        }

        public void Handle(User_DeletedEvent message)
        {
            _sendMailGroupLuceneService.DeleteDocuments(message.UserId);
        }

        public void Handle(Affiliate_Segment_AddedEvent message)
        {
            _sendMailGroupLuceneService.AddSegment(message.Id, message.Name, GetAffiliateSegmentUsersIds(message, message.AffiliateId), message.AffiliateId);
        }

        public void Handle(Affiliate_Segment_ChangedEvent message)
        {
            _sendMailGroupLuceneService.UpdateSegment(message.Id, message.Name, GetAffiliateSegmentUsersIds(message, message.AffiliateId), message.AffiliateId);
        }

        public void Handle(Affiliate_Segment_DeletedEvent message)
        {
            _sendMailGroupLuceneService.DeleteDocuments(message.Id);
        }

        private List<string> GetAffiliateSegmentUsersIds(Affiliate_Segment_ChangedEvent message, string affiliateId)
        {
            return GetAffiliateSegmentUsersIds(new SegmentData
            {
                AffiliateId = message.AffiliateId,
                AgeRangeFrom = message.AgeRangeFrom,
                AgeRangeTo = message.AgeRangeTo,
                CustomDateRangeEnd = message.CustomDateRangeEnd,
                CustomDateRangeStart = message.CustomDateRangeStart,
                Name = message.Name,
                Id = message.Id,
                DateRange = message.DateRange,
                Gender = message.Gender,
                Options = message.Options,
                State = message.State,
                ZipCodes = message.ZipCodes,
                MerchantSelections = message.MerchantSelections,
                SpendingCategories = message.SpendingCategories,
            }, affiliateId);
        }

        private List<string> GetAffiliateSegmentUsersIds(Affiliate_Segment_AddedEvent message, string affiliateId)
        {
            return GetAffiliateSegmentUsersIds(new SegmentData
            {
                AffiliateId = message.AffiliateId,
                AgeRangeFrom = message.AgeRangeFrom,
                AgeRangeTo = message.AgeRangeTo,
                CustomDateRangeEnd = message.CustomDateRangeEnd,
                CustomDateRangeStart = message.CustomDateRangeStart,
                Name = message.Name,
                Id = message.Id,
                DateRange = message.DateRange,
                Gender = message.Gender,
                Options = message.Options,
                State = message.State,
                ZipCodes = message.ZipCodes,
                MerchantSelections = message.MerchantSelections,
                SpendingCategories = message.SpendingCategories,
            }, affiliateId);
        }

        private List<string> GetAffiliateSegmentUsersIds(SegmentData segment, string affiliateId)
        {
            return _segmentAggregationService.GetSegmentUser(segment)
                .Where(x => x.AffiliateId == affiliateId)
                .Select(x => x.Id).ToList();
        }
    }
}
