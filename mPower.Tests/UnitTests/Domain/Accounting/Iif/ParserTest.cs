using System.IO;
using NUnit.Framework;
using mPower.Documents.IifHelpers;
using mPower.Tests.Environment;

namespace mPower.Tests.UnitTests.Domain.Accounting.Iif
{
    public class ParserTest : IifTest
    {
        [Test]
        public void Test()
        {
            Assert.IsTrue(ObjectComparer.AreObjectsEqual(ParsingResult, IifParser.ParseIifString(File.OpenRead(TestFilePath))));
        }
    }
}
