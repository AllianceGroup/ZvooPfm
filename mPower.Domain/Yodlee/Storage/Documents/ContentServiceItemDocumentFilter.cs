using mPower.Framework.Services;

namespace mPower.Domain.Yodlee.Storage.Documents
{
    public class ContentServiceItemDocumentFilter : BaseFilter
    {
        public string AuthenticationReferenceId { get; set; }
        public string ContentServiceId { get; set; }
        public string UserId { get; set; }
        public string Id { get; set; }
    }
}
