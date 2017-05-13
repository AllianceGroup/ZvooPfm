using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using FusionChartsMVC.FusionCharts;
using FusionChartsMVC.FusionCharts.Charts;
using FusionChartsMVC.FusionCharts.Styles;
using Font = FusionChartsMVC.FusionCharts.Styles.Font;

namespace mPower.Framework.Mvc.Helpers
{
    public static class Charts
    {
        private const double PieChartWidthRatio = .50;
        private const double PieChartHeightRatio = .75;
        private const int DashboardChartWidth = 460;
        private const int DashboardChartHeight = 218;

        public static ColumnChart LenderRiskColumnChart(double lenderRiskPercentage,
                                                        string divId = "LenderRiskGraph",
                                                        int width = 375,
                                                        int height = 250)
        {
            int selectedColumn = 0;

            if (lenderRiskPercentage <= 89)
                selectedColumn = 0;
            if (lenderRiskPercentage <= 70)
                selectedColumn = 1;
            if (lenderRiskPercentage <= 51)
                selectedColumn = 2;
            if (lenderRiskPercentage <= 31)
                selectedColumn = 3;
            if (lenderRiskPercentage <= 14)
                selectedColumn = 4;
            if (lenderRiskPercentage <= 5)
                selectedColumn = 5;
            if (lenderRiskPercentage <= 2)
                selectedColumn = 6;
            if (lenderRiskPercentage <= 1)
                selectedColumn = 7;

            var columnBars = new List<SetElement>
                             {
                                 new SetElement(89, "Up To 499", Color.Gray),
                                 new SetElement(70, "500 - 549", Color.Gray),
                                 new SetElement(51, "550 - 599", Color.Gray),
                                 new SetElement(31, "600 - 649", Color.Gray),
                                 new SetElement(14, "650 - 699", Color.Gray),
                                 new SetElement(5, "700 - 749", Color.Gray),
                                 new SetElement(2, "750 - 799", Color.Gray),
                                 new SetElement(1, "800&#43;", Color.Gray)
                             };
            columnBars[selectedColumn].Color = Color.DodgerBlue;

            //Make vLine elements
            List<VLineElement> vLines = new List<VLineElement>();
            vLines.Add(new VLineElement(selectedColumn, "You Are Here", Color.DodgerBlue) 
            { 
                ShowLabelBorder = false, 
                LineOrientation = HAlign.Right, 
                Dashed = true, 
            });
            //colGraph.VerticalLines.Add(new VLine("selected", selectedColumn) { IsLabelBorderShowing = false, LineOrientation = HAlign.Right, IsDashed = true, Color = Color.DodgerBlue, Label = "You Are Here" });

            ColumnChart colGraph = new ColumnChart(false, divId, width, height, columnBars, vLines);
            colGraph.NumDivLines = 0;
            colGraph.IsAlternateHGridColorShowing = false;
            colGraph.IsRoundedEdges = true;
            colGraph.DivLineAlpha = 0;
            colGraph.Animation = true;
            colGraph.NumDivLines = 10;
            colGraph.YAxisMinValue = 0;
            colGraph.YAxisMaxValue = 115;
            colGraph.IsBorderVisible = false;
            colGraph.CanvasBackgroundAlphaTop = 0;
            colGraph.BackgroundColorTop = Color.White;
            colGraph.BackgroundAlphaBottom = 0;
            colGraph.IsYAxisValuesVisible = false;
            colGraph.NumberSuffix = "%";

            //Create Styles and Apply elements, then set them on the chart
            List<StyleElement> styles = new List<StyleElement>();
            styles.Add(new Font("myLabelsFont") { Color = Color.FromArgb(105, 105, 105), IsBold = true });

            List<ApplyElement> applyElements = new List<ApplyElement>();
            applyElements.Add(new ApplyElement(ChartObject.DataLabels, "myLabelsFont"));
            applyElements.Add(new ApplyElement(ChartObject.DataValues, "myLabelsFont"));

            colGraph.SetStyles(styles, applyElements);

            //FusionChartsMVC.Model.Font font = new FusionChartsMVC.Model.Font("myLabelsFont") { Color = Color.FromArgb(105, 105, 105), IsBold = true };
            //colGraph.AddStyleToChart(font, ChartObject.DataLabels);
            //colGraph.AddStyleToChart(font, ChartObject.DataValues);

            return colGraph;
        }


        public static ColumnChart CreditDistributionColumnChart(double creditDistributionPercentage,
                                                        string divId = "CreditDistributionGraph",
                                                        int width = 375,
                                                        int height = 250)
        {
            int selectedColumn = 0;

            if (creditDistributionPercentage == 2.1)
                selectedColumn = 0;
            if (creditDistributionPercentage == 4.5)
                selectedColumn = 1;
            if (creditDistributionPercentage == 5.4)
                selectedColumn = 2;
            if (creditDistributionPercentage == 6.5)
                selectedColumn = 3;
            if (creditDistributionPercentage == 7.9)
                selectedColumn = 4;
            if (creditDistributionPercentage == 9.6)
                selectedColumn = 5;
            if (creditDistributionPercentage == 12.0)
                selectedColumn = 6;
            if (creditDistributionPercentage == 13.9)
                selectedColumn = 7;
            if (creditDistributionPercentage == 17.0)
                selectedColumn = 8;
            if (creditDistributionPercentage == 15.9)
                selectedColumn = 9;
            if (creditDistributionPercentage == 5.7)
                selectedColumn = 10;


            var columnBars = new List<SetElement>()
                             {
                                 new SetElement(2.1, "300 - 349", Color.Gray),
                                 new SetElement(4.5, "350 - 399", Color.Gray),
                                 new SetElement(5.4, "400 - 449", Color.Gray),
                                 new SetElement(6.5, "450 - 499", Color.Gray),
                                 new SetElement(7.9, "500 - 549", Color.Gray),
                                 new SetElement(9.6, "550 - 599", Color.Gray),
                                 new SetElement(12.0,"600 - 649", Color.Gray),
                                 new SetElement(13.9,"650 - 699", Color.Gray),
                                 new SetElement(17.0,"700 - 749", Color.Gray),
                                 new SetElement(15.9, "750 - 799", Color.Gray),
                                 new SetElement(5.7, "800&#43;", Color.Gray)
                             };

            columnBars[selectedColumn].Color = Color.DodgerBlue;

            //Make vLine elements
            List<VLineElement> vLines = new List<VLineElement>();
            vLines.Add(new VLineElement(selectedColumn, "You Are Here", Color.DodgerBlue)
            {
                ShowLabelBorder = false,
                LineOrientation = HAlign.Right,
                Dashed = true,
            });
            //colGraph.VerticalLines.Add(new VLine("selected", selectedColumn) { IsLabelBorderShowing = false, LineOrientation = HAlign.Right, IsDashed = true, Color = Color.DodgerBlue, Label = "You Are Here" });
            
            ColumnChart colGraph = new ColumnChart(false, divId, width, height, columnBars);
            colGraph.NumDivLines = 0;
            colGraph.IsAlternateHGridColorShowing = false;
            colGraph.IsRoundedEdges = true;
            colGraph.DivLineAlpha = 0;
            colGraph.Animation = true;
            colGraph.NumDivLines = 10;
            colGraph.YAxisMinValue = 0;
            colGraph.YAxisMaxValue = 25;
            colGraph.IsBorderVisible = false;
            colGraph.CanvasBackgroundAlphaTop = 0;
            colGraph.BackgroundColorTop = Color.White;
            colGraph.BackgroundAlphaBottom = 0;
            colGraph.IsYAxisValuesVisible = false;
            colGraph.CanvasBottomMargin = 0;
            colGraph.NumberSuffix = "%";

            //Create Styles and Apply elements, then set them on the chart
            List<StyleElement> styles = new List<StyleElement>();
            styles.Add(new Font("myLabelsFont") { Color = Color.FromArgb(105, 105, 105), IsBold = true });

            List<ApplyElement> applyElements = new List<ApplyElement>();
            applyElements.Add(new ApplyElement(ChartObject.DataLabels, "myLabelsFont"));
            applyElements.Add(new ApplyElement(ChartObject.DataValues, "myLabelsFont"));

            colGraph.SetStyles(styles, applyElements);

            //FusionChartsMVC.Model.Font font = new FusionChartsMVC.Model.Font("myLabelsFont") { Color = Color.FromArgb(105, 105, 105), IsBold = true };
            //colGraph.AddStyleToChart(font, ChartObject.DataLabels);
            //colGraph.AddStyleToChart(font, ChartObject.DataValues);

            return colGraph;
        }


        public static LineChart ScoreHistoryLineChart(IEnumerable<SetElement> linePoints,
                                                        string divId = "ScoreHistoryGraph",
                                                        int width = 500,
                                                        int height = 350)
        {
            LineChart LineChart = new LineChart(false, divId, width, height, linePoints.ToList());
            LineChart.YAxisName = "Credit Score";
            LineChart.RotatedLabels = true;
            LineChart.SlantLabels = true;
            LineChart.YAxisMinValue = 300;
            LineChart.YAxisMaxValue = 800;
            LineChart.BackgroundColorTop = Color.White;
            LineChart.ConnectNullData = true;
            LineChart.Animation = true;
            LineChart.IsBorderVisible = false;
            LineChart.ShowValues = false;

            //Create Styles and Apply elements, then set them on the chart
            List<StyleElement> styles = new List<StyleElement>();
            styles.Add(new Font("myLabelsFont") { Color = Color.FromArgb(105, 105, 105), IsBold = true });

            List<ApplyElement> applyElements = new List<ApplyElement>();
            applyElements.Add(new ApplyElement(ChartObject.DataLabels, "myLabelsFont"));
            applyElements.Add(new ApplyElement(ChartObject.YAxisName, "myLabelsFont"));
            applyElements.Add(new ApplyElement(ChartObject.YAxisValues, "myLabelsFont"));
            LineChart.SetStyles(styles, applyElements);

            //FusionChartsMVC.Model.Font font = new FusionChartsMVC.Model.Font("myLabelsFont") { Color = Color.FromArgb(105, 105, 105), IsBold = true };
            //LineChart.AddStyleToChart(font, ChartObject.DataLabels);
            //LineChart.AddStyleToChart(font, ChartObject.YAxisName);
            //LineChart.AddStyleToChart(font, ChartObject.YAxisValues);
            return LineChart;
        }

        public static MultiSeriesLineChart MortgageAcceleration(List<DataSetElement> dataSets, CategoriesElement categories, bool forDashboard, string divId = "PaymentScheduleGraph")
        {
            var width = forDashboard ? DashboardChartWidth : 700;
            var height = forDashboard ? DashboardChartHeight : 480;

            var msLineChart = new MultiSeriesLineChart(false, divId, width, height, categories, Encode(dataSets));

            if (forDashboard)
            {
                msLineChart.BackgroundColorBottom = Color.Transparent;
                msLineChart.BackgroundColorTop = Color.Transparent;
                msLineChart.IsBorderVisible = false;
            }

            return msLineChart;
        }

        public static MultiSeriesLineChart DebtElimination(List<DataSetElement> dataSets, CategoriesElement categories,
            string divId = "PaymentScheduleGraph", int width = 700, int height = 480)
        {
            var msLineChart = new MultiSeriesLineChart(false, divId, width, height, categories, Encode(dataSets));

            return msLineChart;
        }

        public static PieChart TrendsChart(List<SetElement> pieces, string divId = "trendsChart", string title = "")
        {
            var graph = new PieChart(false, divId, 600, 400, Encode(pieces),"$");
            graph.PieRadius = GetLargestPossiblePieChartRadius(graph.Width, graph.Height);
            graph.ShowZeroPies = false;
            graph.FormatNumberScale = false;
            graph.ManageLabelOverflow = true;
            graph.UseEllipsesWhenOverflow = true;
            graph.ShowValues = false;
            graph.Animation = true;
            graph.Palette = 3;
            graph.Title = title;

            return graph;
        }

        public static PieChart DebtToIncomeChart(List<SetElement> pieces, string divId, bool forDashboard, string title = "")
        {
            var width = forDashboard ? DashboardChartWidth/2 : 290;
            var height = forDashboard ? DashboardChartHeight : 250;

            var graph = new PieChart(false, divId, width, height, Encode(pieces), "$");
            graph.PieRadius = GetLargestPossiblePieChartRadius(graph.Width, graph.Height);
            graph.ShowZeroPies = false;
            graph.FormatNumberScale = false;
            graph.ManageLabelOverflow = true;
            graph.UseEllipsesWhenOverflow = true;
            graph.ShowValues = true;
            graph.Animation = true;
            graph.Palette = 3;
            graph.Title = title;
            graph.ChartBottomMargin = 0;
            graph.ChartLeftMargin = 0;
            graph.ChartRightMargin = 0;
            graph.ChartTopMargin = 0;
            graph.BackgroundColorBottom = Color.Transparent;
            graph.BackgroundColorTop = Color.Transparent;
            graph.IsBorderVisible = false;


            return graph;
        }

        public static PieChart DasboardAccountsChart(List<SetElement> pieces, string divId = "dasboardAccountsChart", string title = "")
        {
            var graph = new PieChart(false, divId, DashboardChartWidth, DashboardChartHeight, Encode(pieces), "$");
            graph.PieRadius = GetLargestPossiblePieChartRadius(graph.Width, graph.Height);
            graph.ShowZeroPies = false;
            graph.FormatNumberScale = false;
            graph.ManageLabelOverflow = true;
            graph.UseEllipsesWhenOverflow = true;
            graph.ShowValues = true;
            graph.Title = title;
            graph.Animation = true;
            graph.Palette = 3;
            graph.Title = title;
            graph.ChartBottomMargin = 0;
            graph.ChartLeftMargin = 0;
            graph.ChartRightMargin = 0;
            graph.ChartTopMargin = 0;
            graph.BackgroundColorBottom = Color.Transparent;
            graph.BackgroundColorTop = Color.Transparent;
            graph.IsBorderVisible = false;

            return graph;
        }

        public static MultiSeriesLineChart DasboardBudgetsChart(List<SetElement> spent, List<SetElement> budget, string divId = "dasboardBudgetsChart", string title = "")
        {
            var categories = new CategoriesElement(budget.Select(b => new CategoryElement(b.Label)).ToList());
            var datasets = new List<DataSetElement>
            {
                new DataSetElement(spent) {SeriesName = "Expense"},
                new DataSetElement(budget) {SeriesName = "Budget"},
            };

            var graph = new MultiSeriesLineChart(false, divId, DashboardChartWidth, DashboardChartHeight, categories, Encode(datasets));
            graph.Title = title;
            graph.FormatNumberScale = false;
            graph.Animation = true;
            graph.ColorPalette = 3;
            graph.Title = title;
            graph.BackgroundColorBottom = Color.Transparent;
            graph.BackgroundColorTop = Color.Transparent;
            graph.IsBorderVisible = false;

            return graph;
        }

        /// <summary>
        /// Method used to get the largest radius the chart can be, without it's height and width ratios going off of the canvas.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private static int GetLargestPossiblePieChartRadius(double width,double height)
        {
            double desiredWidth = width*PieChartWidthRatio;
            double desiredHeight = height*PieChartHeightRatio;

            int largestPossiblePieDiameter = (int)desiredWidth;
            if (desiredHeight < largestPossiblePieDiameter)
                largestPossiblePieDiameter = (int)desiredHeight;

            int largestPossiblePieRadius = largestPossiblePieDiameter / 2;

            return largestPossiblePieRadius;
        }

        private static List<DataSetElement> Encode(List<DataSetElement> dataSets)
        {
            dataSets.Each(ds => ds.SeriesName = HttpUtility.HtmlEncode(ds.SeriesName));
            return dataSets;
        }

        private static List<SetElement> Encode(List<SetElement> sets)
        {
            sets.Each(s => s.Label = HttpUtility.HtmlEncode(s.Label));
            return sets;
        }
    }
}