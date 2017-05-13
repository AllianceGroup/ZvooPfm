using System.Collections.Generic;
using NUnit.Framework;
using Paralect.Domain;
using mPower.Documents.DocumentServices.Accounting;

namespace mPower.Tests.UnitTests.Domain.Accounting.Ledger
{
    public class ledger_removing_test : LedgerTest
    {
        public override IEnumerable<IEvent> Given()
        {
            yield return Ledger_Created();
        }

        public override IEnumerable<ICommand> When()
        {
            yield return Ledger_Delete();
        }

        public override IEnumerable<IEvent> Expected()
        {
            yield return Ledger_Deleted();
        }

        [Test]
        public void Test()
        {
            Validate();

            DispatchEvents(() =>
            {
                var _lederDocumentService = GetInstance<LedgerDocumentService>();

                var ledgerDoc = _lederDocumentService.GetById(_id);
                Assert.AreEqual(null, ledgerDoc);
            });
        }
    }
}
