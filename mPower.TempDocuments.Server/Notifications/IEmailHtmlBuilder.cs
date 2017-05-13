using mPower.TempDocuments.Server.Notifications.Documents;

namespace mPower.TempDocuments.Server.Notifications
{
    public interface IEmailHtmlBuilder
    {
        void GenerateEmailData(BaseNotification notification, out string subject, out string body);
    }
}