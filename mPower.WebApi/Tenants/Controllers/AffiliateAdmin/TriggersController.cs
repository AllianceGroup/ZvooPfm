using System;
using System.Linq;
using Default.Areas.Administration.Models;
using mPower.Documents.Documents.Affiliate;
using mPower.Documents.DocumentServices;
using mPower.Domain.Application.Affiliate.Commands;
using mPower.Domain.Application.Enums;
using mPower.Framework.Utils.Extensions;
using mPower.Framework.Utils.Notification;
using mPower.WebApi.Tenants.Model.AffiliateAdmin;
using Microsoft.AspNetCore.Mvc;
using mPower.WebApi.Authorization;
using mPower.Domain.Membership.Enums;
using mPower.Framework;
using mPower.Framework.Environment.MultiTenancy;
using mPower.WebApi.Tenants.ViewModels.AffiliateAdmin;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace mPower.WebApi.Tenants.Controllers.AffiliateAdmin
{
    [AllianceAuthorize(UserPermissionEnum.AffiliateAdminView)]
    [Route("api/[controller]")]
    public class TriggersController : BaseController
    {
        private readonly AffiliateDocumentService _affiliateService;

        public TriggersController(AffiliateDocumentService affiliateService,
            ICommandService command, IApplicationTenant tenant) :base(command, tenant)
        {
            _affiliateService = affiliateService;
        }

        [HttpGet]
        public TriggersListModel GetTriggersList()
        {
            return GetTriggersListModel();
        }

        [AllianceAuthorize(UserPermissionEnum.AffiliateAdminEdit)]
        [HttpGet("editTrigger/{id}")]
        public TriggerModel EditTrigger(EmailTypeEnum id)
        {
            return GetTriggerModel(id);         
        }

        [AllianceAuthorize(UserPermissionEnum.AffiliateAdminEdit)]
        [HttpPost("editTrigger")]
        public IActionResult EditTrigger([FromBody] TriggerUpdateModel model)
        {
            if(!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            var cmd = new Affiliate_NotificationTypeEmail_UpdateCommand
            {
                AffiliateId = Tenant.ApplicationId,
                EmailType = model.Id,
                EmailContentId = model.MessageId,
                Status =
                    model.Id.GetNotificationGroup() == NotificationGroupEnum.Affiliate
                        ? model.Status
                        : TriggerStatusEnum.Active,
            };

            Send(cmd);

            return new OkResult();
        }

        private TriggersListModel GetTriggersListModel()
        {
            var affiliateId = Tenant.ApplicationId;
            var affiliate = _affiliateService.GetById(affiliateId);

            var model = new TriggersListModel
            {
                Triggers = affiliate.NotificationTypeEmails.Select(Map).ToList()
            };

            return model;
        }

        private static TriggersListItemModel Map(NotificationTypeEmailDocument doc)
        {
            return new TriggersListItemModel
            {
                Id = doc.EmailType,
                Name = doc.Name,
                Status = doc.Status,
            };
        }

        private TriggerModel GetTriggerModel(EmailTypeEnum id)
        {
            var affiliateId = Tenant.ApplicationId;
            var affiliate = _affiliateService.GetById(affiliateId);

            var model = new TriggerModel {Status = TriggerStatusEnum.Active};
            var selectedTrigger = affiliate.NotificationTypeEmails.Find(c => c.EmailType == id);
            if (selectedTrigger != null)
            {
                model.Id = selectedTrigger.EmailType;
                model.Name = selectedTrigger.Name;
                model.MessageId = selectedTrigger.EmailContentId;
                model.Status = selectedTrigger.Status;
            }

            return BindDropDownValues(model);
        }

        private TriggerModel BindDropDownValues(TriggerModel model)
        {
            var affiliateId = Tenant.ApplicationId;
            var affiliate = _affiliateService.GetById(affiliateId);

            var activeMessages = affiliate.EmailContents.Where(x => x.Status == TemplateStatusEnum.Active);
            model.MessagesList = new SelectList(activeMessages, "Id", "Name", model.MessageId);

            var statuses = Enum.GetValues(typeof(TriggerStatusEnum)).Cast<TriggerStatusEnum>().ToDictionary(x => x, x => x.GetDescription());
            model.StatusesList = new SelectList(statuses, "Key", "Value", model.Status);

            return model;
        }
    }
}
