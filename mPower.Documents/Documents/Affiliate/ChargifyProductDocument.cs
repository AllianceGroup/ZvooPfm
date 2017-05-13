using MongoDB.Bson.Serialization.Attributes;

namespace mPower.Documents.Documents.Affiliate
{
    public class ChargifyProductDocument
    {
        /// <summary>
        /// Get the price (in cents)
        /// </summary>
        public int PriceInCents { get; set; }

        /// <summary>
        /// Get the name of this product
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The ID of the product
        /// </summary>
        [BsonId]
        public int Id { get; set; }

        /// <summary>
        /// Get the handle to this product
        /// </summary>
        public string Handle { get; set; }

        /// <summary>
        /// Get the description of the product
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Get the accounting code for this product
        /// </summary>
        public string AccountingCode { get; set; }

        public bool IsArchived { get; set; }

        /// <summary>
        /// Product public sign-up page
        /// </summary>
        public string SignUpPage { get; set; }
    }
}
