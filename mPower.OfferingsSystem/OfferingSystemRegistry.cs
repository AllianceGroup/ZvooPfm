using mPower.TempDocuments.Server.DocumentServices;
using mPower.TempDocuments.Server.DocumentServices.Lucene;
using mPower.TempDocuments.Server.Sql;
using StructureMap;

namespace mPower.OfferingsSystem
{
    public class OfferingSystemRegistry: Registry
    {
        public OfferingSystemRegistry()
        {
            For<IFileLoader>().Use<FileLoader>();
            For<IAccessDataRepsitory>().Use<AccessDataRepsitory>();
            For<IPackageDownloader>().Use<PackageDownloader>();
            For<IOfferDocumentsBuilder>().Use<OfferDocumentsBuilder>();
            For<IOfferDocumentService>().Use<OfferDocumentService>();
            For<IMerchantDocumentBuilder>().Use<MerchantDocumentBuilder>();
            For<IMerchantDocumentService>().Use<MerchantDocumentService>();
            For<IActionQueue>().Singleton().Use<ActionQueue>();
            For<IOfferGroupDocumentService>().Singleton().Use<OfferGroupDocumentService>();
            For<IOfferLuceneService>().Singleton().Use<OfferLuceneService>();
            For<IOffersRepository>().Singleton().Use<OffersRepository>();
        }
    }
}