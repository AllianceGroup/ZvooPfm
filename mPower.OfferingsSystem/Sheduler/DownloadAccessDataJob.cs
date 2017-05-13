using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using mPower.Framework;
using mPower.Framework.Extensions;
using mPower.TempDocuments.Server.DocumentServices;
using mPower.TempDocuments.Server.DocumentServices.Lucene;
using mPower.TempDocuments.Server.Documents;
using mPower.TempDocuments.Server.Sql;
using Enumerable = mPower.Framework.Extensions.Enumerable;

namespace mPower.OfferingsSystem.Sheduler
{
    public class DownloadAccessDataJob
    {
        private readonly IPackageDownloader _packageDownloader;
        private readonly IOfferDocumentService _documentService;
        private readonly IOfferDocumentsBuilder _builder;
        private readonly IMerchantDocumentBuilder _merchantDocumentBuilder;
        private readonly IMerchantDocumentService _merchantDocumentService;
        private readonly IOfferLuceneService _offerLuceneService;
        private readonly IOffersRepository _offersRepository;
        private readonly IActionQueue _actionQueue = new ActionQueue();
        private readonly IActionQueue _actionQueueUpdateOfferGroups = new ActionQueue();
        private readonly IActionQueue _actionQueueOffersSql = new ActionQueue();
        private readonly IOfferGroupDocumentService _offerGroupDocumentService;
        private readonly Logger _logger = MPowerLogManager.CurrentLogger;

        public DownloadAccessDataJob(
            IPackageDownloader packageDownloader,
            IOfferDocumentService documentService, 
            IOfferDocumentsBuilder builder, 
            IMerchantDocumentBuilder merchantDocumentBuilder, 
            IMerchantDocumentService merchantDocumentService,
            IOfferGroupDocumentService offerGroupDocumentService, 
            IOfferLuceneService offerLuceneService,
            IOffersRepository offersRepository)
        {
            _packageDownloader = packageDownloader;
            _documentService = documentService;
            _builder = builder;
            _merchantDocumentBuilder = merchantDocumentBuilder;
            _merchantDocumentService = merchantDocumentService;
            _offerGroupDocumentService = offerGroupDocumentService;
            _offerLuceneService = offerLuceneService;
            _offersRepository = offersRepository;
        }

        public void Execute()
        {
            _actionQueue.Add(() =>
                                 {
                                     _logger.Info("Start pulling new Offers from Access");
                                     var packageInfo = _packageDownloader.Download();
                                     var merchants = _merchantDocumentBuilder.GetAll();
                                     var allMerchantsIds = _merchantDocumentService.GetAllIds();
                                     var newMerchants = merchants.Where(x => x.Id.HasValue() && !allMerchantsIds.Contains(x.Id));
                                     foreach (var batch in newMerchants.Batch(1000))
                                     {
                                         _merchantDocumentService.InsertMany(batch);
                                     }
                                     var offers = _builder.GetAll(packageInfo);
                                     var allOffersIds = _documentService.GetAllOffersIds();
                                     var newOffers = offers.Where(x => x.Id.HasValue() && !allOffersIds.Contains(x.Id));
                                     foreach (var batch in newOffers.Batch(1000))
                                     {
                                         _documentService.InsertMany(batch);
                                         UpdateOffersSql(batch);
                                         //UpdateOfferGroupsAsync(batch);
                                     }
                                     _logger.Info("Finish pulling new Offers from Access");
                                 });
        }

        private void UpdateOffersSql(OfferDocument[] batch)
        {
            _actionQueueOffersSql.Add(() => _offersRepository.Insert(batch));
        }

        private void UpdateOfferGroupsAsync(IEnumerable<OfferDocument> offers)
        {
            _actionQueueUpdateOfferGroups.Add(() => UpdateOfferGroups(offers.ToList()));
        }
        private void UpdateOfferGroups(IEnumerable<OfferDocument> offers)
        {
            var comparer =
                new Enumerable.PredicateEqualityComparer<GroupKey>(
                    (x, y) => x.Title == y.Title && x.Merchant == y.Merchant);
            foreach (var grouping in offers.GroupBy(k => new GroupKey { Title = k.Title, Merchant = k.Merchant }, comparer))
            {
                _offerGroupDocumentService.UpsertMany(grouping.Key.Title,grouping.Key.Merchant, grouping);
            }
        }

        private class GroupKey
        {
            public string Merchant;
            public string Title;
        }
    }
}