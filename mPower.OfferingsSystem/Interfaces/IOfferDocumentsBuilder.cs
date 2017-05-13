using System;
using System.Collections.Generic;
using mPower.TempDocuments.Server.Documents;

namespace mPower.OfferingsSystem
{
    public interface IOfferDocumentsBuilder
    {
        IEnumerable<OfferDocument> GetAll(PackageInfo packageInfo);
    }
}