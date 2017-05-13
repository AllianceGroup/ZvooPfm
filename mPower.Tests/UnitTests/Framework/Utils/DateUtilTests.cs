using System;
using NUnit.Framework;
using mPower.Framework.Utils;

namespace mPower.Tests.UnitTests.Framework.Utils
{
    public class DateUtilTests
    {
        [Test]
        public void CalcPrevQuarterByBorderValue()
        {
            var fiscalYearStart = DateTime.Today;
            var fiscalQuarter = new FiscalQuarter(fiscalYearStart);
            var prevQuarterStart = fiscalQuarter.GetStartOfLastQuarter();
            var prevQuarterEnd = fiscalQuarter.GetEndOfLastQuarter();

            var expectedPrevQuarterStart = fiscalYearStart.AddMonths(-3);
            var expectedPrevQuarterEnd = fiscalYearStart.AddMilliseconds(-1);

            Assert.AreEqual(expectedPrevQuarterStart.Year, prevQuarterStart.Year);
            Assert.AreEqual(expectedPrevQuarterStart.Month, prevQuarterStart.Month);
            Assert.AreEqual(expectedPrevQuarterStart.Day, prevQuarterStart.Day);

            Assert.AreEqual(expectedPrevQuarterEnd.Year, prevQuarterEnd.Year);
            Assert.AreEqual(expectedPrevQuarterEnd.Month, prevQuarterEnd.Month);
            Assert.AreEqual(expectedPrevQuarterEnd.Day, prevQuarterEnd.Day);
        }
    }
}