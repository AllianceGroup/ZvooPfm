using mPower.TempDocuments.Server.Documents;

namespace mPower.TempDocuments.Server.Sql
{
    public interface IOffersRepository
    {
        void Insert(OfferDocument[] batch);
    }
}