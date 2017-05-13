using Default.ViewModel.Areas.Shared;
using mPower.Framework.Mvc;
using mPower.Signals;
using mPower.WebApi.Tenants.SignalR.Hubs;
using Paralect.ServiceBus;

namespace mPower.WebApi.Tenants.SignalR.Handlers
{
    public class LedgerSignalHandler : IMessageHandler<AccountsUpdateSignal>, 
        IMessageHandler<AccountAddedSignal>
    {
        private readonly IObjectRepository _objectRepository;

        public LedgerSignalHandler(IObjectRepository objectRepository)
        {
            _objectRepository = objectRepository;
        }

        public void Handle(AccountsUpdateSignal sigmal)
        {
            var accounts = _objectRepository.Load<string, AccountsSidebarModel>(sigmal.LedgerId);
            LedgerHub.AccountUpdated(sigmal.UserId, accounts);
        }

        public void Handle(AccountAddedSignal signal)
        {
            LedgerHub.AccountAdded(signal.UserId);
        }
    }
}
