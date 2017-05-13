using System.Collections.Generic;
using System.Linq;
using Default.Areas.Administration.Models;
using Default.Helpers;
using Default.ViewModel;
using mPower.Documents.Documents.Affiliate;
using mPower.Documents.DocumentServices;
using mPower.Documents.Segments;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Application.Affiliate.Commands;
using mPower.Domain.Application.Affiliate.Data;
using mPower.Framework.Mvc;
using mPower.Framework.Utils.Extensions;
using mPower.WebApi.Tenants.ViewModels.AffiliateAdmin;
using Microsoft.AspNetCore.Mvc;
using mPower.WebApi.Authorization;
using mPower.Domain.Membership.Enums;
using mPower.Framework;
using mPower.Framework.Environment.MultiTenancy;

namespace mPower.WebApi.Tenants.Controllers.AffiliateAdmin
{
    [AllianceAuthorize(UserPermissionEnum.AffiliateAdminView)]
    [Route("api/[controller]")]
    public class CampaignBuilderController : BaseController
    {
        private readonly AffiliateDocumentService _affiliateService;
        private readonly AccountsService _accountsService;
        private readonly IObjectRepository _objectRepository;
        private readonly SegmentViewHelper _viewHelper;
        public readonly SegmentEstimationHelper EstimationHelper;

        protected virtual bool IsCampaignMode => true;

        public CampaignBuilderController(AffiliateDocumentService affiliateService, IObjectRepository objectRepository, 
            SegmentViewHelper viewHelper, AccountsService accountsService, SegmentEstimationHelper estimationHelper,
            ICommandService command, IApplicationTenant tenant) :base(command, tenant)
        {
            _affiliateService = affiliateService;
            _objectRepository = objectRepository;
            _viewHelper = viewHelper;
            _accountsService = accountsService;
            EstimationHelper = estimationHelper;
        }

        #region Segment

        [HttpGet("getSegments")]
        public SegmentsListModel GetSegments()
        {
            var affiliateId = Tenant.ApplicationId;
            var affiliate = _affiliateService.GetById(affiliateId);
            var items = affiliate.Segments?
                .Select(_objectRepository.Load<SegmentDocument, SegmentsListItemModel>).OrderBy(x => x.Name)
                .ToList() ?? new List<SegmentsListItemModel>();
            var model = new SegmentsListModel {Segments = items};

            return model;
        }

        [AllianceAuthorize(UserPermissionEnum.AffiliateAdminEdit)]
        [HttpGet("createSegment")]
        public SegmentViewModel CreateSegment()
        {
            var segment = new SegmentModel {IsNew = true, AffiliateId = Tenant.ApplicationId};
            _viewHelper.FormatReachNumber(segment);
            var model = new SegmentViewModel {Segment = segment};
            BindSegmentSelectLists(model);
            return model;
        }

        [AllianceAuthorize(UserPermissionEnum.AffiliateAdminEdit)]
        [HttpPost("createSegment")]
        public IActionResult CreateSegment([FromBody] Model.AffiliateAdmin.SegmentModel model)
        {
            if(!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            var segment = new SegmentModel
            {
                Id = model.Id,
                AffiliateId = Tenant.ApplicationId,
                Name = model.Name,
                Reach = model.Reach,
                IsNew = model.IsNew,
                ApplicationOptions = model.ApplicationOptions,
                Gender = model.Gender,
                AgeRangeFrom = model.AgeRangeFrom,
                AgeRangeTo = model.AgeRangeTo,
                State = model.State,
                ZipCodes = model.ZipCodes,
                DateRange = model.DateRange,
                CustomDateRangeStart = model.CustomDateRangeStart,
                CustomDateRangeEnd = model.CustomDateRangeEnd,
                FinancesOptions = model.FinancesOptions,
                MerchantSelections = model.MerchantSelections,
                MerchantOptions = model.MerchantOptions,
                SpendingCategories = model.SpendingCategories,
                SpendingCategoryOptions = model.SpendingCategoryOptions,
            };

            model.AffiliateId = Tenant.ApplicationId;
            var command = _objectRepository.Load<SegmentModel, Affiliate_Segment_AddCommand>(segment);
            Send(command);

            return new OkResult();
        }

        [AllianceAuthorize(UserPermissionEnum.AffiliateAdminEdit)]
        [HttpPost("estimateSegment")]
        public IActionResult EstimateSegment([FromBody] SegmentModel model)
        {
            if(ModelState.All(x => x.Key == "Name" || !x.Value.Errors.Any()))
            {
                ModelState.Clear();
                model.Reach = EstimationHelper.GetMatchingUsers(_objectRepository.Load<SegmentModel, SegmentData>(model)).Count;
                _viewHelper.FormatReachNumber(model);
                return new OkObjectResult(new {model.Reach, model.ReachFormatted });
            }

            return new BadRequestResult();
        }

        [AllianceAuthorize(UserPermissionEnum.AffiliateAdminEdit)]
        [HttpGet("editSegment/{id}")]
        public SegmentViewModel EditSegment(string id)
        {
            var segment = _objectRepository.Load<string, SegmentModel>(id);
            var model = new SegmentViewModel { Segment = segment };
            BindSegmentSelectLists(model);

            return model;
        }

        [AllianceAuthorize(UserPermissionEnum.AffiliateAdminEdit)]
        [HttpGet("clearFilters")]
        public SegmentViewModel ClearFilters(string segmentId)
        {
            var segment = new SegmentModel
            {
                Id = segmentId,
                AffiliateId = Tenant.ApplicationId,
                IsNew = string.IsNullOrEmpty(segmentId)
            };
            var model = new SegmentViewModel { Segment = segment };
            BindSegmentSelectLists(model);

            return model;
        }

        [AllianceAuthorize(UserPermissionEnum.AffiliateAdminEdit)]
        [HttpPost("editSegment")]
        public IActionResult EditSegment([FromBody] Model.AffiliateAdmin.SegmentModel model)
        {
            if(!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            var segment = new SegmentModel
            {
                Id = model.Id,
                AffiliateId = Tenant.ApplicationId,
                Name = model.Name,
                Reach = model.Reach,
                IsNew = model.IsNew,
                ApplicationOptions = model.ApplicationOptions,
                Gender = model.Gender,
                AgeRangeFrom = model.AgeRangeFrom,
                AgeRangeTo = model.AgeRangeTo,
                State = model.State,
                ZipCodes = model.ZipCodes,
                DateRange = model.DateRange,
                CustomDateRangeStart = model.CustomDateRangeStart,
                CustomDateRangeEnd = model.CustomDateRangeEnd,
                FinancesOptions = model.FinancesOptions,
                MerchantSelections = model.MerchantSelections,
                MerchantOptions = model.MerchantOptions,
                SpendingCategories = model.SpendingCategories,
                SpendingCategoryOptions = model.SpendingCategoryOptions,
            };
            var command = _objectRepository.Load<SegmentModel, Affiliate_Segment_UpdateCommand>(segment);
            Send(command);

            return new OkResult();
        }

        [AllianceAuthorize(UserPermissionEnum.AffiliateAdminDelete)]
        [HttpDelete("deleteSegment/{id}")]
        public IActionResult DeleteSegment(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new NotFoundResult();

            var command = new Affiliate_Segment_DeleteCommand
            {
                Id = id,
                AffiliateId = Tenant.ApplicationId
            };

            Send(command);

            return new OkResult();
        }

        private void BindSegmentSelectLists(SegmentViewModel model)
        {
            var expenseAccounts = new List<string> { "Uncategorized Expense" };
            foreach(var account in _accountsService.CommonPersonalExpenseAccounts())
            {
                expenseAccounts.Add(account.Name);
                expenseAccounts.AddRange(account.SubAccounts.Select(x => x.Name));
            }

            expenseAccounts.Sort();
            var categoriesList = expenseAccounts.ToDictionary(x => x, x => AccountLabelEnum.Expense);
            categoriesList.Add("Uncategorized Income", AccountLabelEnum.Income);

            model.CategoriesList = categoriesList.Select(pair => new GroupedSelectListItem
            {
                Text = $"{pair.Key}|{pair.Value.GetDescription()}",
                Value = pair.Key,
                Group = AccountingFormatter.GenericCategoryGroup(pair.Value)
            }).ToList();

            model.SpendingCategories = _accountsService.CommonPersonalExpenseAccounts().ToList();
        }
        #endregion
    }
}
