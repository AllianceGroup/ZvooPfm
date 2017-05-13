using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CsvHelper;
using MongoDB.Driver.Builders;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.DocumentServices.Accounting.Reports;
using mPower.Domain.Accounting.Ledger.Commands;
using mPower.Domain.Accounting.Ledger.Events;
using mPower.Framework;
using mPower.Framework.Extensions;
using mPower.Framework.Geo;
using mPower.OfferingsSystem.Sheduler;
using mPower.TempDocuments.Server.DocumentServices;
using mPower.TempDocuments.Server.DocumentServices.Lucene;
using mPower.TempDocuments.Server.Documents;
using mPower.TempDocuments.Server.Sql;

namespace mPower.Web.Admin.Controllers
{
    public class ReadModelUpdatersController : BaseAdminController
    {
        private readonly BusinessReportDocumentService _businessReportDocumentService;
        private readonly ZipCodeDocumentService _zipCodeDocumentService;
        private readonly MongoTemp _temp;
        private readonly LedgerDocumentService _ledgerDocumentService;
        private readonly IEventService _eventService;
        private readonly DownloadAccessDataJob _job;
        private readonly MerchantDocumentService _merchantDocumentService;
        private readonly OfferDocumentService _offerDocumentService;
        private readonly OfferGroupDocumentService _offerGroupDocumentService;
        private readonly OfferLuceneService _offerLuceneService;
        private readonly OffersRepository _offersRepository;

        public ReadModelUpdatersController(BusinessReportDocumentService businessReportDocumentService, ZipCodeDocumentService zipCodeDocumentService, MongoTemp temp,
            LedgerDocumentService ledgerDocumentService, IEventService eventService,
            DownloadAccessDataJob job, MerchantDocumentService merchantDocumentService, OfferDocumentService offerDocumentService, OfferLuceneService offerLuceneService, OfferGroupDocumentService offerGroupDocumentService, OffersRepository offersRepository)
        {
            _businessReportDocumentService = businessReportDocumentService;
            _zipCodeDocumentService = zipCodeDocumentService;
            _temp = temp;
            _ledgerDocumentService = ledgerDocumentService;
            _eventService = eventService;
            _job = job;
            _merchantDocumentService = merchantDocumentService;
            _offerDocumentService = offerDocumentService;
            _offerLuceneService = offerLuceneService;
            _offerGroupDocumentService = offerGroupDocumentService;
            _offersRepository = offersRepository;
        }

        public ActionResult Index(string message)
        {
            return View((object)message);
        }


        [HttpPost]
        public ActionResult PullOffers(bool dropMerchants, bool dropOffers)
        {
            if (dropMerchants)
            {
                _merchantDocumentService.RemoveAll();
            }
            if(dropOffers)
            {
                _offerDocumentService.RemoveAll();
                _offerGroupDocumentService.RemoveAll();
                _offerLuceneService.RemoveIndexFromDisc();
                _offersRepository.RemoveAll();
                _temp.EnsureIndexes();
            }
            _job.Execute();
            return RedirectToAction("Index", new {message = "Offers pulling from Access started!"});
        }

        public ActionResult UpdateLedgersAccountBalance(string ledgerId)
        {
            var ledgers = new List<LedgerDocument>();

            if (!String.IsNullOrEmpty(ledgerId))
            {
                ledgers.Add(_ledgerDocumentService.GetById(ledgerId));
            }
            else
            {
                ledgers.AddRange(_ledgerDocumentService.GetAll());
            }

            foreach (var ledger in ledgers)
            {
                foreach (var account in ledger.Accounts)
                {
                    var balance = _businessReportDocumentService.GetAccountBalance(ledger.Id, account.Id, null, null);

                    var evt = new Ledger_Account_BalanceChangedEvent
                    {
                        LedgerId = ledger.Id,
                        AccountId = account.Id,
                        AccountName = account.Name,
                        AccountLabel = account.LabelEnum,
                        UserId = "DevAdmin",
                        BalanceInCents = balance,
                        OldValueInCents = balance,
                        Date = DateTime.Now,
                    };

                    _eventService.Send(evt);
                }
            }

            return RedirectToAction("Index", new { message = "Balance of ledger(s) was updated" });
        }

        public ActionResult RemoveLedger(string ledgerId)
        {
            var ledger = _ledgerDocumentService.GetById(ledgerId);

            if (ledger != null)
            {
                var command = new Ledger_DeleteCommand {LedgerId = ledger.Id};
                Send(command);

                return RedirectToAction("Index", new { message = "Ledger was removed" });
            }

            return RedirectToAction("Index", new { message = "Can't find ledger with id specified" });
        }

        [HttpPost]
        public ActionResult ImportZipCodes(ImportModel model)
        {
            var zipCodes = ReadZipCodesAnotherFormat(model.File.InputStream);
            _zipCodeDocumentService.Remove(Query.Null);
            foreach (var batch in zipCodes.Batch(1000))
            {
                _zipCodeDocumentService.InsertMany(batch);
            }
            return Content("Success");
        }

        private IEnumerable<ZipCodeDocument> ReadZipCodes(Stream stream)
        {
            using (var parser = new CsvParser(new StreamReader(stream)))
            {
                var first = true;
                while (stream.Position != stream.Length)
                {
                    var record = parser.Read();
                    if (first)
                    {
                        first = false;
                        continue;
                    }
                    if (record.Length == 7 && record.All(x => !string.IsNullOrEmpty(x)))
                    {
                        var doc = new ZipCodeDocument
                                      {
                                          Id = record[0],
                                          City = record[1],
                                          State = record[2],
                                          Location = Location.Parse(record[3], record[4]),
                                          TimeZone = int.Parse(record[5])
                                      };
                        yield return doc;
                    }
                }
            }
        }

        private IEnumerable<ZipCodeDocument> ReadZipCodesAnotherFormat(Stream stream)
        {
            using (var parser = new CsvParser(new StreamReader(stream)))
            {
                while (stream.Position != stream.Length)
                {
                    var record = parser.Read();

                    if (record.Length == 7 && record.All(x => !string.IsNullOrEmpty(x)))
                    {
                        var doc = new ZipCodeDocument
                        {
                            Id = record[0],
                            City = record[3],
                            State = record[4],
                            Location = Location.Parse(record[1], record[2])
                        };
                        yield return doc;
                    }
                }
            }
        }

        public class ImportModel
        {
            public HttpPostedFileBase File { get; set; }
        }
    }
}
