using System;

namespace mPower.Framework.Utils.CreditCalculator
{
    public static class CreditCalculator
    {
        public static double LenderRisk(int transriskScore)
        {
            int rv = 89;
            if (transriskScore >= 800)
                rv = 1;
            else if (transriskScore >= 750)
                rv = 2;
            else if (transriskScore >= 700)
                rv = 5;
            else if (transriskScore >= 650)
                rv = 14;
            else if (transriskScore >= 600)
                rv = 31;
            else if (transriskScore >= 550)
                rv = 51;
            else if (transriskScore >= 500)
                rv = 70;
            return rv;
        }

        public static double CreditDistribution(int transriskScore)
        {
            double rv = 2.1;

            if (transriskScore >= 350)
                rv = 4.5;

            if (transriskScore >= 400)
                rv = 5.4;

            if (transriskScore >= 450)
                rv = 6.5;

            if (transriskScore >= 500)
                rv = 7.9;

            if (transriskScore >= 550)
                rv = 9.6;

            if (transriskScore >= 600)
                rv = 12.0;

            if (transriskScore >= 650)
                rv = 13.9;

            if (transriskScore >= 700)
                rv = 17.0;

            if (transriskScore >= 750)
                rv = 15.9;

            if (transriskScore >= 800)
                rv = 5.7;

            return rv;
        }

        public static string CreditGrade(int populationRank)
        {
            //http://www.augustana.ab.ca/~jmohr/courses/common/conversion.alpha.shtml

            if (populationRank >= 90)
                return "A+";

            if (populationRank >= 85)
                return "A";

            if (populationRank >= 80)
                return "A-";

            if (populationRank >= 77)
                return "B+";

            if (populationRank >= 73)
                return "B";

            if (populationRank >= 70)
                return "B-";

            if (populationRank >= 67)
                return "C+";

            if (populationRank >= 63)
                return "C";

            if (populationRank >= 60)
                return "C-";

            if (populationRank >= 55)
                return "D+";

            if (populationRank >= 50)
                return "D";

            if (populationRank >= 0)
                return "D";

            return "Unknown";
        }

        public static string MaskSocialSecurityNumber(string ssn)
        {
            const int visiblePartLength = 4;
            var startIndex = ssn.Length - visiblePartLength;

            return startIndex > 0
                ? string.Format("xxx-xx-{0}", ssn.Substring(startIndex, visiblePartLength))
                : ssn;
        }

        #region Credit Report Card

        public static ReportCardOut CalculateCreditReportGrades(ReportCardIn report)
        {
            var occuRatio =  OpenCreditCardUtilizationRatio(report.CreditLimit, report.CurrentBalance);
            var percotRatio = PercentPaymentsOnTimeRatio(report.TotalPayments, report.TotalLatePayments);
            var avgAge = AverageAgeOfOpenCredit(report.TotalNumberOfOpenCards, report.TotalNumberOfDaysCardsHaveBeenOpen);
            var yFactor = YFactor(report.OpenAccountsCount, report.ClosedAccountsCount, report.RevolvingAccountsCount, report.InstallmentAccountsCount, report.MortgageAccountsCount, report.OtherAccountsCount);
            var totalNegativeFactors = report.NegativeFactorsCount + report.PublicRecordsCount;
            var card = new ReportCardOut
            { 
                OpenCreditCardUtilizationRatio = occuRatio ,
                OpenCreditCardUtilizationGrade = OpenCreditCardUtilizationGrade(occuRatio),
                PercentOfPaymentsOnTimeRatio = percotRatio,
                PercentPaymentsOnTimeGrade = PercentPaymentsOnTimeGrade(percotRatio),
                AvgAge = (int)avgAge / 12 + " Yrs, " + (int)avgAge % 12 + " Mos",
                AvgAgeGrade = AverageAgeOfOpenCreditGrade(avgAge),
                TotalAccounts =  report.OpenAccountsCount + report.ClosedAccountsCount,
                TotalAccountsGrade = TotalAccountsGrade(yFactor),
                InqueriesCount = report.InqueriesCount,
                InqueriesGrade = HardCreditInquriesGrade(report.InqueriesCount),
                OtherNegativeFactorsCount = report.NegativeFactorsCount + report.PublicRecordsCount,
                OtherNegativeFactorsGrade = OtherNegativeFactorGrade(totalNegativeFactors)                  
            };
            return card;
        }

        private static double OpenCreditCardUtilizationRatio(double creditLimit, double currentBalance)
        {
            double openCreditCardUtilization = 0;

            if (creditLimit > 0)
                openCreditCardUtilization = 100.0 * currentBalance / creditLimit;

            return openCreditCardUtilization;
        }

        private static string OpenCreditCardUtilizationGrade(double openCreditCardUtilizationRatio)
        {
            var occuGrade = "A+";

            if (openCreditCardUtilizationRatio > 90)
                occuGrade = "D";
            else if (openCreditCardUtilizationRatio > 80)
                occuGrade = "C-";
            else if (openCreditCardUtilizationRatio > 70)
                occuGrade = "C";
            else if (openCreditCardUtilizationRatio > 60)
                occuGrade = "C+";
            else if (openCreditCardUtilizationRatio > 50)
                occuGrade = "B-";
            else if (openCreditCardUtilizationRatio > 40)
                occuGrade = "B";
            else if (openCreditCardUtilizationRatio > 30)
                occuGrade = "B+";
            else if (openCreditCardUtilizationRatio > 20)
                occuGrade = "A-";
            else if (openCreditCardUtilizationRatio > 10)
                occuGrade = "A";

            return occuGrade;
        }


        private static double PercentPaymentsOnTimeRatio(double totalPayments, double totalLatePayments)
        {
            double percot = 0;
            if (totalPayments > 0)
            {
                percot = 100 * (totalPayments - totalLatePayments) / totalPayments;
            }
            return percot;
        }

        private static string PercentPaymentsOnTimeGrade(double percot)
        {
            string percotGrade = "D";
            if (percot > 95)
                percotGrade = "A+";
            else if (percot > 90)
                percotGrade = "A";
            else if (percot > 85)
                percotGrade = "A-";
            else if (percot > 80)
                percotGrade = "B+";
            else if (percot > 75)
                percotGrade = "B";
            else if (percot > 70)
                percotGrade = "B-";
            else if (percot > 65)
                percotGrade = "C+";
            else if (percot > 60)
                percotGrade = "C";
            else if (percot > 55)
                percotGrade = "C-";
            return percotGrade;
        }

        private static double AverageAgeOfOpenCredit(double totalNumberOfCards, double totalDays)
        {
            int avgDays = 0;
            if (totalNumberOfCards > 0)
                avgDays = (int)(totalDays / totalNumberOfCards);
            double Months = avgDays / 30;
            return Months;
        }

        private static string AverageAgeOfOpenCreditGrade(double months)
        {
            string ageGrade = "D";
            if (months > 81)
                ageGrade = "A+";
            else if (months > 72)
                ageGrade = "A";
            else if (months > 63)
                ageGrade = "A-";
            else if (months > 54)
                ageGrade = "B+";
            else if (months > 45)
                ageGrade = "B";
            else if (months > 36)
                ageGrade = "B-";
            else if (months > 27)
                ageGrade = "C+";
            else if (months > 18)
                ageGrade = "C";
            else if (months > 9)
                ageGrade = "C-";
            return ageGrade;
        }

        private static string HardCreditInquriesGrade(int inqCount)
        {
            string inqGrade = "A+";
            if (inqCount > 9)
                inqGrade = "D";
            else if (inqCount > 8)
                inqGrade = "C-";
            else if (inqCount > 7)
                inqGrade = "C";
            else if (inqCount > 6)
                inqGrade = "C+";
            else if (inqCount > 5)
                inqGrade = "B-";
            else if (inqCount > 4)
                inqGrade = "B";
            else if (inqCount > 3)
                inqGrade = "B+";
            else if (inqCount > 2)
                inqGrade = "A-";
            else if (inqCount > 1)
                inqGrade = "A";
            return inqGrade;
        }

        private static string OtherNegativeFactorGrade(int negCount)
        {
            string negGrade = "A+";
            if (negCount > 9)
                negGrade = "D";
            else if (negCount > 8)
                negGrade = "C-";
            else if (negCount > 7)
                negGrade = "C";
            else if (negCount > 6)
                negGrade = "C+";
            else if (negCount > 5)
                negGrade = "B-";
            else if (negCount > 4)
                negGrade = "B";
            else if (negCount > 3)
                negGrade = "B+";
            else if (negCount > 2)
                negGrade = "A-";
            else if (negCount > 1)
                negGrade = "A";
            return negGrade;
        }

        private static double YFactor(int openAccountsCount, int closedAccountsCount, int revolvingCount, int installmentCount, int mortgageCount, int OtherCount)
        {
            var total = openAccountsCount + closedAccountsCount;
            double median = total / 3;
            double xFactor = Math.Abs(revolvingCount - median);
            xFactor += Math.Abs(installmentCount - median);
            xFactor += Math.Abs(mortgageCount - median);
            double yFactor = 100 * xFactor / total;
            return yFactor;
        }

        private static string TotalAccountsGrade(double yFactor)
        {
            string countGrade = "A+";
            if (yFactor > 90)
                countGrade = "D";
            else if (yFactor > 80)
                countGrade = "C-";
            else if (yFactor > 70)
                countGrade = "C";
            else if (yFactor > 60)
                countGrade = "C+";
            else if (yFactor > 50)
                countGrade = "B-";
            else if (yFactor > 40)
                countGrade = "B";
            else if (yFactor > 30)
                countGrade = "B+";
            else if (yFactor > 20)
                countGrade = "A-";
            else if (yFactor > 10)
                countGrade = "A";
            return countGrade;
        }

        #endregion
    }
}
