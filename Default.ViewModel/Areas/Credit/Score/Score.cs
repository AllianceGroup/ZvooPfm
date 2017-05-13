using System.Collections.Generic;
using Default.ViewModel.Areas.Credit.IdGuard;
using FusionChartsMVC.FusionCharts.Charts;

namespace Default.ViewModel.Areas.Credit.Score
{
    public class Score
	{
        public string ClientKey { get; set; }
        public bool EligibleForNewCreditReport { get; set; }
        public LineChart ScoreHistoryLineGraph { get; set; }
        public ColumnChart LenderRiskColumnGraph { get; set; }
        public ColumnChart CreditDistributionColumnGraph { get; set; }
        public string Date { get; set; }
		public int CreditScore { get; set; }
		public int PercentageRating { get; set; }
		public int QualitativeRank { get; set; }
		public double LenderRiskPercentage { get; set; }
		public double CreditDistributionPercentage { get; set; }
		public string Grade { get; set; }
		public List<CreditAlert> CreditAlerts { get; set; }
        public CreditAlertSignUp CreditAlertSignUp { get; set; }


    }
}
