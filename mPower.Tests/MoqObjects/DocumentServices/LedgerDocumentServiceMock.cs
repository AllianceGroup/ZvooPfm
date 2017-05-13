using System;
using System.Collections.Generic;
using Moq;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.DocumentServices.Accounting.Filters;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Domain.Accounting.Enums;
using mPower.Tests.Environment;
using mPower.Tests.MoqObjects.Mongodb;

namespace mPower.Tests.MoqObjects.DocumentServices
{
    public class LedgerDocumentServiceMock : IMock<LedgerDocumentService>
    {
        private readonly MockFactory _mockFactory;
        private Mock<LedgerDocumentService> _current;

        public LedgerDocumentServiceMock(MockFactory mockFactory)
        {
            _mockFactory = mockFactory;
            _current = Create();
        }

        public LedgerDocumentService Object
        {
            get { return _current.Object; }
        }

        public Mock<LedgerDocumentService> Create()
        {
            var mongoread = _mockFactory.Create<MongoReadMock>();

            return new Mock<LedgerDocumentService>(mongoread.Object);
        }

        public LedgerDocumentServiceMock AddFindOne()
        {
            _current.Setup(x => x.FindOne()).Returns(FakeLedger);

            return this;
        }

        public LedgerDocumentServiceMock AddGetById()
        {
            _current.Setup(x => x.GetById("1")).Returns(FakeLedger);

            return this;
        }

        private LedgerDocument _ledger;

        public LedgerDocument FakeLedger
        {
            get
            {
                if (_ledger == null)
                {

                    var accounts = new List<AccountDocument>();

                    accounts.Add(new AccountDocument()
                    {
                        Id = "1",
                        Denormalized = new AccountDocument.DenormalizedData() { Balance = 1 },
                        Name = "1",
                        Description = "1",
                        ParentAccountId = null,
                        Number = "1",
                        LabelEnum = AccountLabelEnum.Bank,
                        TypeEnum = AccountTypeEnum.Asset
                    });

                    accounts.Add(new AccountDocument()
                    {
                        Id = "2",
                        Denormalized = new AccountDocument.DenormalizedData() { Balance = 2 },
                        Name = "2",
                        Description = "2",
                        ParentAccountId = null,
                        Number = "2",
                        LabelEnum = AccountLabelEnum.OtherCurrentLiability,
                        TypeEnum = AccountTypeEnum.Liability
                    });

                    accounts.Add(new AccountDocument()
                    {
                        Id = "3",
                        Denormalized = new AccountDocument.DenormalizedData() { Balance = 3 },
                        Name = "3",
                        Description = "3",
                        ParentAccountId = null,
                        Number = "3",
                        LabelEnum = AccountLabelEnum.CreditCard,
                        TypeEnum = AccountTypeEnum.Liability
                    });

                    accounts.Add(new AccountDocument()
                    {
                        Id = "4",
                        Denormalized = new AccountDocument.DenormalizedData() { Balance = 4 },
                        Name = "4",
                        Description = "4",
                        ParentAccountId = null,
                        Number = "4",
                        LabelEnum = AccountLabelEnum.FixedAsset,
                        TypeEnum = AccountTypeEnum.Asset
                    });

                    accounts.Add(new AccountDocument()
                    {
                        Id = "5",
                        Denormalized = new AccountDocument.DenormalizedData() { Balance = 5 },
                        Name = "5",
                        Description = "5",
                        ParentAccountId = null,
                        Number = "5",
                        LabelEnum = AccountLabelEnum.Expense,
                        TypeEnum = AccountTypeEnum.Expense
                    });

                    accounts.Add(new AccountDocument()
                    {
                        Id = "51",
                        Denormalized = new AccountDocument.DenormalizedData() { Balance = 51 },
                        Name = "51",
                        Description = "51",
                        ParentAccountId = "5",
                        Number = "51",
                        LabelEnum = AccountLabelEnum.Expense,
                        TypeEnum = AccountTypeEnum.Expense
                    });

                    accounts.Add(new AccountDocument()
                    {
                        Id = "52",
                        Denormalized = new AccountDocument.DenormalizedData() { Balance = 52 },
                        Name = "52",
                        Description = "52",
                        ParentAccountId = "5",
                        Number = "52",
                        LabelEnum = AccountLabelEnum.Expense,
                        TypeEnum = AccountTypeEnum.Expense
                    });


                    accounts.Add(new AccountDocument()
                    {
                        Id = "6",
                        Denormalized = new AccountDocument.DenormalizedData() { Balance = 6 },
                        Name = "6",
                        Description = "6",
                        ParentAccountId = null,
                        Number = "6",
                        LabelEnum = AccountLabelEnum.Income,
                        TypeEnum = AccountTypeEnum.Income
                    });

                    accounts.Add(new AccountDocument()
                    {
                        Id = BaseAccounts.UnknownCash,
                        Denormalized = new AccountDocument.DenormalizedData() { Balance = 7 },
                        Name = "Unknown Cash",
                        Description = "7",
                        ParentAccountId = null,
                        Number = "7",
                        LabelEnum = AccountLabelEnum.Income,
                        TypeEnum = AccountTypeEnum.Income
                    });

                    accounts.Add(new AccountDocument()
                    {
                        Id = "8",
                        Denormalized = new AccountDocument.DenormalizedData() { Balance = 8 },
                        Name = "Loan",
                        Description = "",
                        ParentAccountId = null,
                        Number = "8",
                        LabelEnum = AccountLabelEnum.Loan,
                        TypeEnum = AccountTypeEnum.Liability
                    });

                    var ld = new LedgerDocument()
                    {
                        Id = "1",
                        Name = "Sample Company",
                        Accounts = accounts,
                        Users = new List<LedgerUserDocument>() { new LedgerUserDocument() { Id = "1" } }

                    };

                    _ledger = ld;
                }


                return _ledger;
            }
        }
    }
}
