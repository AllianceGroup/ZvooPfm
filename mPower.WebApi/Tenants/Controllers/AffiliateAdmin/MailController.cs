using System;
using System.Collections.Generic;
using System.Linq;
using mPower.Documents.DocumentServices;
using mPower.Documents.ExternalServices.FullTextSearch;
using mPower.Domain.Application.Enums;
using mPower.Framework;
using mPower.TempDocuments.Server.Notifications.Messages;
using mPower.WebApi.Tenants.Model.AffiliateAdmin;
using mPower.WebApi.Tenants.ViewModels.AffiliateAdmin;
using Microsoft.AspNetCore.Mvc;
using mPower.WebApi.Authorization;
using mPower.Domain.Membership.Enums;
using mPower.Framework.Environment.MultiTenancy;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace mPower.WebApi.Tenants.Controllers.AffiliateAdmin
{
    [AllianceAuthorize(UserPermissionEnum.AffiliateAdminView)]
    [Route("api/[controller]")]
    public class MailController : BaseController
    {
        private readonly SendMailGroupLuceneService _sendMailGroupLuceneService;
        private readonly AffiliateDocumentService _affiliateService;
        private readonly IEventService _eventService;

        public MailController(SendMailGroupLuceneService sendMailGroupLuceneService, 
            AffiliateDocumentService affiliateService, IEventService eventService,
            ICommandService command, IApplicationTenant tenant) :base(command, tenant)
        {
            _sendMailGroupLuceneService = sendMailGroupLuceneService;
            _affiliateService = affiliateService;
            _eventService = eventService;
        }

        [AllianceAuthorize(UserPermissionEnum.AffiliateAdminView)]
        [HttpGet]
        public SendMailViewModel SendMail()
        {
            var model = new SendMailViewModel();
            PrepareModel(model);
            return model;
        }

        [AllianceAuthorize(UserPermissionEnum.AffiliateAdminEdit)]
        [HttpPost]
        public IActionResult SendMail([FromBody]SendMailViewModel model)
        {
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            var userIds = GetRecipientsIds(model.Ids);

            var message = new EmailManuallyCreatedMessage
            {
                UsersIds = userIds,
                AffiliateId = Tenant.ApplicationId,
                EmailContentId = model.ContentId
            };
            _eventService.Send(message);

            return new OkResult();
        }

        private void PrepareModel(SendMailViewModel model)
        {
            var affiliate = _affiliateService.GetById(Tenant.ApplicationId);
            model.Contents = affiliate.EmailContents.Where(x => x.IsDefaultForEmailType != EmailTypeEnum.ForgotPassword)
                .Select(c => new SelectListItem {Text = c.Name, Value = c.Id}).ToList();

            affiliate.Segments.ForEach(
                segment => { model.Segments.Add(new MailSegmentModel {Id = segment.Id, Name = segment.Name}); });
        }

        private List<string> GetRecipientsIds(string ids)
        {
            var idsArray = ids.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            var userIds = new List<string>();

            foreach (var doc in idsArray.Select(id => _sendMailGroupLuceneService.GetById(id)))
                userIds.AddRange(doc.UserIds);

            return userIds;
        }
    }
}
