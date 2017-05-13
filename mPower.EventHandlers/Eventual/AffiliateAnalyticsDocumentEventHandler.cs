using System;
using System.Linq;
using mPower.Documents.Documents.Affiliate;
using mPower.Documents.DocumentServices;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.DocumentServices.Membership;
using mPower.Documents.Documents.Membership;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Ledger.Events;
using mPower.Domain.Accounting.Ledger.Messages;
using mPower.Domain.Application.Affiliate.Events;
using mPower.EventHandlers.Eventual.Segments;
using Paralect.ServiceBus;

namespace mPower.EventHandlers.Eventual
{
    public class AffiliateAnalyticsDocumentEventHandler: 
        IMessageHandler<Ledger_Account_BalanceChangedEvent>,
        IMessageHandler<Ledger_Account_AggregatedBalanceUpdatedMessage>,
        IMessageHandler<Affiliate_CreatedEvent>

    {
        private readonly AffiliateAnalyticsDocumentService _affiliateAnalyticsDocumentService;
        private readonly UserDocumentService _userDocumentService;
        private readonly LedgerDocumentService _ledgerDocumentService;

        public AffiliateAnalyticsDocumentEventHandler(AffiliateAnalyticsDocumentService affiliateAnalyticsDocumentService, UserDocumentService userDocumentService, LedgerDocumentService ledgerDocumentService)
        {
            _affiliateAnalyticsDocumentService = affiliateAnalyticsDocumentService;
            _userDocumentService = userDocumentService;
            _ledgerDocumentService = ledgerDocumentService;
        }

        public void Handle(Ledger_Account_BalanceChangedEvent message)
        {
            var user = _userDocumentService.GetById(message.UserId);
            if (user == null || string.IsNullOrEmpty(user.ApplicationId))
                return;

            var dto = new AffiliateBalanceChangedDto
                          {
                              AccountId = message.AccountId,
                              Date = message.Date,
                              AccountName = message.AccountName,
                              BalanceInCents = message.BalanceInCents,
                              LabelEnum = message.AccountLabel,
                              LedgerId = message.LedgerId,
                              OldValueInCents = message.OldValueInCents,
                              UserId = message.UserId
                          };
            SaveAffiliateStatistic(user.ApplicationId, dto);

        }

        public void Handle(Ledger_Account_AggregatedBalanceUpdatedMessage message)
        {
            var userId = message.UserId ?? _ledgerDocumentService.GetById(message.LedgerId).Users.First().Id;
            if (string.IsNullOrEmpty(userId))
                return;

            var user = _userDocumentService.GetById(userId);
            if (user == null || string.IsNullOrEmpty(user.ApplicationId))
                return;

            var dto = new AffiliateBalanceChangedDto
            {
                AccountId = message.AccountId,
                Date = message.Date,
                AccountName = message.AccountName,
                BalanceInCents = message.NewBalance,
                LabelEnum = message.LabelEnum,
                LedgerId = message.LedgerId,
                OldValueInCents = message.OldValueInCents,
                UserId = message.UserId
            };
            SaveAffiliateStatistic(user.ApplicationId, dto);
        }

        private void SaveAffiliateStatistic(string id, AffiliateBalanceChangedDto dto)
        {
            Update(id, (doc) =>
            {
                var value = dto.BalanceInCents - dto.OldValueInCents;
                var date = dto.Date;
                switch (dto.LabelEnum)
                {
                    case AccountLabelEnum.Bank:
                        SaveTotalMoneyManagedStatistic(value, date, doc);
                        SaveAvailableCashStatisctics(value, dto.UserId, date, doc);
                        break;
                    case AccountLabelEnum.CreditCard:
                        SaveTotalMoneyManagedStatistic(value, date, doc);
                        SaveUserDebtStatisctics(value, dto.UserId, date, doc);
                        SaveAvailableCreditStatisctics(dto, doc);
                        break;
                    case AccountLabelEnum.Income:
                        SaveAvgUserAnnualIncomeStatistic(value, dto.UserId, date, doc);
                        break;
                    case AccountLabelEnum.Loan:
                        SaveTotalMoneyManagedStatistic(value, date, doc);
                        SaveUserDebtStatisctics(value, dto.UserId, date, doc);
                        break;
                    case AccountLabelEnum.Investment:
                        SaveTotalMoneyManagedStatistic(value, date, doc);
                        break;
                }
            });
        }

        private void SaveAvgUserAnnualIncomeStatistic(long value, string userId, DateTime date, AffiliateAnalyticsDocument doc)
        {
            if (!doc.UserIncomeStatisctics.ContainsKey(userId))
            {
                doc.UserIncomeStatisctics[userId] = new SpentStatiscticData();
            }
            doc.UserIncomeStatisctics[userId].AddStats(date, value);
        }

        private void SaveTotalMoneyManagedStatistic(long value, DateTime date, AffiliateAnalyticsDocument doc)
        {
            doc.BalanceAdjustmentStatisctics.AddStats(date, value);
        }

        private void SaveUserDebtStatisctics(long value, string userId, DateTime date, AffiliateAnalyticsDocument doc)
        {
            if (userId == null)
                return;
            if (!doc.UserDebtStatisctics.ContainsKey(userId))
            {
                doc.UserDebtStatisctics[userId] = new SpentStatiscticData();
            }
            doc.UserDebtStatisctics[userId].AddStats(date, value);
        }

        private void SaveAvailableCashStatisctics(long value, string userId, DateTime date, AffiliateAnalyticsDocument doc)
        {
            if (userId == null)
                return;
            if (!doc.AvailableCashStatisctics.ContainsKey(userId))
            {
                doc.AvailableCashStatisctics[userId] = new SpentStatiscticData();
            }
            doc.AvailableCashStatisctics[userId].AddStats(date, value);
        }

        private void SaveAvailableCreditStatisctics(AffiliateBalanceChangedDto dto, AffiliateAnalyticsDocument doc)
        {
            var account = _ledgerDocumentService.GetById(dto.LedgerId).Accounts.Single(x => x.Id == dto.AccountId);
            dto.CreditLimitInCents = account.CreditLimitInCents;
            var value = Math.Min(dto.CreditLimitInCents, dto.OldValueInCents) -
                        Math.Min(dto.CreditLimitInCents, dto.BalanceInCents);
            if (value == 0)
            {
                return;
            }
            doc.AvailableCreditStatisctics.AddStats(dto.Date, value);
        }

        private void Update(string id, Action<AffiliateAnalyticsDocument> updater)
        {
            var doc = _affiliateAnalyticsDocumentService.GetById(id);

            if (doc != null)
            {
                updater(doc);
                _affiliateAnalyticsDocumentService.Save(doc);
            }
        }


        public void Handle(Affiliate_CreatedEvent message)
        {
            var doc = new AffiliateAnalyticsDocument
                          {
                              Id = message.Id
                          };
            _affiliateAnalyticsDocumentService.Save(doc);
        }
    }

    public class AffiliateBalanceChangedDto: BalanceChangedDto
    {
        public string UserId { get; set; }
    }
}