using System.Linq;
using Default.Areas.Administration.Models;
using mPower.Documents.Documents.Affiliate;
using mPower.Documents.DocumentServices;
using mPower.Domain.Application.Affiliate.Commands;
using mPower.Domain.Application.Enums;
using mPower.Framework.Environment;
using Microsoft.AspNetCore.Mvc;
using Paralect.Domain;
using mPower.WebApi.Authorization;
using mPower.Domain.Membership.Enums;
using mPower.Framework;
using mPower.Framework.Environment.MultiTenancy;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace mPower.WebApi.Tenants.Controllers.AffiliateAdmin
{
    [AllianceAuthorize(UserPermissionEnum.AffiliateAdminView)]
    [Route("api/[controller]")]
    public class TemplatesController : BaseController
    {
        private readonly AffiliateDocumentService _affiliateService;
        private readonly IIdGenerator _idGenerator;

        public TemplatesController(AffiliateDocumentService affiliateService, IIdGenerator idGenerator,
            ICommandService command, IApplicationTenant tenant) :base(command, tenant)
        {
            _affiliateService = affiliateService;
            _idGenerator = idGenerator;
        }

        [HttpGet]
        public TemplatesListModel GetTemplates()
        {
            return GetTemplatesListModel();
        }

        [AllianceAuthorize(UserPermissionEnum.AffiliateAdminEdit)]
        [HttpGet("edit")]
        public TemplateModel EditTemplate(string id)
        {
            return GetTemplateModel(id);
        }

        [AllianceAuthorize(UserPermissionEnum.AffiliateAdminEdit)]
        [HttpPost("update")]
        public IActionResult EditTemplate([FromBody] TemplateModel model)
        {
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            var affiliateId = Tenant.ApplicationId;

            Command cmd;
            if (model.IsNew)
            {
                cmd = new Affiliate_Email_Template_AddCommand
                {
                    AffiliateId = affiliateId,
                    Id = _idGenerator.Generate(),
                    Name = model.Name,
                    Html = model.Html,
                };
            }
            else
            {
                cmd = new Affiliate_Email_Template_UpdateCommand
                {
                    AffiliateId = affiliateId,
                    Id = model.Id,
                    Name = model.Name,
                    Html = model.Html,
                    Status = TemplateStatusEnum.Active
                };
            }

            Send(cmd);

            return new OkResult();
        }

        [AllianceAuthorize(UserPermissionEnum.AffiliateAdminEdit)]
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteTemplate(string id)
        {
            var affiliateId = Tenant.ApplicationId;
            var affiliate = _affiliateService.GetById(affiliateId);

            var designTemplate = affiliate.EmailTemplates.Find(ec => ec.Id == id);

            if(designTemplate == null)
                ModelState.AddModelError("Doesn't exist", "Specified template doesn't exist.");
            if(designTemplate.IsDefault)
                ModelState.AddModelError("Deleted", "Default template can't be deleted.");
            if(affiliate.EmailContents.Any(ec => ec.TemplateId == id))
                ModelState.AddModelError("Used", "This template can't be deleted, because it's currently used.");
            if(!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            var command = new Affiliate_Email_Template_DeleteCommand{ Id = id, AffiliateId = Tenant.ApplicationId };

            Send(command);

            return new OkResult();
        }

        private TemplateModel GetTemplateModel(string id = null)
        {
            var affiliateId = Tenant.ApplicationId;
            var affiliate = _affiliateService.GetById(affiliateId);
            var templatesList = affiliate.EmailTemplates.Select(t => new SelectListItem {Value = t.Id, Text = t.Name}).ToList();
            templatesList.Insert(0, new SelectListItem {Value = "-1", Text = "Create new"});

            var model = new TemplateModel();
            if (!string.IsNullOrEmpty(id))
            {
                var selectedTemplate = affiliate.EmailTemplates.Find(t => t.Id == id);
                if(selectedTemplate != null)
                {
                    model.Id = selectedTemplate.Id;
                    model.Name = selectedTemplate.Name;
                    model.Html = selectedTemplate.Html;
                }
            }

            return model;
        }

        private TemplatesListModel GetTemplatesListModel()
        {
            var affiliateId = Tenant.ApplicationId;
            var affiliate = _affiliateService.GetById(affiliateId);
            var tmpls = affiliate.EmailTemplates.Select(Map).OrderBy(x => x.Name).ToList();

            return new TemplatesListModel {Templates = tmpls};
        }

        private static TemplatesListItemModel Map(EmailTemplateDocument doc)
        {
            return new TemplatesListItemModel
            {
                Id = doc.Id,
                Created = doc.CreationDate,
                Name = doc.Name,
                Status = doc.Status,
                IsDefault = doc.IsDefault
            };
        }
    }
}
