using System;
using MongoDB.Driver.Builders;
using Paralect.ServiceBus;
using com.yodlee.soap.core.refresh;
using mPower.Domain.Yodlee.Enums;
using mPower.Domain.Yodlee.Services;
using mPower.Domain.Yodlee.Storage.Documents;

namespace mPower.Domain.Yodlee.ContentServiceItem.Commands
{
    public class ContentServiceItem_StartRefreshCommandHandler : BaseYodleeService, IMessageHandler<ContentServiceItem_StartRefreshCommand>
    {
       
        private readonly ContentServiceItemDocumentService _documentService;

        public ContentServiceItem_StartRefreshCommandHandler(ContentServiceItemDocumentService documentService)
        {
            _documentService = documentService;
        }


        public void Handle(ContentServiceItem_StartRefreshCommand message)
        {
            ConnectToYodlee();
            var user = LoginUser(message.Username, message.Password);

            // Starts Yodlee Refreshing Data
            var refreshService = new RefreshService();
            try
            {
                var refreshStatus = refreshService.startRefresh(user.userContext, message.ItemId, 1);
                var query = Query.EQ("_id", message.ItemId.ToString());

                var update = Update<ContentServiceItemDocument>.Set(x => x.RefreshStatus, (RefreshStatus)refreshStatus.status);

                _documentService.Update(query, update);
            }catch(Exception e)
            {
                var query = Query.EQ("_id", message.ItemId.ToString());
                var update = Update<ContentServiceItemDocument>.Set(x => x.RefreshStatus, RefreshStatus.InvalidItem);
                _documentService.Update(query, update);
            }
            
        }
    }
}