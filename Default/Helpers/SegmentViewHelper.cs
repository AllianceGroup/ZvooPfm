using Default.Areas.Administration.Models;
using Default.ViewModel;
using mPower.Documents.DocumentServices.Membership;
using mPower.Documents.DocumentServices.Membership.Filters;
using mPower.Documents.Segments;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Application.Affiliate.Data;
using mPower.Framework.Mvc;
using mPower.Framework.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Default.Helpers
{
    public class SegmentViewHelper
    {
        private readonly AccountsService _accountsService;
        private readonly UserDocumentService _userService;
        private readonly IObjectRepository _objectRepository;

        public SegmentViewHelper(AccountsService accountsService, UserDocumentService userService, IObjectRepository objectRepository)
        {
            _accountsService = accountsService;
            _userService = userService;
            _objectRepository = objectRepository;
        }

        public void FormatReachNumber(SegmentModel model)
        {
            var usersCount = model.AllOptions.Select(_objectRepository.Load<SegmentOptionModel, SegmentOption>).IsShowNetworkUsersEnabled() 
                ? _userService.Count() 
                : _userService.Count(new UserFilter {AffiliateId = model.AffiliateId});
            model.ReachFormatted = string.Format("{0} of {1}", model.Reach, usersCount);
        }
    }
}