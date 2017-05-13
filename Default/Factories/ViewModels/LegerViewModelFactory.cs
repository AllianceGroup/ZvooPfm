using Default.ViewModel.Areas.Business.BusinessController;
using Default.ViewModel.Areas.Finance.BudgetController.Filters;
using Default.ViewModel.Areas.Shared;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Documents.Documents.Affiliate;
using mPower.Documents.DocumentServices;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.DocumentServices.Accounting.Filters;
using mPower.Documents.ExternalServices.FullTextSearch;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Application.Affiliate.Commands;
using mPower.Domain.Application.Affiliate.Data;
using mPower.Domain.Application.Enums;
using mPower.Framework;
using mPower.Framework.Mvc;
using mPower.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using BudgetFilter = mPower.Documents.DocumentServices.Accounting.Filters.BudgetFilter;

namespace Default.Factories.ViewModels
{
    internal class CampaignData : CampaignDocument
    {
        public AffiliateDocument Affiliate { get; private set; }

        public CampaignData(CampaignDocument doc, AffiliateDocument affiliate)
        {
            Affiliate = affiliate;
            Id = doc.Id;
            SegmentId = doc.SegmentId;
            Offer = doc.Offer;
            Settings = doc.Settings;
            Statistic = doc.Statistic;
        }
    }

    public class LegerViewModelFactory :
        IObjectFactory<string, List<Category>>,
        IObjectFactory<TransactionClientFilter, List<Entry>>,
        IObjectFactory<BuildAllEntriesFilter, List<Entry>>,
        IObjectFactory<AccountDocument, BusinessAccount>
    {
        private readonly AffiliateDocumentService _affiliatesService;
        private readonly LedgerDocumentService _ledgerService;
        private readonly EntryDocumentService _entryDocumentService;
        private readonly TransactionLuceneService _transactionLuceneService;
        private readonly UploadUtil _uploadUtil;
        private readonly CommandService _cmdService;
        private readonly BudgetDocumentService _budgetDocumentService;

        public LegerViewModelFactory(
            AffiliateDocumentService affiliatesService,
            LedgerDocumentService ledgerService,
            EntryDocumentService entryDocumentService,
            BudgetDocumentService budgetDocumentService,
            TransactionLuceneService transactionLuceneService,
            UploadUtil uploadUtil,
            CommandService cmdService)
        {
            _affiliatesService = affiliatesService;
            _ledgerService = ledgerService;
            _entryDocumentService = entryDocumentService;
            _transactionLuceneService = transactionLuceneService;
            _uploadUtil = uploadUtil;
            _cmdService = cmdService;
            _budgetDocumentService = budgetDocumentService;
        }

        //Load all catigories for scpecified ledger
        public List<Category> Load(string ledgerId)
        {
            var accounts = _ledgerService.GetById(ledgerId).Accounts.OrderBy(x => x.Order).ThenBy(x => x.TypeEnum).ThenBy(x => x.Name);

            return (from a in accounts
                    select new Category
                               {
                                   AccountId = a.Id,
                                   AccountLabel = a.LabelEnum,
                                   AccountName = a.Name,
                                   AccountType = a.TypeEnum,
                                   ParentAccountId = a.ParentAccountId
                               }).ToList();
        }

        public List<Entry> Load(TransactionClientFilter clientFilter)
        {
            var filter = BuildLuceneSearchFilter(clientFilter);

            return PrepareForView(_transactionLuceneService.SearchByQuery(filter), clientFilter.UserId, clientFilter.affiliateId);
        }

        public EntryLuceneFilter BuildLuceneSearchFilter(TransactionClientFilter clientFilter)
        {
            var filter = new EntryLuceneFilter {LedgerId = clientFilter.ledgerId, PagingInfo = clientFilter.Paging};
            if (!String.IsNullOrEmpty(clientFilter.accountId))
            {
                if (clientFilter.IncludeSubAccounts)
                {
                    var ledger = _ledgerService.GetById(clientFilter.ledgerId);
                    var subIds = ledger.Accounts.Where(x => x.ParentAccountId == clientFilter.accountId).Select(x => x.Id);
                    filter.AccountIds.AddRange(subIds);
                    filter.AccountIds.Add(clientFilter.accountId);
                }
                else
                {
                    filter.AccountId = clientFilter.accountId;
                }
            }
            decimal res;
            decimal.TryParse(clientFilter.request, out res);
            if (res != 0)
            {
                const decimal balanceRange = 0.2m;
                filter.MinEntryAmount = (long)((res * (1 - balanceRange) * 100));
                filter.MaxEntryAmount = (long)((res * (1 + balanceRange) * 100));
            }
            else if (!String.IsNullOrEmpty(clientFilter.request))
            {
                filter.SearchText = clientFilter.request;
            }

            if (clientFilter.ParsedLabel != null)
            {
                filter.AccountLabels = new List<AccountLabelEnum> {clientFilter.ParsedLabel.Value};
            }

            if (!String.IsNullOrEmpty(clientFilter.from) || !String.IsNullOrEmpty(clientFilter.to))
            {
                DateTime toDate;
                DateTime.TryParse(clientFilter.to, out toDate);
                toDate = DateUtil.GetEndOfDay(toDate);
                DateTime fromDate;
                DateTime.TryParse(clientFilter.from, out fromDate);
                fromDate = DateUtil.GetStartOfDay(fromDate);
                filter.BookedDateMinValue = fromDate;
                filter.BookedDateMaxValue = toDate;

                //integration with budgets, to show all expense/income transactions
                if (clientFilter.ParsedBudgetType != null)
                {
                    var budgets = _budgetDocumentService.GetByFilter(new BudgetFilter
                    {
                        Year = toDate.Year,
                        Month = toDate.Month,
                        AccountType = clientFilter.ParsedBudgetType
                    });
                    var parentAccountIds = budgets.Select(x => x.AccountId);
                    var ledger = _ledgerService.GetById(clientFilter.ledgerId);
                    var subIds = ledger.Accounts.Where(x => parentAccountIds.Contains(x.ParentAccountId)).Select(x => x.Id);
                    filter.AccountIds.AddRange(parentAccountIds);
                    filter.AccountIds.AddRange(subIds);
                }
            }
            return filter;
        }

        public List<Entry> Load(BuildAllEntriesFilter filter)
        {
            var entryFilter = new EntryFilter
            {
                LedgerId = filter.LedgerId,
                SortByFiled = EntrySortFieldEnum.BookedDate,
                AccountLabel = new List<AccountLabelEnum> {AccountLabelEnum.Bank, AccountLabelEnum.CreditCard},
                PagingInfo = filter.Paging //new PagingInfo() { CurrentPage = page, ItemsPerPage = itemsPerPage,},
            };

            var rawEntries = _entryDocumentService.GetByFilter(entryFilter);

            return PrepareForView(rawEntries, filter.UserId, filter.AffiliateId);
        }

        public BusinessAccount Load(AccountDocument account)
        {
            return new BusinessAccount
            {
                BalanceInDollars = AccountingFormatter.CentsToDollars(account.Denormalized.Balance),
                LastUpdatedAgo = DateTime.Now.Subtract(account.DateLastAggregated).ToShortTimeString(),
                Name = account.Name,
                Id = account.Id,
                Order = account.Order,
                IsUpdating = 
                    account.AggregatedAccountStatus == AggregatedAccountStatusEnum.BeginPullingTransactions ||
                    account.AggregatedAccountStatus == AggregatedAccountStatusEnum.PullingTransactions,
                IsUserAttentionRequired = account.AggregatedAccountStatus > 0 &&
                                          account.AggregatedAccountStatus != AggregatedAccountStatusEnum.Normal &&    
                                          account.AggregatedAccountStatus != AggregatedAccountStatusEnum.PullingTransactions &&
                                          account.AggregatedAccountStatus != AggregatedAccountStatusEnum.BeginPullingTransactions,
                AggregatedBalanceInDollars = AccountingFormatter.CentsToDollars(account.ActualBalance),
                AccountStatus = GetAccountStatus(account.AggregatedAccountStatus),
                DisplayName = string.IsNullOrEmpty(account.InstitutionName) ? "Other" : account.InstitutionName,
                IsAggregatedAccount = account.Imported,
                IntuitAccountId = account.IntuitAccountId,
                IntuitInstitutionId = account.IntuitInstitutionId
            };
        }

        private string GetAccountStatus(AggregatedAccountStatusEnum c)
        {
            string status = null;
            switch (c)
            {
                case AggregatedAccountStatusEnum.UnexpectedErrorOccurred:
                    status = "Unexpected Error Occurred";
                    break;
                case AggregatedAccountStatusEnum.NeedInteractiveRefresh:
                    status = "Need Interactive Refresh";
                    break;
                case AggregatedAccountStatusEnum.NeedReauthentication:
                    status = "Need Reauthentication";
                    break;
            }
            return status;
        }

        private List<Entry> PrepareForView(IEnumerable<EntryDocument> rawEntries, string userId, string affiliateId)
        {
            var affiliate = _affiliatesService.GetById(affiliateId);
            var mappedEntries = rawEntries.Select(x => Map(x, userId, affiliate)).ToList();
            TrackShownOffers(mappedEntries, userId, affiliateId);

            return mappedEntries;
        }

        private Entry Map(EntryDocument doc, string userId, AffiliateDocument affiliate)
        {
            var entry = new Entry
            {
                AccountId = doc.AccountId,
                OffsetAccountName = doc.OffsetAccountName,
                OffsetAccountId = doc.OffsetAccountId,
                BookedDate = doc.BookedDate,
                FormattedAmountInDollars = doc.FormattedAmountInDollars,
                Payee = doc.Payee,
                TransactionId = doc.TransactionId,
                AccountName = doc.AccountName.Ellipsize(25),
                Memo = doc.Memo,
            };
            if (affiliate.MarketingEnabled)
            {
                entry.Offers = GetMatchingOffers(affiliate, userId, doc);
            }

            return entry;
        }

        private void TrackShownOffers(IEnumerable<Entry> entries, string userId, string affiliateId)
        {
            // if offer is shown for 2 transactions it will be counted twice
            var shownOffers = entries.SelectMany(e => e.Offers).GroupBy(x => x.AffiliateId)
                .ToDictionary(g => g.Key, g => g.Select(off => off.CampaignId).ToList());

            if (shownOffers.Any())
            {
                _cmdService.SendAsync(new Affiliate_Offers_ShowToUserCommand
                {
                    UserId = userId,
                    UserAffiliateId = affiliateId,
                    ShownAffiliateOffers = shownOffers,
                    Metadata = {UserId = userId}
                });
            }
        }

        private List<InlineOffer> GetMatchingOffers(AffiliateDocument affiliate, string userId, EntryDocument doc)
        {
            var entryAmount = AccountingFormatter.FormatDebitCreditToPositiveOrNegativeNumberByAccountLabel(doc.DebitAmountInCents, doc.CreditAmountInCents, doc.AccountLabel);
            var offers = GetAvailableCampaigns(affiliate)
                .Where(camp => 
                    // search within active inline transactions' offers
                    camp.GetCurrentStatus() == OfferStatusEnum.Active && camp.Offer != null && camp.Offer.OfferType == OfferTypeEnum.InlineTransaction 
                    // check that user belong to campaign-related segment
                    && camp.Affiliate.Segments.Single(segm => segm.Id == camp.SegmentId).MatchedUsers.Any(x => x.Id == userId && x.AffiliateId == affiliate.ApplicationId)
                    // check that entry matches by Merchant or Spending Category
                    && camp.Settings != null && (camp.MatchesMerchantName(doc.Payee) || camp.MatchesCategory(doc.AccountName, doc.OffsetAccountName))
                    && !camp.Statistic.AcceptedByUsers.Contains(userId))
                .Select(x => Map(x, entryAmount)).ToList();

            if (offers.Count > 1)
            {
                var maxOfferValue = offers.Max(x => x.OfferValueInCents);
                var result = new List<InlineOffer> {offers.First(x => x.OfferValueInCents == maxOfferValue)};
                return result;
            }
            return offers;
        }

        private IEnumerable<CampaignData> GetAvailableCampaigns(AffiliateDocument affiliate)
        {
            return affiliate.Campaigns.Select(x => new CampaignData(x, affiliate))
                .Concat(affiliate.NetworkCampaigns.SelectMany(nc =>
                {
                    var anotherAffiliate = _affiliatesService.GetById(nc.Key);
                    if (anotherAffiliate != null)
                    {
                        return anotherAffiliate.Campaigns.Where(c => nc.Value.Contains(c.Id))
                            .Select(x => new CampaignData(x, anotherAffiliate));
                    }
                    return new List<CampaignData>();
                }));
        }

        private InlineOffer Map(CampaignData data, long entryAmountInCents)
        {
            return new InlineOffer
            {
                AffiliateId = data.Affiliate.ApplicationId,
                CampaignId = data.Id,
                Headline = data.Offer.Headline,
                Body = data.Offer.Body,
                LogoPath = _uploadUtil.GetCampaigLogoUrl(data.Affiliate.ApplicationName, data.Id, data.Offer.Logo),
                ExpirationDate = data.Offer.ExpirationDate,
                Terms = data.Offer.Terms,
                OfferValueInCents = GetOfferValueForEntry(entryAmountInCents, data.Offer),
            };
        }

        private static long GetOfferValueForEntry(long entryAmountInCents, CampaignOfferData doc)
        {
            var offerValuePercToCents = (long) Math.Round(Math.Abs(entryAmountInCents)*(decimal)(doc.OfferValueInPerc ?? 0)/100, 0);
            return Math.Max(doc.OfferValueInCents ?? 0, offerValuePercToCents);
        }
    }
}
