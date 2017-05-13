using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using mPower.Framework;
using mPower.Framework.Utils;
using mPower.OfferingsSystem;
using mPower.OfferingsSystem.Sheduler;
using mPower.TempDocuments.Server.DocumentServices;
using mPower.TempDocuments.Server.DocumentServices.Lucene;
using mPower.TempDocuments.Server.Documents;
using mPower.TempDocuments.Server.Sql;

namespace mPower.Tests.UnitTests.OfferingsSystem
{
    [TestFixture]
    [Ignore]
    public class DownloadAccessDataJobTest
    {
        private DownloadAccessDataJob _job;
        private Mock<IPackageDownloader> _download;
        private Mock<IOfferDocumentService> _documentService;
        private Mock<IOfferDocumentsBuilder> _documentBuilder;
        private Mock<IMerchantDocumentBuilder> _merchantDocumentBuilder;
        private Mock<IMerchantDocumentService> _merchantDocumentService;
        private Mock<IOfferGroupDocumentService> _offerGroupDocumentService;
        private Mock<IOfferLuceneService> _merchantLuceneService;
        private Mock<IOffersRepository> _offerRepository;
        private ActionQueue _actionQueue;

        [SetUp]
        public void SetUp()
        {
            _download = new Mock<IPackageDownloader>();
            _documentService = new Mock<IOfferDocumentService>();
            _documentBuilder = new Mock<IOfferDocumentsBuilder>();
            _merchantDocumentBuilder = new Mock<IMerchantDocumentBuilder>();
            _merchantDocumentService = new Mock<IMerchantDocumentService>();
            _offerGroupDocumentService = new Mock<IOfferGroupDocumentService>();
            _merchantLuceneService = new Mock<IOfferLuceneService>();
            _offerRepository = new Mock<IOffersRepository>();
            _actionQueue = new ActionQueue();
            _job = new DownloadAccessDataJob(
                _download.Object, 
                _documentService.Object, 
                _documentBuilder.Object, 
                _merchantDocumentBuilder.Object, 
                _merchantDocumentService.Object, 
                _offerGroupDocumentService.Object, 
                _merchantLuceneService.Object,
                _offerRepository.Object);
        }

        private void Execute()
        {
            _job.Execute();
            _actionQueue.Wait();
        }
        [Test]
        public void it_downloads_data()
        {
            Execute();
            _download.Verify(x=>x.Download());
        }

        [Test]
        public void it_saves_offers()
        {
            _documentBuilder.Setup(x => x.GetAll(It.IsAny<PackageInfo>())).Returns(() => new List<OfferDocument>
                                                                      {
                                                                          new OfferDocument
                                                                              {
                                                                                  Id = "1"
                                                                              }
                                                                      });
            var offers = new List<OfferDocument>();
            _documentService.Setup(x => x.InsertMany(It.IsAny<IEnumerable<OfferDocument>>())).Callback(
                (IEnumerable<OfferDocument> items) => offers.AddRange(items));

            Execute();
            Assert.AreEqual(1,offers.Count);
            Assert.AreEqual("1",offers[0].Id);
        }

        [Test]
        public void it_saves_test_data()
        {
            var settnigs = new MPowerSettings
                               {
                                   AccessDataFtpUrl = "ftp.adchosted.com", 
                                   AccessDataLocalPath = "d:\\access_local_folder",
                                   AccessDataUsername = "mpower01",
                                   AccessDataPassword = "Cp3#gcnDu3=a"
                               };
            _job = new DownloadAccessDataJob(
                new PackageDownloader(settnigs, new UploadUtil(settnigs)),
                _documentService.Object, 
                new OfferDocumentsBuilder(new AccessDataRepsitory(new FileLoader(settnigs)),
                    _merchantDocumentService.Object), 
                    _merchantDocumentBuilder.Object, 
                    _merchantDocumentService.Object,
                    _offerGroupDocumentService.Object,  
                    _merchantLuceneService.Object,
                    _offerRepository.Object  );
            Execute();
        }
    }
}