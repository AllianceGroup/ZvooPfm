using mPower.Framework.Geo;

namespace mPower.TempDocuments.Server.Sql
{
    public class OfferEntity
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Merchant { get; set; }
        public double Distance { get; set; }
        public Location Location { get; set; }
        public string Category { get; set; }
        public string Logo { get; set; }
        public string FormatedAward { get; set; }
    }
}