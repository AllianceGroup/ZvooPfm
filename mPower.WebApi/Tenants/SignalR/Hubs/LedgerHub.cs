using System.Threading.Tasks;
using Default.ViewModel.Areas.Shared;
using Microsoft.AspNet.SignalR;

namespace mPower.WebApi.Tenants.SignalR.Hubs
{
    public class LedgerHub : Hub
    {
        private static IHubContext CurrentContext =>
            GlobalHost.ConnectionManager.GetHubContext<LedgerHub>();

        public async static Task Connect(string connectionId, string groupId)
        {
            await CurrentContext.Groups.Add(connectionId, groupId);
        }

        public async static Task Disconnect(string connectionId, string groupId)
        {
            await CurrentContext.Groups.Remove(connectionId, groupId);
        }

        public static void AccountUpdated(string userId, AccountsSidebarModel accounts)
        {
            CurrentContext.Clients.Group(userId).accountUpdated(accounts);
        }

        public static void AccountAdded(string userId)
        {
            CurrentContext.Clients.Group(userId).accountAdded();
        }
    }
}
