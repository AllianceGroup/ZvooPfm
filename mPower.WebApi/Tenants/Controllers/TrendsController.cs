using System;
using Default.ViewModel.Areas.Finance.TrendsController;
using mPower.Framework;
using mPower.Framework.Environment.MultiTenancy;
using mPower.Framework.Mvc;
using mPower.WebApi.Tenants.ViewModels.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrendModelFilterEnum = mPower.WebApi.Tenants.Model.Trends.TrendModelFilterEnum;
using TrendsModel = mPower.WebApi.Tenants.Model.Trends.TrendsModel;

namespace mPower.WebApi.Tenants.Controllers
{
    [Authorize("Pfm")]
    [Route("api/[controller]")]
    public class TrendsController : BaseController
    {
        private readonly IObjectRepository _objectRepository;

        public TrendsController(IObjectRepository objectRepository,
            ICommandService command, IApplicationTenant tenant) :base(command, tenant)
        {
            _objectRepository = objectRepository;
        }

        [HttpGet("GetTrends")]
        public TrendsModel GetTrends()
        {
            var date = DateTime.Now;
            var filter = new TrendsViewModelFilter
            {
                Filter = TrendModelFilterEnum.SelectedMonth,
                ShowFormat = ShowFormatEnum.Spending,
                Month = date.Month,
                Year = date.Year,
                LedgerId = GetLedgerId()
            };

            var model = _objectRepository.Load<TrendsViewModelFilter, TrendsModel>(filter);

            return model;
        }

        [HttpPost("Refresh")]
        public TrendsModel Refresh([FromBody] TrendsViewModelFilter input)
        {
            input.LedgerId = GetLedgerId();
            if (input.Filter == TrendModelFilterEnum.PrevMonth)
            {
                var date = DateTime.Now.AddMonths(-1);
                input.Month = date.Month;
                input.Year = date.Year;
            }
            if (input.Filter == TrendModelFilterEnum.ThisMonth)
            {
                var date = DateTime.Now;
                input.Month = date.Month;
                input.Year = date.Year;
            }
            input.TakeCategories = input.All ? 1000 : 10;
            input.LedgerId = GetLedgerId();

            var model = _objectRepository.Load<TrendsViewModelFilter, TrendsModel>(input);
            return model;
        }

    }
}
