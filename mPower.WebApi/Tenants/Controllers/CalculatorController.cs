using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using mPower.WebApi.Tenants.ViewModels;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Web;
using mPower.Documents;
using Default.Helpers;
using mPower.Framework;
using mPower.Framework.Environment.MultiTenancy;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace mPower.WebApi.Tenants.Controllers
{
    //[Authorize("Pfm")]
    [Route("api/calculator")]
    public class CalculatorController : BaseController
    {
        private readonly FinancialBankingService _financialService;

        public CalculatorController(FinancialBankingService financialService,ICommandService command, IApplicationTenant tenant) :
            base(command, tenant)
        {
            _financialService = financialService;
        }


        [HttpGet("GetMarketHistoryDateRange")]
        public List<int> GetMarketHistoryDateRange()
        {
            var historyData = _financialService.GetAll();
            List<int> dateRange = new List<int>();
            dateRange.Add(Convert.ToInt32(historyData.FirstOrDefault().Year));
            dateRange.Add(Convert.ToInt32(historyData.AsEnumerable().OrderByDescending(x => Convert.ToInt32(x.Year)).FirstOrDefault().Year));
            return dateRange;
        }
        [HttpPost("GetSPHistoryData")]
        public List<double> DownloadCashFlowData(int StartYear, int EndYear)
        {
            var historyData = _financialService.GetAll();
            List<double> SPHistoryData = historyData.AsEnumerable().Where(x => Convert.ToInt32(x.Year) >= StartYear && Convert.ToInt32(x.Year) <= EndYear)
                .Select(x => x.SPWithDividends).ToList();
            return SPHistoryData;
        }


        [HttpGet("marketHistoryCalculator")]
        public MarketHistoryCalculatorData GetMarketHistoryData()
        {
            var model = new MarketHistoryCalculatorData();
            var historyData = _financialService.GetAll();
            
            model.ListofData = historyData.Select(x => new
                   ListofMarketHistoryData
                    {
                //_id=x._id,
                        Year = x.Year,
                        DJI = x.DJI,
                        SPWithoutDividends = x.SPWithoutDividends,
                        SPWithDividends= x.SPWithDividends
                    }).ToList();
            model.FirstYear = Convert.ToInt32(model.ListofData.FirstOrDefault().Year);
            model.LastYear = Convert.ToInt32(model.ListofData.OrderByDescending(x=> x.Year).FirstOrDefault().Year);

            double AvgDJI = 0, AvgSPWOD = 0, AvgSPWD = 0;
            foreach (var item in model.ListofData)
            {
                AvgDJI += item.DJI;
                AvgSPWOD += item.SPWithoutDividends;
                AvgSPWD += item.SPWithDividends;
            }

            SummaryOfMarketHistoryData avg = new SummaryOfMarketHistoryData();
            avg.Year = "Average ROR";
            avg.DJI = Math.Round(AvgDJI / model.ListofData.Count,2) + "%";
            avg.SPWithoutDividends = Math.Round(AvgSPWOD / model.ListofData.Count,2) + "%";
            avg.SPWithDividends = Math.Round(AvgSPWD / model.ListofData.Count,2) + "%";

            SummaryOfMarketHistoryData actual = new SummaryOfMarketHistoryData();
            actual.Year = "Actual ROR";
            model.SummaryData.Add(avg);
            model.SummaryData.Add(actual);
            return model;
        }
    }
}
