namespace mPower.Web.Admin.Models
{
    public class ItemDeleteTransferModel
    {
        public string ElapsedTime { get; set; }

        public long DeletedReadCount { get; set; }
        public long DeletedWriteCount { get; set; }
    }
}