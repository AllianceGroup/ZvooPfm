using Paralect.Domain;
using mPower.Domain.Accounting.Enums;

namespace mPower.Domain.Yodlee.ContentServiceItem.Commands
{
    public class ContentServiceItem_Account_UpdateImportStatusCommand : Command
    {
        public string LedgerAccountId { get; set; }
        public string LedgerId { get; set; }
        public string ItemId { get; set; }
        public string ContentServiceItemAccountId { get; set; }
        public ImportStatusEnum ImportStatus { get; set; }
    }
}