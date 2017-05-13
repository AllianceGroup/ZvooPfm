using mPower.Framework.Services;

namespace mPower.TempDocuments.Server.DocumentServices.Filters
{
    public class ImportFilter : BaseFilter
    {
        public string Id { get; set; }

        public string LedgerId { get; set; }
    }
}
