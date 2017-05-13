using Paralect.Domain;

namespace mPower.Domain.Yodlee.ContentServiceItem.Commands
{
    public class ContentServiceItem_DownloadCommand : Command
    {

        public string LoginName { get; set; }
        public string Password { get; set; }
        public long ItemId { get; set; }
        public string UserId { get; set; }
        /// <summary>
        /// This is an optional property only used on initial download of data.
        /// </summary>
        public string AuthenticationReferenceId { get; set; }
    }
}
