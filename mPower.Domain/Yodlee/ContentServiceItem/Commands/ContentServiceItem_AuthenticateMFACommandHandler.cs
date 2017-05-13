using Paralect.Domain;
using Paralect.ServiceBus;
using com.yodlee.soap.core.refresh;
using mPower.Domain.Yodlee.Form;
using mPower.Domain.Yodlee.Services;
using mPower.Framework;

namespace mPower.Domain.Yodlee.ContentServiceItem.Commands
{
    public class ContentServiceItem_AuthenticateMFACommandHandler : BaseYodleeService, IMessageHandler<ContentServiceItem_AuthenticateMFACommand>
    {
        private readonly IRepository _repository;
        private readonly CommandService _commandService;

        public ContentServiceItem_AuthenticateMFACommandHandler(IRepository repository, CommandService commandService)
        {
            _repository = repository;
            _commandService = commandService;
        }

        public void Handle(ContentServiceItem_AuthenticateMFACommand message)
        {
            ConnectToYodlee();

            var user = LoginUser(message.Username, message.Password);

            var mfaUserResponse = FormHelpers.BuildQuestionAnswersInput(message.PostData);

            var refreshService = new RefreshService();
            refreshService.putMFARequest(user.userContext, mfaUserResponse, message.ContentServiceItemId);

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