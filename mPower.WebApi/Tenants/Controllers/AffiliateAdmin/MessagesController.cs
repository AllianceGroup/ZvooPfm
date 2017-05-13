using System;
using System.Collections.Generic;
using System.Linq;
using Default.Areas.Administration.Models;
using mPower.Documents.Documents.Affiliate;
using mPower.Documents.DocumentServices;
using mPower.Domain.Application.Affiliate.Commands;
using mPower.Domain.Application.Enums;
using mPower.Framework.Environment;
using mPower.TempDocuments.Server.Notifications;
using mPower.TempDocuments.Server.Notifications.Nuggets;
using Microsoft.AspNetCore.Mvc;
using Paralect.Domain;
using mPower.WebApi.Authorization;
using mPower.Domain.Membership.Enums;
using mPower.Framework;
using mPower.Framework.Environment.MultiTenancy;

namespace mPower.WebApi.Tenants.Controllers.AffiliateAdmin
{
    [AllianceAuthorize(UserPermissionEnum.AffiliateAdminView)]
    [Route("api/[controller]")]
    public class MessagesController : BaseController
    {
        private readonly AffiliateDocumentService _affiliateService;
        private readonly NuggetHtmlBuilder _nuggetBuilder;
        private readonly IIdGenerator _idGenerator;

        public MessagesController(AffiliateDocumentService affiliateService, NuggetHtmlBuilder nuggetBuilder, 
            IIdGenerator idGenerator, ICommandService command, IApplicationTenant tenant) :base(command, tenant)
        {
            _affiliateService = affiliateService;
            _nuggetBuilder = nuggetBuilder;
            _idGenerator = idGenerator;
        }

        [HttpGet]
        public MessagesListModel GetMessages()
        {
            return GetMessagesListModel();
        }

        [AllianceAuthorize(UserPermissionEnum.AffiliateAdminEdit)]
        [HttpGet("edit")]
        public MessageModel EditMessage(string id)
        {
            return GetMessageModel(id);
        }

        [AllianceAuthorize(UserPermissionEnum.AffiliateAdminEdit)]
        [HttpPost("update")]
        public IActionResult UpdateMessage([FromBody] MessageModel model)
        {
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            Command cmd;
            if (model.IsNew)
            {
                cmd = new Affiliate_Email_Content_AddCommand
                {
                    AffiliateId = Tenant.ApplicationId,
                    TemplateId = model.TemplateId,
                    Id = _idGenerator.Generate(),
                    Name = model.Name,
                    Subject = model.Subject,
                    Html = model.Html,
                    Status = model.Status,
                    CreationDate = DateTime.Now,
                };
            }
            else
            {
                cmd = new Affiliate_Email_Content_UpdateCommand
                {
                    AffiliateId = Tenant.ApplicationId,
                    TemplateId = model.TemplateId,
                    Id = model.Id,
                    Name = model.Name,
                    Subject = model.Subject,
                    Html = model.Html,
                    Status = model.Status,
                };
            }

            Send(cmd);

            return new OkResult();
        }

        [AllianceAuthorize(UserPermissionEnum.AffiliateAdminDelete)]
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteMessage(string id)
        {
            var affiliateId = Tenant.ApplicationId;
            var affiliate = _affiliateService.GetById(affiliateId);

            var message = affiliate.EmailContents.Find(ec => ec.Id == id);

            if (message == null)
                ModelState.AddModelError("Doesn't exist", "Specified message doesn't exist.");
            if (message.IsDefaultForEmailType.HasValue)
                ModelState.AddModelError("Deleted", "Default message can't be deleted.");
            if (affiliate.NotificationTypeEmails.Any(nte => nte.EmailContentId == id))
                ModelState.AddModelError("Used", "This message can't be deleted, because it's currently used.");
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            var command = new Affiliate_Email_Content_DeleteCommand
            {
                Id = id,
                AffiliateId = Tenant.ApplicationId,
            };

            Send(command);

            return new OkResult();
        }

        [AllianceAuthorize(UserPermissionEnum.AffiliateAdminEdit)]
        [HttpPost("previewMessage")]
        public IActionResult PreviewMessage([FromBody]MessageModel contentModel)
        {
            var affiliate = _affiliateService.GetById(Tenant.ApplicationId);
            ModelState.Clear();
            if(!string.IsNullOrEmpty(contentModel.Html))
            {
                var emailTemplate = affiliate.EmailTemplates.LastOrDefault(t => t.Id == contentModel.TemplateId);
                if(emailTemplate != null)
                {
                    var html = emailTemplate.Html.Replace("{{Body}}", contentModel.Html);
                    return new OkObjectResult(html);
                }
                ModelState.AddModelError("template", "Can't find specified template.");
                return new BadRequestObjectResult(ModelState);
            }

            ModelState.AddModelError("Html", "Please add some content to email before preview.");
            return new BadRequestObjectResult(ModelState);
        }

        private MessagesListModel GetMessagesListModel()
        {
            var affiliateId = Tenant.ApplicationId;
            var affiliate = _affiliateService.GetById(affiliateId);
            var msgs = affiliate.EmailContents.Select(Map).OrderBy(x => x.Name).ToList();

            return new MessagesListModel {Messages = msgs};
        }

        private MessageModel GetMessageModel(string id = null)
        {
            var affiliateId = Tenant.ApplicationId;
            var affiliate = _affiliateService.GetById(affiliateId);

            var model = new MessageModel {Status = TemplateStatusEnum.Active};
            if (!string.IsNullOrEmpty(id))
            {
                var selectedMessage = affiliate.EmailContents.Find(c => c.Id == id);
                if (selectedMessage != null)
                {
                    model.Id = selectedMessage.Id;
                    model.TemplateId = selectedMessage.TemplateId;
                    model.Name = selectedMessage.Name;
                    model.Subject = selectedMessage.Subject;
                    model.Html = selectedMessage.Html;
                    model.Status = selectedMessage.Status;
                    model.IsUsedInTrigger = affiliate.NotificationTypeEmails.Any(x => x.EmailContentId == selectedMessage.Id);
                }
            }

            return BindDropDownValues(model);
        }

        private MessageModel BindDropDownValues(MessageModel model)
        {
            var affiliateId = Tenant.ApplicationId;
            var affiliate = _affiliateService.GetById(affiliateId);
            model.Templates = new Dictionary<string, string>();
            affiliate.EmailTemplates.ForEach(t => model.Templates.Add(t.Id, t.Name));
            model.Nuggets = _nuggetBuilder.AllNuggets.Select(Map).OrderBy(x => x.DisplayName).ToList();

            return model;
        }

        private static MessagesListItemModel Map(EmailContentDocument doc)
        {
            return new MessagesListItemModel
            {
                Id = doc.Id,
                Created = doc.CreationDate,
                Name = doc.Name,
                Status = doc.Status,
                IsDefault = doc.IsDefaultForEmailType.HasValue,
            };
        }

        private static NuggetListItemModel Map(INugget nugget)
        {
            return new NuggetListItemModel
            {
                Tag = nugget.Tag,
                DisplayName = nugget.DisplayName,
            };
        }
    }
}
