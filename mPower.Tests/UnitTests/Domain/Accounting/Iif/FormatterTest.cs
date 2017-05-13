using System.IO;
using NUnit.Framework;
using mPower.Documents.IifHelpers;

namespace mPower.Tests.UnitTests.Domain.Accounting.Iif
{
    [TestFixture]
    public class FormatterTest : IifTest
    {
        [Test]
        [Ignore]
        public void CorrectDataTest()
        {
            string expectedResult;
            using (var reader = new StreamReader(TestFilePath))
            {
                expectedResult = reader.ReadToEnd();
                reader.Close();
            }

            Assert.AreEqual(expectedResult, IifFormatter.LedgerToIifString(Ledger, Transactions));
        }
    }
}
