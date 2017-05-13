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
    public class FaqController : BaseController
    {
        private readonly AffiliateDocumentService _affiliateService;
        private readonly IIdGenerator _idGenerator;

        public FaqController(AffiliateDocumentService affiliateService, IIdGenerator idGenerator,
            ICommandService command, IApplicationTenant tenant) :base(command, tenant)
        {
            _affiliateService = affiliateService;
            _idGenerator = idGenerator;
        }

        [HttpGet]
        public FaqListModel GetFaq()
        {
            return GetFaqListModel();
        }

        [AllianceAuthorize(UserPermissionEnum.AffiliateAdminEdit)]
        [HttpGet("edit")]
        public FaqModel EditFaq(string id)
        {
            return GetFaqModel(id);
        }

        [AllianceAuthorize(UserPermissionEnum.AffiliateAdminEdit)]
        [HttpPost("update")]
        public IActionResult EditFaq([FromBody] FaqModel model)
        {
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            var affiliateId = Tenant.ApplicationId;

            Command cmd;
            if (model.Id == null)
            {
                cmd = new Affiliate_Faq_AddCommand
                {
                    AffiliateId = affiliateId,
                    Id = _idGenerator.Generate(),
                    Name = model.Name,
                    Html = model.Html,
                    IsActive = model.IsActive
                };
            }
            else
            {
                cmd = new Affiliate_Faq_UpdateCommand
                {
                    AffiliateId = affiliateId,
                    Id = model.Id,
                    Name = model.Name,
                    Html = model.Html,
                    IsActive = model.IsActive
                };
            }

            Send(cmd);

            return new OkResult();
        }

        [AllianceAuthorize(UserPermissionEnum.AffiliateAdminEdit)]
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteFaq(string id)
        {
            var affiliateId = Tenant.ApplicationId;
            var affiliate = _affiliateService.GetById(affiliateId);

            var faqDocument = affiliate.FaqDocuments.Find(ec => ec.Id == id);

            if (faqDocument == null)
                ModelState.AddModelError("Doesn't exist", "Specified FAQ doesn't exist.");
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            var command = new Affiliate_Faq_DeleteCommand { Id = id, AffiliateId = Tenant.ApplicationId };

            Send(command);

            return new OkResult();
        }

        private FaqModel GetFaqModel(string id = null)
        {
            var affiliateId = Tenant.ApplicationId;
            var affiliate = _affiliateService.GetById(affiliateId);
            var templatesList = affiliate.FaqDocuments.Select(t => new SelectListItem { Value = t.Id, Text = t.Name }).ToList();
            templatesList.Insert(0, new SelectListItem { Value = "-1", Text = "Create new" });

            var model = new FaqModel();
            if (!string.IsNullOrEmpty(id))
            {
                var selectedFaq = affiliate.FaqDocuments.Find(t => t.Id == id);
                if (selectedFaq != null)
                {
                    model.Id = selectedFaq.Id;
                    model.Name = selectedFaq.Name;
                    model.Html = selectedFaq.Html;
                    model.IsActive = selectedFaq.IsActive;
                }
            }

            return model;
        }

        private FaqListModel GetFaqListModel()
        {
            var affiliateId = Tenant.ApplicationId;
            var affiliate = _affiliateService.GetById(affiliateId);
            var faqs = affiliate.FaqDocuments.Select(Map).OrderBy(x => x.Name).ToList();

            return new FaqListModel { FaqList = faqs };
        }

        private static FaqListItemModel Map(FaqDocument doc)
        {
            return new FaqListItemModel
            {
                Id = doc.Id,
                Created = doc.CreationDate,
                Name = doc.Name,
                IsActive = doc.IsActive,  
                Html = doc.Html
            };
        }
    }
}
