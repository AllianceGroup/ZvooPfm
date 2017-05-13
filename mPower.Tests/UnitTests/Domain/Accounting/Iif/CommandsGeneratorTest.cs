using mPower.Domain.Accounting.Ledger.Commands;
using mPower.Domain.Accounting.Transaction.Commands;
using NUnit.Framework;
using mPower.Tests.Environment;

namespace mPower.Tests.UnitTests.Domain.Accounting.Iif
{
    public class CommandsGeneratorTest : IifTest
    {
        [Test]
        public void Test()
        {
            var commands = CommandsGenerator.Generate(Ledger.Id, ParsingResult);

            // can't compare Ids
            Assert.AreEqual(ExpectedCommands.Count, commands.Count);

            for (var i = 0; i < commands.Count; i++)
            {
                var cmd = commands[i];
                if (cmd is Ledger_Account_CreateCommand)
                {
                    var expectedTransCreateCmd = (Ledger_Account_CreateCommand)ExpectedCommands[i];
                    var actualTransCreateCmd = (Ledger_Account_CreateCommand)cmd;
                    Assert.AreEqual(expectedTransCreateCmd.LedgerId, actualTransCreateCmd.LedgerId);
                    Assert.AreEqual(expectedTransCreateCmd.Name, actualTransCreateCmd.Name);
                    Assert.AreEqual(expectedTransCreateCmd.AccountTypeEnum, actualTransCreateCmd.AccountTypeEnum);
                    Assert.AreEqual(expectedTransCreateCmd.AccountLabelEnum, actualTransCreateCmd.AccountLabelEnum);
                    Assert.AreEqual(expectedTransCreateCmd.Imported, actualTransCreateCmd.Imported);
                }
                else if (cmd is Ledger_Account_UpdateCommand)
                {
                    var expectedTransUpdateCmd = (Ledger_Account_UpdateCommand)ExpectedCommands[i];
                    var actualTransUpdateCmd = (Ledger_Account_UpdateCommand)cmd;
                    Assert.AreEqual(expectedTransUpdateCmd.LedgerId, actualTransUpdateCmd.LedgerId);
                    Assert.AreEqual(expectedTransUpdateCmd.Name, actualTransUpdateCmd.Name);
                    Assert.AreEqual(expectedTransUpdateCmd.Description, actualTransUpdateCmd.Description);
                }
                else if (cmd is Transaction_CreateCommand)
                {
                    var expectedTransCreateCmd = (Transaction_CreateCommand)ExpectedCommands[i];
                    var actualTransCreateCmd = (Transaction_CreateCommand)cmd;
                    Assert.AreEqual(expectedTransCreateCmd.LedgerId, actualTransCreateCmd.LedgerId);
                    Assert.AreEqual(expectedTransCreateCmd.Type, actualTransCreateCmd.Type);
                    //Assert.AreEqual(expectedTransCreateCmd.Entries.Count, actualTransCreateCmd.Entries.Count);
                    Assert.AreEqual(expectedTransCreateCmd.Imported, actualTransCreateCmd.Imported);
                    //for (var j = 0; j < actualTransCreateCmd.Entries.Count; j++)
                    //{
                    //    Assert.IsTrue(ObjectComparer.AreObjectsEqual(expectedTransCreateCmd.Entries[j], actualTransCreateCmd.Entries[j]));
                    //}
                }
                else
                {
                    Assert.IsTrue(ObjectComparer.AreObjectsEqual(ExpectedCommands[i], cmd));
                }
            }
        }
    }
}
