using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mPower.Documents.DocumentServices.Accounting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;

namespace mPower.WebApi.Authorization
{
    public class LedgerAuthorizationHandler: IAuthorizationHandler
    {
        private readonly LedgerDocumentService _ledgerDocumetService;
        private readonly IMemoryCache _memoryCache;

        public LedgerAuthorizationHandler(LedgerDocumentService ledgerDocumetService, IMemoryCache memoryCache)
        {
            _ledgerDocumetService = ledgerDocumetService;
            _memoryCache = memoryCache;
        }

        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            var ledgerId =
                (context.Resource as Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext)?.HttpContext.Request.Headers[
                    "LedgerId"].FirstOrDefault();
            var userId = context.User.Identity.Name;

            var ledger = _memoryCache.Get(ledgerId) as ShortLedgerModel;

            if (ledger == null || ledger.Id != ledgerId)
            {
                var ledgerDocument = _ledgerDocumetService.GetById(ledgerId);
                if (ledgerDocument == null)
                {
                    context.Fail();
                    return Task.FromResult(0);
                }

                ledger = new ShortLedgerModel { Id = ledgerId, Users = ledgerDocument.Users.Select(x => x.Id).ToList() };
                _memoryCache.Set(ledgerId, ledger);
            }

            if (ledger.Users.FirstOrDefault(id => id == userId) == null)
                context.Fail();

            return Task.FromResult(0);
        }

        public class ShortLedgerModel
        {
            public string Id { get; set; }
            public List<string> Users { get; set; }
        }
    }
}
