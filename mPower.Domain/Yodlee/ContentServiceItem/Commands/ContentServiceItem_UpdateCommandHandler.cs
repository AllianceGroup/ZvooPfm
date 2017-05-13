using Paralect.ServiceBus;
using mPower.Domain.Yodlee.Services;
using mPower.Domain.Yodlee.Storage.Documents;

namespace mPower.Domain.Yodlee.ContentServiceItem.Commands
{
    public class ContentServiceItem_UpdateCommandHandler : BaseYodleeService, IMessageHandler<ContentServiceItem_UpdateCommand>
    {
        private readonly ContentServiceItemDocumentService _docService;

        public ContentServiceItem_UpdateCommandHandler( ContentServiceItemDocumentService docService )
        {
            _docService = docService;
        }

        public void Handle(ContentServiceItem_UpdateCommand message)
        {
            var document = _docService.GetById(message.ContentServiceItemId);

            document.AuthenticationReferenceId = message.AuthenticationReferenceId;

            _docService.Save(document);
            
        }
    }
}