using System.Collections.Generic;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Domain.Accounting.Ledger.Commands;
using mPower.Domain.Accounting.Ledger.Events;
using NUnit.Framework;
using Paralect.Domain;

namespace mPower.Tests.UnitTests.Domain.Accounting.Ledger.UserTests
{
    public class ledger_user_remove_test : LedgerTest
    {
        public override IEnumerable<IEvent> Given()
        {
            yield return Ledger_Created();
            yield return new Ledger_User_AddedEvent() { LedgerId = _id, UserId = "1" };
        }

        public override IEnumerable<ICommand> When()
        {
            yield return new Ledger_User_RemoveCommand() { LedgerId = _id, UserId = "1" };
        }

        public override IEnumerable<IEvent> Expected()
        {
            yield return new Ledger_User_RemovedEvent() { LedgerId = _id, UserId = "1" };
        }

        [Test]
        public void Test()
        {
            Validate();

            DispatchEvents(() =>
            {
                var _lederDocumentService = GetInstance<LedgerDocumentService>();

                var ledgerDoc = _lederDocumentService.GetById(_id);
                Assert.AreEqual(ledgerDoc.Users.Count, 0);

                // mobile API asserts
                //var _mobileLedgerDocumentService = GetInstance<MobileLedgerDocumentService>();
                //var mobileLedgerDoc = _mobileLedgerDocumentService.GetById(_id);

                //Assert.AreEqual(0, mobileLedgerDoc.Users.Count);
            });
        }
    }
}