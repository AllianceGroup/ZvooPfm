using System;
using System.Collections.Generic;
using System.Web.Mvc;
using mPower.Documents.DocumentServices.Membership;
using mPower.Documents.DocumentServices.Membership.Filters;
using mPower.Domain.Membership.Enums;
using mPower.Domain.Membership.User.Commands;
using mPower.Framework.Utils;
using mPower.Web.Admin.Models;

namespace mPower.Web.Admin.Controllers
{
    public enum UserLoginsDayFilterEnum
    {
        Today = 1,
        Yesterday,
        ThisWeek,
        LastWeek,
        ThisMonth,
        LastMonth
    }

    public class UserLoginsController : BaseAdminController
    {
        private readonly UserLoginsDocumentService _userLoginsService;

        public UserLoginsController(UserLoginsDocumentService userLoginsService)
        {
            _userLoginsService = userLoginsService;
        }

        public ActionResult Index(string searchKey = null, UserLoginsDayFilterEnum date = UserLoginsDayFilterEnum.Today, UserLoginsSortEnum sort = UserLoginsSortEnum.LoginDate)
        {
            var from = DateTime.MinValue;
            var to = DateTime.MaxValue;

            switch (date)
            {
                case UserLoginsDayFilterEnum.Today:
                    from = DateUtil.GetStartOfDay(DateTime.Now);
                    to = DateUtil.GetEndOfDay(DateTime.Now);
                    break;
                case UserLoginsDayFilterEnum.Yesterday:
                    from = DateUtil.GetStartOfDay(DateTime.Now.AddDays(-1));
                    to = DateUtil.GetEndOfDay(DateTime.Now.AddDays(-1));
                    break;
                case UserLoginsDayFilterEnum.ThisWeek:
                    from = DateUtil.GetStartOfCurrentWeek();
                    to = DateUtil.GetEndOfCurrentWeek();
                    break;
                case UserLoginsDayFilterEnum.LastWeek:
                    from = DateUtil.GetStartOfLastWeek();
                    to = DateUtil.GetEndOfLastWeek();
                    break;
                case UserLoginsDayFilterEnum.ThisMonth:
                    from = DateUtil.GetStartOfCurrentMonth();
                    to = DateUtil.GetEndOfCurrentMonth();
                    break;
                case UserLoginsDayFilterEnum.LastMonth:
                    from = DateUtil.GetStartOfLastMonth();
                    to = DateUtil.GetEndOfLastMonth();
                    break;
            }

            var logins = _userLoginsService.GetByFilter(new UserLoginsFilter() { From = from, To = to, SearchKey = searchKey, SortByField = sort });
            var model = FillModel(searchKey, date, sort);
            model.Logins = logins;

            return View(model);
        }

        public UserLoginsModel FillModel(string searchKey, UserLoginsDayFilterEnum date, UserLoginsSortEnum sort)
        {
            var model = new UserLoginsModel();
            model.DateOptions = new List<SelectListItem>();
            model.DateOptions.Add(new SelectListItem() { Text = "Today", Value = "1" });
            model.DateOptions.Add(new SelectListItem() { Text = "Yesterday", Value = "2" });
            model.DateOptions.Add(new SelectListItem() { Text = "This Week", Value = "3" });
            model.DateOptions.Add(new SelectListItem() { Text = "Last Week", Value = "4" });
            model.DateOptions.Add(new SelectListItem() { Text = "This Month", Value = "5" });
            model.DateOptions.Add(new SelectListItem() { Text = "Last Month", Value = "6" });
            model.SearchKey = searchKey;
            foreach (var item in model.DateOptions)
            {
                if (item.Value == ((int)date).ToString())
                    item.Selected = true;
            }


            model.SortOptions = new List<SelectListItem>();
            model.SortOptions.Add(new SelectListItem() { Text = "Login Date", Value = "1" });
            model.SortOptions.Add(new SelectListItem() { Text = "Affiliate Name", Value = "2" });
            foreach (var item in model.SortOptions)
            {
                if (item.Value == ((int)sort).ToString())
                    item.Selected = true;
            }

            return model;
        }


        public ActionResult AddGlobalAdmin()
        {

            return View();
        }

        [HttpPost]
        public ActionResult AddGlobalAdmin(string userId)
        {
            Send(new User_AddPermissionCommand { Permission = UserPermissionEnum.GlobalAdminDelete, UserId = userId },
                    new User_AddPermissionCommand { Permission = UserPermissionEnum.GlobalAdminEdit, UserId = userId },
                    new User_AddPermissionCommand { Permission = UserPermissionEnum.GlobalAdminView, UserId = userId });

            return Content("Done");

        }
    }
}
