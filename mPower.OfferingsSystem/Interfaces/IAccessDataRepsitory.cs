using System.Collections.Generic;
using mPower.OfferingsSystem.Data;

namespace mPower.OfferingsSystem
{
    public interface IAccessDataRepsitory
    {
        IEnumerable<Offer> GetOffers();
        IEnumerable<Brand> GetBrands();
        IEnumerable<Channel> GetChannels();
        IEnumerable<Merchant> GetMerchants();
        IEnumerable<Subscription> GetSubscriptions();
        IEnumerable<Category> GetCategories();
        IEnumerable<Member> GetMembers();
        IEnumerable<Mid> GetMids();
        IEnumerable<Product> GetProducts();
        IEnumerable<Redeem> GetRedeems();
        IEnumerable<Statement> GetStatements();
        IEnumerable<Status> GetStatuses();
        IEnumerable<Card> GetCards();
        IEnumerable<Transaction> GetTransactions();
        IEnumerable<Usage> GetUsages();
        IEnumerable<Settlement> GetSettlements();
    }
}