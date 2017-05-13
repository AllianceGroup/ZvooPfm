using System.Collections.Generic;
using NUnit.Framework;
using Paralect.Domain;
using mPower.Documents.DocumentServices.Credit;

namespace mPower.Tests.UnitTests.Domain.Accounting.CreditIdentity.Report
{
    public class AddReport : CreditIdentityTest
    {
        public override IEnumerable<IEvent> Given()
        {
            yield return CreditIdentity_Created();
            yield return CreditIdentity_Questions_Set();

        }

        public override IEnumerable<ICommand> When()
        {
            yield return CreditIdentity_Report_Add();
        }

        public override IEnumerable<IEvent> Expected()
        {
            yield return CreditIdentity_Report_Added();
        }

        [Test]
        [Ignore]
        public void Test()
        {
            Validate();

            DispatchEvents(() =>
            {
                var creditDocumentService = GetInstance<CreditIdentityDocumentService>();

                var creditIdentity = creditDocumentService.GetById(_id);
                Assert.NotNull(creditIdentity.CreditReports);
                Assert.IsTrue(creditIdentity.CreditReports.Count == 1);

            });
        }

    }
}
