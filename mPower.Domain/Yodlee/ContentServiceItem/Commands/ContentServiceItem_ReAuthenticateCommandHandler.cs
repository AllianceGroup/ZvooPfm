using Paralect.ServiceBus;
using com.yodlee.soap.core.accountmanagement;
using com.yodlee.soap.core.refresh;
using mPower.Domain.Yodlee.Form;
using mPower.Domain.Yodlee.Services;
using mPower.Framework;

namespace mPower.Domain.Yodlee.ContentServiceItem.Commands
{
    public class ContentServiceItem_ReAuthenticateCommandHandler : BaseYodleeService, IMessageHandler<ContentServiceItem_ReAuthenticateCommand>
    {
        
        private readonly CommandService _commandService;

        public ContentServiceItem_ReAuthenticateCommandHandler(CommandService commandService)
        {
            _commandService = commandService;
        }

        public void Handle(ContentServiceItem_ReAuthenticateCommand message)
        {
            ConnectToYodlee();

            var user = LoginUser(message.Username, message.Password);

            var itemManagementService = new ItemManagementService();

            var authenticationForm = itemManagementService.getLoginFormCredentialsForItem(user.userContext,
                                                                                          message.ContentServiceItemId);

            //Fills out the yodlee form
            FormHelpers.BuildParameterList(ref authenticationForm.componentList, message.PostData);

            //ItemManagementService does everything with forms and authentication
            itemManagementService.updateCredentialsForItem(user.userContext, message.ContentServiceItemId,
                                                           authenticationForm.componentList);

            // Starts Yodlee Refreshing Data
            var refreshService = new RefreshService();
            var refreshStatus = refreshService.startRefresh(user.userContext, message.ContentServiceItemId, 1);
            //long[] failedStatus = { 0, 3, 7, 5 };

            _commandService.Send(new ContentServiceItem_DownloadCommand()
                                     {
                                         ItemId = message.ContentServiceItemId,
                                         UserId = message.UserId,
                                         LoginName = message.Username,
                                         Password = message.Password
                                     });
        }
    }
}