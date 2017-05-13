using System;
using System.Collections.Generic;
using Moq;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Documents.DocumentServices.Accounting.Reports;
using mPower.Domain.Accounting.Enums;
using mPower.Tests.Environment;

namespace mPower.Tests.MoqObjects.DocumentServices
{
    public class BusinessReportDocumentServiceMock : IMock<BusinessReportDocumentService>
    {
        private readonly MockFactory _mockFactory;
        private readonly Mock<BusinessReportDocumentService> _current;

        public BusinessReportDocumentServiceMock(MockFactory mockFactory)
        {
            _mockFactory = mockFactory;
            _current = Create();
        }

        public BusinessReportDocumentService Object
        {
            get { return _current.Object; }
        }

        public Mock<BusinessReportDocumentService> Create()
        {
            var entryService = _mockFactory.Create<EntryDocumentServiceMock>();

            return new Mock<BusinessReportDocumentService>(entryService.Object);
        }

        public BusinessReportDocumentServiceMock AddGetProfitLossReportData()
        {
            _current.Setup(x => x.GetProfitLossReportData(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<LedgerDocument>())).Returns(FakeProfitLossReportData);

            return this;
        }

        public List<LedgerAccountBalanceByDay> FakeProfitLossReportData
        {
            get
            {
                return new List<LedgerAccountBalanceByDay>
                {
                    new LedgerAccountBalanceByDay
                    {
                        AccountId = "5",
                        Name = "5",
                        AccountType = AccountTypeEnum.Expense,
                        AmountPerDay = new List<DateAmount>
                        {
                            new DateAmount {Date = new DateTime(2012, 1, 3), Amount = 15},
                            new DateAmount {Date = new DateTime(2012, 1, 20), Amount = 5},
                        },
                        SubAccounts = new List<LedgerAccountBalanceByDay>
                        {
                            new LedgerAccountBalanceByDay
                            {
                                AccountId = "51",
                                Name = "51",
                                AccountType = AccountTypeEnum.Expense,
                                AmountPerDay = new List<DateAmount>
                                {
                                    new DateAmount {Date = new DateTime(2012, 1, 10), Amount = 10},
                                    new DateAmount {Date = new DateTime(2012, 1, 19), Amount = 10},
                                },
                            },
                            new LedgerAccountBalanceByDay
                            {
                                AccountId = "52",
                                Name = "52",
                                AccountType = AccountTypeEnum.Expense,
                                AmountPerDay = new List<DateAmount>
                                {
                                    new DateAmount {Date = new DateTime(2012, 1, 7), Amount = 13},
                                    new DateAmount {Date = new DateTime(2012, 1, 27), Amount = 20},
                                },
                            },
                        },
                    },
                    new LedgerAccountBalanceByDay
                    {
                        AccountId = "6",
                        Name = "6",
                        AccountType = AccountTypeEnum.Income,
                        AmountPerDay = new List<DateAmount>
                        {
                            new DateAmount {Date = new DateTime(2012, 1, 15), Amount = 15},
                            new DateAmount {Date = new DateTime(2012, 1, 22), Amount = 18},
                        },
                    },
                };
            }
        }
    }
}