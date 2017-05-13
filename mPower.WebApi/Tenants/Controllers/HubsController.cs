using System.Threading.Tasks;
using mPower.WebApi.Tenants.SignalR.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace mPower.WebApi.Tenants.Controllers
{
    [Authorize("Pfm")]
    [Route("api/[controller]")]
    public class HubsController : Controller
    {
        [HttpPost("connect/{connectionId}")]
        public async Task Connect(string connectionId)
        {
            await LedgerHub.Connect(connectionId, GetGroupId());
        }

        [HttpPost("disconnect/{connectionId}")]
        public async Task Disconnect(string connectionId)
        {
            await LedgerHub.Disconnect(connectionId, GetGroupId());
        }

        private string GetGroupId()
        {
            return HttpContext.User.Identity.Name;
        }
    }
}