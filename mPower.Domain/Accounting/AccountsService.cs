using System;
using System.Collections.Generic;
using System.Linq;
using Paralect.Domain;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Ledger.Commands;
using mPower.Domain.Accounting.Ledger.Data;
using mPower.Domain.Accounting.Transaction.Commands;
using mPower.Framework.Environment;
using ExpandedEntryData = mPower.Domain.Accounting.Transaction.Data.ExpandedEntryData;

namespace mPower.Domain.Accounting
{
    public class AccountsService
    {
        private readonly IIdGenerator _idGenerator;

        public static IEnumerable<AccountLabelEnum> PersonalAccountLabelsList
        {
            get
            {
                //yield return AccountLabelEnum.Bank;
                yield return AccountLabelEnum.CreditCard;
                //yield return AccountLabelEnum.LongTermLiability;
                yield return AccountLabelEnum.Loan;
                //yield return AccountLabelEnum.Investment;
            }
        }

        public AccountsService(IIdGenerator generator)
        {
            _idGenerator = generator;
        }

        public IEnumerable<ICommand> CreatePersonalBaseAccounts(string ledgerId)
        {
            yield return new Ledger_Account_CreateCommand
            {
                AccountId = BaseAccounts.UnCategorizedExpense,
                AccountLabelEnum = AccountLabelEnum.Expense,
                AccountTypeEnum = AccountTypeEnum.Expense,
                LedgerId = ledgerId,
                Name = "Uncategorized Expense"
            };

            yield return new Ledger_Account_CreateCommand
            {
                AccountId = BaseAccounts.UnCategorizedIncome,
                AccountLabelEnum = AccountLabelEnum.Income,
                AccountTypeEnum = AccountTypeEnum.Income,
                LedgerId = ledgerId,
                Name = "Uncategorized Income"
            };

            
            yield return new Ledger_Account_CreateCommand
            {
                AccountId = BaseAccounts.UnknownCash,
                AccountLabelEnum = AccountLabelEnum.Bank,
                AccountTypeEnum = AccountTypeEnum.Asset,
                LedgerId = ledgerId,
                Name = "Unknown Cash"
            };

            yield return new Ledger_Account_CreateCommand
            {
                AccountId = BaseAccounts.Payments,
                AccountLabelEnum = AccountLabelEnum.Equity,
                AccountTypeEnum = AccountTypeEnum.Equity,
                LedgerId = ledgerId,
                Name = "Payments"
            };

            yield return new Ledger_Account_CreateCommand
            {
                AccountId = BaseAccounts.Transfers,
                AccountLabelEnum = AccountLabelEnum.Equity,
                AccountTypeEnum = AccountTypeEnum.Equity,
                LedgerId = ledgerId,
                Name = "Transfers"
            };

            //Move Unknown Cash to the end of the list of accounts, user may change this order later
            yield return new Ledger_Account_UpdateOrderCommand
            {
                LedgerId = ledgerId,
                Orders = new List<AccountOrderData> { new AccountOrderData { Id = BaseAccounts.UnknownCash, Order = 1 } }
            };

            yield return new Ledger_Account_CreateCommand
            {
                AccountId = BaseAccounts.Interest,
                AccountLabelEnum = AccountLabelEnum.OtherExpense,
                AccountTypeEnum = AccountTypeEnum.Expense,
                LedgerId = ledgerId,
                Name = "Interest"
            };

            yield return new Ledger_Account_CreateCommand
            {
                AccountId = BaseAccounts.OpeningBalanceEquity,
                AccountLabelEnum = AccountLabelEnum.Equity,
                AccountTypeEnum = AccountTypeEnum.Equity,
                LedgerId = ledgerId,
                Name = "Opening Balance Equity"
            };

            yield return new Ledger_Account_CreateCommand
            {
                AccountId = BaseAccounts.Salary,
                AccountLabelEnum = AccountLabelEnum.Income,
                AccountTypeEnum = AccountTypeEnum.Income,
                LedgerId = ledgerId,
                Name = "Paycheck/Salary",
                IntuitCategoriesNames = new List<string> {"Paycheck"},
            };
        }

        public IEnumerable<ICommand> CreateBusinessBaseAccounts(string ledgerId)
        {
            yield return new Ledger_Account_CreateCommand
            {
                AccountId = BaseAccounts.OwnerContribution,
                AccountLabelEnum = AccountLabelEnum.Equity,
                AccountTypeEnum = AccountTypeEnum.Equity,
                LedgerId = ledgerId,
                Name = "Owner Contribution"
            };

            yield return new Ledger_Account_CreateCommand
            {
                AccountId = BaseAccounts.OwnerDistribution,
                AccountLabelEnum = AccountLabelEnum.Equity,
                AccountTypeEnum = AccountTypeEnum.Equity,
                LedgerId = ledgerId,
                Name = "Owner Distribution"
            };

            yield return new Ledger_Account_CreateCommand
            {
                AccountId = BaseAccounts.RetainedEarnings,
                AccountLabelEnum = AccountLabelEnum.Equity,
                AccountTypeEnum = AccountTypeEnum.Equity,
                LedgerId = ledgerId,
                Name = "Retained Earnings"
            };

            yield return new Ledger_Account_CreateCommand
            {
                AccountId = BaseAccounts.AccountsReceivable,
                AccountLabelEnum = AccountLabelEnum.AccountsReceivable,
                AccountTypeEnum = AccountTypeEnum.Asset,
                LedgerId = ledgerId,
                Name = "Accounts Receivable"
            };

            yield return new Ledger_Account_CreateCommand
            {
                AccountId = BaseAccounts.AccountsPayable,
                AccountLabelEnum = AccountLabelEnum.AccountsPayable,
                AccountTypeEnum = AccountTypeEnum.Liability,
                LedgerId = ledgerId,
                Name = "Accounts Payable"
            };

            yield return new Ledger_Account_CreateCommand
            {
                AccountId = BaseAccounts.UnCategorizedExpense,
                AccountLabelEnum = AccountLabelEnum.Expense,
                AccountTypeEnum = AccountTypeEnum.Expense,
                LedgerId = ledgerId,
                Name = "Uncategorized Expense"
            };

            yield return new Ledger_Account_CreateCommand
            {
                AccountId = BaseAccounts.UnCategorizedIncome,
                AccountLabelEnum = AccountLabelEnum.Income,
                AccountTypeEnum = AccountTypeEnum.Income,
                LedgerId = ledgerId,
                Name = "Uncategorized Income"
            };

            yield return new Ledger_Account_CreateCommand
            {
                AccountId = BaseAccounts.UnknownCash,
                AccountLabelEnum = AccountLabelEnum.Bank,
                AccountTypeEnum = AccountTypeEnum.Asset,
                LedgerId = ledgerId,
                Name = "Unknown Cash"
            };

            //Move Unknown Cash to the end of the list of accounts, user may change this order later
            yield return new Ledger_Account_UpdateOrderCommand
            {
                LedgerId = ledgerId,
                Orders = new List<AccountOrderData> { new AccountOrderData { Id = BaseAccounts.UnknownCash, Order = 1 } }
            };

            yield return new Ledger_Account_CreateCommand
            {
                AccountId = BaseAccounts.Interest,
                AccountLabelEnum = AccountLabelEnum.OtherExpense,
                AccountTypeEnum = AccountTypeEnum.Expense,
                LedgerId = ledgerId,
                Name = "Interest"
            };

        }

        public IEnumerable<Ledger_Account_CreateCommand> CreateBusinessCommonAccounts(string ledgerId)
        {
            return CommonBusinessExpenseAccounts().Select(account => new Ledger_Account_CreateCommand
            {
                AccountId = _idGenerator.Generate(),
                AccountLabelEnum = AccountLabelEnum.Expense,
                AccountTypeEnum = AccountTypeEnum.Expense,
                LedgerId = ledgerId,
                Name = account
            });
        }

        public IEnumerable<Ledger_Account_CreateCommand> CreatePersonalCommonAccounts(string ledgerId)
        {
            var cmds = new List<Ledger_Account_CreateCommand>();

            foreach (var account in CommonPersonalExpenseAccounts())
            {
                cmds.AddRange(account.GetCreateAccountCommands(_idGenerator, ledgerId));
            }

            return cmds.AsEnumerable();

        }

        public static IEnumerable<string> CommonBusinessExpenseAccounts()
        {
            yield return "Advertising";
            yield return "Bank Fees";
            yield return "Commissions";
            yield return "Dues & Subscription";
            yield return "Insurance";
            yield return "Licenses & Permits";
            yield return "Legal & Professional";
            yield return "Merchant Fees";
            yield return "Office Expense";
            yield return "Outside Services";
            yield return "Rent";
            yield return "Salaries & Wages";
            yield return "Supplies";
            yield return "Taxes";
            yield return "Tools";
            yield return "Telephone";
            yield return "Utilities";
        }

        public IEnumerable<RootExpenseAccount> CommonPersonalExpenseAccounts()
        {
            yield return new RootExpenseAccount("Auto & Transport", "Auto") {SubAccounts = new List<ExpenseAccount>
            {
                new ExpenseAccount("Auto Insurance"),
                new ExpenseAccount("Fuel", "Gas"),
                new ExpenseAccount("Parking"),
                new ExpenseAccount("Public Transportation"),
                new ExpenseAccount("Service & Parts"),
            }};
            yield return new RootExpenseAccount("Bills & Utilities", "Utilities", new List<string> {"Home Phone", "Mobile Phone", "Television", "Utilities"});
            yield return new RootExpenseAccount("Business");
            yield return new RootExpenseAccount("Credit Card");
            yield return new RootExpenseAccount("Education", subAccountsNames: new List<string> {"Books & Supplies", "Tuition"});
            yield return new RootExpenseAccount("Entertainment", subAccountsNames: new List<string> {"Amusement", "Arts", "Movies", "Music", "Newspapers, Magazines"});
            yield return new RootExpenseAccount("Fees & Charges", new List<string> {"Misc. Expense", "Taxi"}, new List<string> {"ATM Fees", "Bank Fees", "Finance Charges", "Late Fees", "Service Fees"});
            yield return new RootExpenseAccount("Financial", subAccountsNames: new List<string> {"Financial Advisor", "Life Insurance"});
            yield return new RootExpenseAccount("Food & Dining", "Dining", new List<string> { "Alcohol & Bars", "Coffee Shops", "Fast Food", "Groceries", "Restaurants"});
            yield return new RootExpenseAccount("Gifts & Donations") {SubAccounts = new List<ExpenseAccount>
            {
                new ExpenseAccount("Charity", "Charity/Donations"),
                new ExpenseAccount("Gifts"),
            }};
            yield return new RootExpenseAccount("Health & Fitness", subAccountsNames: new List<string> {"Dentist", "Doctor", "Eyecare", "Gym", "Health Insurance", "Pharmacy", "Sports"});
            yield return new RootExpenseAccount("Home Improvement", "Home Repair", new List<string> { "Furnishings", "Home Insurance", "Home Services", "Home Supplies", "Lawn & Garden", "Rent" });
            yield return new RootExpenseAccount("Insurance");
            yield return new RootExpenseAccount("Kids", "My Kids", new List<string> {"Allowance", "Baby Supplies", "Baby Sitter", "Daycare", "Child Support", "Kids Activites", "Toys"});
            yield return new RootExpenseAccount("Mortgage", "Household");
            yield return new RootExpenseAccount("Personal Care", "Medical", new List<string> {"Hair", "Laundry", "Spa & Massage"});
            yield return new RootExpenseAccount("Pets", "My Pets", new List<string> {"Pet Food & Supplies", "Pet Grooming", "Veterinary"});
            yield return new RootExpenseAccount("Shopping", subAccountsNames: new List<string> {"Books", "Clothing", "Hobbies", "Sporting Goods"});
            yield return new RootExpenseAccount("Taxes", subAccountsNames: new List<string> {"Federal Tax", "Local Tax", "Property Tax", "Sales Tax", "State Tax"});
            yield return new RootExpenseAccount("Travel", "Vacation/Travel", new List<string> {"Air Travel", "Hotel", "Rental Car & Travel", "Vacation"});
        }

        public IEnumerable<Ledger_Account_CreateCommand> CreateProductBasedBusinessAccounts(string ledgerId)
        {

            yield return new Ledger_Account_CreateCommand
            {
                AccountId = _idGenerator.Generate(),
                AccountLabelEnum = AccountLabelEnum.Income,
                AccountTypeEnum = AccountTypeEnum.Income,
                LedgerId = ledgerId,
                Name = "Sales"
            };

            yield return new Ledger_Account_CreateCommand
            {
                AccountId = _idGenerator.Generate(),
                AccountLabelEnum = AccountLabelEnum.CostOfGoodsSold,
                AccountTypeEnum = AccountTypeEnum.Expense,
                LedgerId = ledgerId,
                Name = "Cost Of Goods Sold"
            };

            yield return new Ledger_Account_CreateCommand
            {
                AccountId = _idGenerator.Generate(),
                AccountLabelEnum = AccountLabelEnum.CostOfGoodsSold,
                AccountTypeEnum = AccountTypeEnum.Expense,
                LedgerId = ledgerId,
                Name = "Returns"
            };

            yield return new Ledger_Account_CreateCommand
            {
                AccountId = _idGenerator.Generate(),
                AccountLabelEnum = AccountLabelEnum.Income,
                AccountTypeEnum = AccountTypeEnum.Income,
                LedgerId = ledgerId,
                Name = "Interest Income"
            };
        }

        public IEnumerable<Ledger_Account_CreateCommand> CreateServiceBasedBusinessAccounts(string ledgerId)
        {
            yield return new Ledger_Account_CreateCommand
            {
                AccountId = _idGenerator.Generate(),
                AccountLabelEnum = AccountLabelEnum.Income,
                AccountTypeEnum = AccountTypeEnum.Income,
                LedgerId = ledgerId,
                Name = "Sales"
            };

            yield return new Ledger_Account_CreateCommand
            {
                AccountId = _idGenerator.Generate(),
                AccountLabelEnum = AccountLabelEnum.Income,
                AccountTypeEnum = AccountTypeEnum.Income,
                LedgerId = ledgerId,
                Name = "Interest Income"
            };
        }

        public IEnumerable<ICommand> CreateAccountCommands(string name, string userId, string ledgerId, AccountLabelEnum label, string parentAccountId, string description, string number, float interestRatePercentage, decimal minMonthlyPaymentInDollars, decimal creditLimitInDollars, decimal openingBalance)
        {
            var cmd = new Ledger_Account_CreateCommand
            {
                AccountId = _idGenerator.Generate(),
                Name = name,
                LedgerId = ledgerId,
                AccountLabelEnum = label,
                AccountTypeEnum = AccountingFormatter.AccountLabelToType(label),
                ParentAccountId = parentAccountId,
                Aggregated = false,
                Description = description,
                Imported = false,
                Number = number,
                InterestRatePerc = interestRatePercentage,
                MinMonthPaymentInCents = AccountingFormatter.DollarsToCents(minMonthlyPaymentInDollars),
                CreditLimitInCents = AccountingFormatter.DollarsToCents(creditLimitInDollars),
            };

            yield return cmd;

            if (openingBalance > 0)
            {
                var openingBalanceInCents = AccountingFormatter.DollarsToCents(openingBalance);

                TransactionType transactionType;

                switch (cmd.AccountLabelEnum)
                {
                    case AccountLabelEnum.Bank:
                    case AccountLabelEnum.Investment:
                        transactionType = TransactionType.Deposit;
                        break;

                    case AccountLabelEnum.CreditCard:
                        transactionType = TransactionType.CreditCard;
                        break;

                    case AccountLabelEnum.Loan:
                        transactionType = TransactionType.Check;
                        break;

                    default:
                        transactionType = TransactionType.Deposit;
                        break;
                }

                var transactionId = _idGenerator.Generate();
                var command = new Transaction_CreateCommand
                {
                    TransactionId = transactionId,
                    Type = transactionType,
                    UserId = userId,
                    LedgerId = cmd.LedgerId,
                    BaseEntryAccountId = cmd.AccountId,
                    BaseEntryAccountType = cmd.AccountTypeEnum,
                    Entries = new List<ExpandedEntryData>
                    {
                        new ExpandedEntryData
                        {
                            AccountId = cmd.AccountId,
                            BookedDate = DateTime.Now.AddDays(-1),
                            CreditAmountInCents = AccountingFormatter.CreditAmount(openingBalanceInCents, cmd.AccountTypeEnum),
                            DebitAmountInCents = AccountingFormatter.DebitAmount(openingBalanceInCents, cmd.AccountTypeEnum),
                            Memo = "Beginning Balance Adjustment",
                            LedgerId = cmd.LedgerId,
                            TransactionId = transactionId,
                            AccountType = cmd.AccountTypeEnum,
                            AccountLabel = cmd.AccountLabelEnum,
                            AccountName = cmd.Name,
                            OffsetAccountId = BaseAccounts.OpeningBalanceEquity,
                            OffsetAccountName = BaseAccounts.OpeningBalanceEquity.GetDescription(),
                            TransactionImported = false
                        },
                        new ExpandedEntryData
                        {
                            AccountId = BaseAccounts.OpeningBalanceEquity,
                            BookedDate = DateTime.Now.AddDays(-1),
                            CreditAmountInCents = AccountingFormatter.DebitAmount(openingBalanceInCents, cmd.AccountTypeEnum),
                            DebitAmountInCents = AccountingFormatter.CreditAmount(openingBalanceInCents, cmd.AccountTypeEnum),
                            Memo = "Beginning Balance Adjustment",
                            LedgerId = cmd.LedgerId,
                            TransactionId = transactionId,
                            AccountType = AccountTypeEnum.Equity,
                            AccountLabel = AccountLabelEnum.Equity,
                            AccountName = BaseAccounts.OpeningBalanceEquity.GetDescription(),
                            OffsetAccountId = cmd.AccountId,
                            OffsetAccountName = cmd.Name,
                            TransactionImported = false
                        }
                    }
                };

                yield return command;
            }
        }

        public IEnumerable<ICommand> SetupPersonalLedger(string userId, string ledgerId)
        {

            var ledgerCreateCommand = new Ledger_CreateCommand
            {
                Name = "Personal",
                CreatedDate = DateTime.Now,
                TypeEnum = LedgerTypeEnum.Personal,
                LedgerId = ledgerId,
                FiscalYearStart = new DateTime(DateTime.Now.Year, 1, 1)
            };

            var ledgerUserAddCommand = new Ledger_User_AddCommand
            {
                LedgerId = ledgerCreateCommand.LedgerId,
                UserId = userId
            };

            yield return ledgerCreateCommand;
            yield return ledgerUserAddCommand;

            var accountCommands = CreatePersonalBaseAccounts(ledgerCreateCommand.LedgerId).ToList();
            accountCommands.AddRange(CreatePersonalCommonAccounts(ledgerCreateCommand.LedgerId).ToList());

            foreach (var acct in accountCommands)
            {
                yield return acct;
            }

        }
    }

    public class ExpenseAccount
    {
        public string Name { get; private set; }
        public List<string> IntuitCategoriesNames { get; private set; }

        public ExpenseAccount(string name, string intuitCategoryName = null)
            : this(name, new List<string>())
        {
            if (!string.IsNullOrEmpty(intuitCategoryName))
            {
                IntuitCategoriesNames.Add(intuitCategoryName);
            }
        }

        public ExpenseAccount(string name, List<string> intuitCategoriesNames)
        {
            Name = name;
            IntuitCategoriesNames = intuitCategoriesNames ?? new List<string>();
        }
    }

    public class RootExpenseAccount : ExpenseAccount
    {
        public List<ExpenseAccount> SubAccounts { get; set; }

        public RootExpenseAccount(string name, string intuitCategoryName = null, List<string> subAccountsNames = null)
            : base(name, intuitCategoryName)
        {
            Initialize(subAccountsNames);
        }

        public RootExpenseAccount(string name, List<string> intuitCategoriesNames, List<string> subAccountsNames = null)
            : base(name, intuitCategoriesNames)
        {
            Initialize(subAccountsNames);
        }

        private void Initialize(List<string> subAccountsNames)
        {
            SubAccounts = (subAccountsNames ?? new List<string>()).Select(x => new ExpenseAccount(x)).ToList();
        }

        public IEnumerable<Ledger_Account_CreateCommand> GetCreateAccountCommands(IIdGenerator idGenerator, string ledgerId)
        {
            var parentAccount = new Ledger_Account_CreateCommand
            {
                AccountId = idGenerator.Generate(),
                AccountLabelEnum = AccountLabelEnum.Expense,
                AccountTypeEnum = AccountTypeEnum.Expense,
                LedgerId = ledgerId,
                Name = Name,
                IntuitCategoriesNames = IntuitCategoriesNames,
            };

            var subAccounts = SubAccounts.Select(account => new Ledger_Account_CreateCommand
            {
                AccountId = idGenerator.Generate(),
                AccountLabelEnum = AccountLabelEnum.Expense,
                AccountTypeEnum = AccountTypeEnum.Expense,
                LedgerId = ledgerId,
                Name = account.Name,
                IntuitCategoriesNames = account.IntuitCategoriesNames,
                ParentAccountId = parentAccount.AccountId
            });



            yield return parentAccount;

            foreach (var ledgerAccountCreateCommand in subAccounts)
            {
                yield return ledgerAccountCreateCommand;
            }



        }
    }
}
