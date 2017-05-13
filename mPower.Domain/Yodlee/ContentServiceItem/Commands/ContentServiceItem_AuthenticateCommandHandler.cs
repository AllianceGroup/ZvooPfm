using Paralect.Domain;
using Paralect.ServiceBus;
using com.yodlee.soap.core.accountmanagement;
using com.yodlee.soap.core.refresh;
using mPower.Domain.Yodlee.Form;
using mPower.Domain.Yodlee.Services;
using mPower.Framework;

namespace mPower.Domain.Yodlee.ContentServiceItem.Commands
{
    public class ContentServiceItem_AuthenticateCommandHandler : BaseYodleeService, IMessageHandler<ContentServiceItem_AuthenticateCommand>
    {
        private readonly IRepository _repository;
        private readonly CommandService _commandService;

        public ContentServiceItem_AuthenticateCommandHandler(IRepository repository, CommandService commandService)
        {
            _repository = repository;
            _commandService = commandService;
        }

        public void Handle(ContentServiceItem_AuthenticateCommand message)
        {
            ConnectToYodlee();

            var user = LoginUser(message.Username, message.Password);

            var itemManagementService = new ItemManagementService();

            var authenticationForm = itemManagementService.getLoginFormForContentService(user.userContext, message.ContentServiceId);

            //Fills out the yodlee form
            FormHelpers.BuildParameterList(ref authenticationForm.componentList, message.PostData);

            //Submits form. ItemManagementService does everything with forms and authentication
            var contentServiceItemId = new ItemManagementService().addItemForContentService(user.userContext, message.ContentServiceId,
                                                                                            authenticationForm.componentList, true,
                                                                                            true);

            // Starts Yodlee Refreshing Data
            var refreshService = new RefreshService();
            var refreshStatus = refreshService.startRefresh(user.userContext, contentServiceItemId, 1);
            //long[] failedStatus = { 0, 3, 7, 5 };

            _commandService.Send(new ContentServiceItem_DownloadCommand()
                                     {
                                         ItemId = contentServiceItemId,
                                         UserId = message.UserId,
                                         LoginName = message.Username,
                                         Password = message.Password,
                                         AuthenticationReferenceId = message.AuthenticationReferenceId
                                     });
            
        }
    }
}