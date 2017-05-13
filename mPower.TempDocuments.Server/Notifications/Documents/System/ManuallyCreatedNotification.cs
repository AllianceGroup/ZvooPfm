using mPower.Domain.Application.Enums;

namespace mPower.TempDocuments.Server.Notifications.Documents.System
{
    public class ManuallyCreatedNotification : BaseNotification
    {
        public string EmailContentId { get; set; }

        public ManuallyCreatedNotification()
        {
            Type = EmailTypeEnum.ManuallyCreated;
        }
    }
}