using System.Collections.Generic;
using Default.ViewModel.Areas.Finance.DebtToolsController;
using mPower.Documents.Documents.Accounting.DebtElimination;
using mPower.Domain.Accounting.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace mPower.WebApi.Tenants.ViewModels.DebtElimninationProgram
{
    public class Step3Model
    {
        public double PayoffTime { get; set; }
        public double PayoffTimeViaPlan { get; set; }

        public double TotalPayed { get; set; }
        public double TotalPayedViaPlan { get; set; }

        public double MoneySaved => TotalPayed - TotalPayedViaPlan;

        public double YearsSaved => PayoffTime - PayoffTimeViaPlan;

        public SelectList DisplayModes { get; set; }

        public ChartViewModel Chart { get; set; }

        public List<ProgramDetailsItemShort> Details { get; set; }

        public string ExtraAmount { get; set; }

        public bool AddedToCalendar { get; set; }

        public DebtEliminationDisplayModeEnum DisplayMode { get; set; }

        public Step3Model()
        {
            Details = new List<ProgramDetailsItemShort>();
        }
    }
}
