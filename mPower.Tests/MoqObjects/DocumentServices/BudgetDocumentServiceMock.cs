using System.Collections.Generic;
using System.Linq;
using Moq;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.DocumentServices.Accounting.Filters;
using mPower.Domain.Accounting.Enums;
using mPower.Tests.Environment;
using mPower.Tests.MoqObjects.Mongodb;

namespace mPower.Tests.MoqObjects.DocumentServices
{
    public class BudgetDocumentServiceMock : IMock<BudgetDocumentService>
    {
        private readonly MockFactory _mockFactory;
        private Mock<BudgetDocumentService> _current;

        public BudgetDocumentServiceMock(MockFactory mockFactory)
        {
            _mockFactory = mockFactory;
            Create();
        }

        public Mock<BudgetDocumentService> Create()
        {
            var mongoreadMock = _mockFactory.Create<MongoReadMock>();

            _current = new Mock<BudgetDocumentService>(mongoreadMock.Object);

            return _current;
        }

        public BudgetDocumentService Object
        {
            get { return _current.Object; }
        }

        public BudgetDocumentServiceMock AddGetByFilterWithSubBudgets()
        {
            _current.Setup(x => x.GetByFilter(It.IsAny<BudgetFilter>())).Returns(BudgetsWithSubBudgets);

            return this;
        }

        public BudgetDocumentServiceMock AddGetLedgetBudgetsByMonthAndYear()
        {
            _current.Setup(x => x.GetLedgetBudgetsByMonthAndYear(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())).Returns(
                (int month, int year, string ledgerId) => BudgetsWithSubBudgets.Where(x=>x.LedgerId == ledgerId && x.Year == year && x.Month == month).ToList());

            return this;
        }

        public List<BudgetDocument> BudgetsWithSubBudgets
        {
            get
            {
                return new List<BudgetDocument>
                {
                    new BudgetDocument
                    {
                        AccountId = "5",
                        AccountName = "5",
                        AccountType = AccountTypeEnum.Expense,
                        BudgetAmount = 100,
                        LedgerId = "1",
                        Id = "1",
                        Year = 2012,
                        Month = 1,
                        SpentAmount = 20,
                        SubBudgets = new List<ChildBudgetDocument>
                        {
                            new ChildBudgetDocument
                            {
                                AccountId = "51",
                                AccountName = "51",
                                AccountType = AccountTypeEnum.Expense,
                                SpentAmount = 20,
                                ParentAccountId = "5"
                            },
                            new ChildBudgetDocument
                            {
                                AccountId = "52",
                                AccountName = "52",
                                AccountType = AccountTypeEnum.Expense,
                                SpentAmount = 33,
                                ParentAccountId = "5"
                            },
                        }
                    },
                    new BudgetDocument
                    {
                        AccountId = "6",
                        LedgerId = "1",
                        Id = "2",
                        AccountName = "6",
                        AccountType = AccountTypeEnum.Income,
                        Year = 2012,
                        Month = 1,
                        BudgetAmount = 150,
                        SpentAmount = 33
                    }
                };
            }
        }
    }
}
