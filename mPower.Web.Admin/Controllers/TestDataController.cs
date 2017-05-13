using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MongoDB.Bson.Serialization.Attributes;
using mPower.Documents;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.ExternalServices.FullTextSearch;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.CreditIdentity.Commands;
using mPower.Domain.Accounting.CreditIdentity.Data;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Ledger.Commands;
using mPower.Domain.Accounting.Transaction.Commands;
using mPower.Domain.Accounting.Transaction.Data;
using mPower.Domain.Application.Affiliate.Commands;
using mPower.Domain.Membership.Enums;
using mPower.Domain.Membership.User.Commands;
using mPower.Framework;
using mPower.Framework.Environment;
using mPower.Framework.Mvc;
using mPower.Framework.Services;
using mPower.Framework.Utils;
using mPower.Framework.Utils.Security;
using Paralect.Domain;
using ExpandedEntryData = mPower.Domain.Accounting.Transaction.Data.ExpandedEntryData;

namespace mPower.Web.Admin.Controllers
{
    public class TestDataController : BaseAdminController
    {
        private const string BusinessLedgerId = "1";
        private const string PersonalLedgerId = "2";
        private const string ClientKey = "qwerty-1234-asdf";
        private const string UserId = "5121212212";
        private const float InterestRate = 11f;
        private const long MinMonthPay = 3500; // in cents

        private readonly MongoRead _mongoRead;
        private readonly MongoWrite _mongoWrite;
        private readonly IEncryptionService _encrypter;
        private readonly LedgerDocumentService _ledgerService;
        private readonly IIdGenerator _idGenerator;
        private readonly MongoTemp _mongoTemp;
        private readonly AffiliatesController _affiliatesController;
        private readonly AccountsService _accountsService;
        private readonly TransactionGenerator _transactionGenerator;
        private readonly TransactionLuceneService _transactionLuceneSerivce;
        private readonly IObjectRepository _objectRepository;

        public TestDataController(MongoRead mongoRead, MongoWrite mongoWrite,
            IEncryptionService encrypter,
            LedgerDocumentService ledgerService,
            IIdGenerator idGenerator,
            MongoTemp mongoTemp,
            AffiliatesController affiliatesController,
            AccountsService accountsService,
            TransactionGenerator transactionGenerator,
            TransactionLuceneService transactionLuceneSerivce,
            IObjectRepository objectRepository

            )
        {
            _mongoRead = mongoRead;
            _mongoWrite = mongoWrite;
            _encrypter = encrypter;
            _ledgerService = ledgerService;
            _idGenerator = idGenerator;
            _mongoTemp = mongoTemp;
            _affiliatesController = affiliatesController;
            _accountsService = accountsService;
            _transactionGenerator = transactionGenerator;
            _transactionLuceneSerivce = transactionLuceneSerivce;
            _objectRepository = objectRepository;
        }

        private readonly DateTime _currentDate = DateTime.Now;

        public DateTime CurrentDate
        {
            get { return _currentDate; }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            _affiliatesController.ControllerContext = ControllerContext;
        }

        public ActionResult Search(long amount = 5, string ledgerId = "4ea819f4d7e35b2ae42992cd")
        {
            var entries = _mongoRead.Entries.FindAllAs<EntryDocument>();
            _transactionLuceneSerivce.RemoveIndexFromDisc();
            _transactionLuceneSerivce.Insert(entries.ToArray());
            //PAYPAL INST XFER ACH Ent ry Memo Posted Today

            var results = _transactionLuceneSerivce.SearchByQuery(
                new EntryLuceneFilter
                {
                    MinEntryAmount = 5,
                    MaxEntryAmount = 1000000,
                    LedgerId = ledgerId,
                    AccountId = "4eb8e2788d68fd1abc5196ff",
                    SearchText = "bank",
                    AccountLabels = new List<AccountLabelEnum> { AccountLabelEnum.Bank },
                    PagingInfo = new PagingInfo()
                });

            return new ContentResult { Content = "Done" };
        }

        public ActionResult Index()
        {
            _mongoWrite.Database.Drop();
            _mongoRead.Database.Drop();
            _mongoTemp.Database.Drop();

            CreateAffiliates();
            CreateLedger();
            CreateUsers();

            CreateCreditIdentity();
            SetVerificationQuestions();
            AddCreditReports();
            AddCreditAlerts();

            return new ContentResult {Content = "Done"};
        }

        public class Doc
        {
            [BsonId]
            public string Id { get; set; }

            public DateTime Local { get; set; }

            public DateTime Utc { get; set; }
        }

        public ActionResult GetLedger()
        {
            var ledger = _ledgerService.GetAll().FirstOrDefault();

            return new JsonResult { Data = ledger };
        }

        #region Ledgers

        private void CreateLedger()
        {
            Send(_accountsService.SetupPersonalLedger(UserId, PersonalLedgerId).ToArray());

            var createBusinessLedgerCommand = new Ledger_CreateCommand
            {
                LedgerId = BusinessLedgerId,
                Name = "Sample Company, LLC",
                FiscalYearStart = DateUtil.GetStartOfCurrentYear().AddMonths(6),
                CreatedDate = DateTime.Now.AddYears(-2),
                TypeEnum = LedgerTypeEnum.Business
            };

            Send(createBusinessLedgerCommand);

            var addUserToBusinessLedgerCommand = new Ledger_User_AddCommand
                                             {
                                                 LedgerId = BusinessLedgerId,
                                                 UserId = UserId
                                             };

            Send(addUserToBusinessLedgerCommand);

            var accountCommands = new List<ICommand>();
            accountCommands.AddRange(_accountsService.CreateBusinessBaseAccounts(BusinessLedgerId).ToList());
            accountCommands.AddRange(_accountsService.CreateProductBasedBusinessAccounts(BusinessLedgerId));

            Send(accountCommands.ToArray());

            //Income
            Ledger_Add_Account("Sales", AccountTypeEnum.Income, AccountLabelEnum.Income);
            Ledger_Add_Account("OtherIncome", AccountTypeEnum.Income, AccountLabelEnum.Income);
            Ledger_Add_Account("Refunds", AccountTypeEnum.Income, AccountLabelEnum.Income);

            //Cogs
            Ledger_Add_Account("CostOfGoodsSold", AccountTypeEnum.Expense, AccountLabelEnum.CostOfGoodsSold);
            //Expense
            Ledger_Add_Account("Auto", AccountTypeEnum.Expense, AccountLabelEnum.Expense);
            Ledger_Add_Account("Travel", AccountTypeEnum.Expense, AccountLabelEnum.Expense);
            Ledger_Add_Account("Hotels", AccountTypeEnum.Expense, AccountLabelEnum.Expense);
            Ledger_Add_Account("Rent", AccountTypeEnum.Expense, AccountLabelEnum.Expense);
            Ledger_Add_Account("Uncategorized Expense", AccountTypeEnum.Expense, AccountLabelEnum.Expense, "0");
            Ledger_Add_Account("Uncategorized Income", AccountTypeEnum.Income, AccountLabelEnum.Income, "1");
            //Assets
            Ledger_Add_Account("AccountsReceivable", AccountTypeEnum.Asset, AccountLabelEnum.AccountsReceivable);
            Ledger_Add_Account("Inventory", AccountTypeEnum.Asset, AccountLabelEnum.OtherCurrentAsset);
            Ledger_Add_Account("CheckingAccount", AccountTypeEnum.Asset, AccountLabelEnum.Bank);
            Ledger_Add_Account("Scottrade", AccountTypeEnum.Asset, AccountLabelEnum.OtherCurrentAsset);

            //Liabilities
            Ledger_Add_Account("AccountsPayable", AccountTypeEnum.Liability, AccountLabelEnum.AccountsPayable);
            Ledger_Add_Account("LineofCredit", AccountTypeEnum.Liability, AccountLabelEnum.OtherCurrentLiability);
            Ledger_Add_Account("CreditCard", AccountTypeEnum.Liability, AccountLabelEnum.CreditCard);

            Ledger_Add_Account("OwnersContribution", AccountTypeEnum.Equity, AccountLabelEnum.Equity);
            Ledger_Add_Account("Owner1", AccountTypeEnum.Equity, AccountLabelEnum.Equity);
            Ledger_Add_Account("Equity", AccountTypeEnum.Equity, AccountLabelEnum.Equity);

            Ledger_Update("Refunds", AccountTypeEnum.Income, "Refunds", "OtherIncome", AccountLabelEnum.Income);
            Ledger_Update("Travel", AccountTypeEnum.Expense, "Travel", "Auto", AccountLabelEnum.Expense);
            Ledger_Update("Hotels", AccountTypeEnum.Expense, "Hotels", "Travel", AccountLabelEnum.Expense);

            Ledger_Update("Owner1", AccountTypeEnum.Equity, "Owner1", "OwnersContribution", AccountLabelEnum.Equity);

            var bussinessLedger = _ledgerService.GetById(BusinessLedgerId);

            var command = Ledger_CreateTransaction("11212")
              .AddEntries(bussinessLedger,
                            new EntryData("CostOfGoodsSold", 450, 0, CurrentDate),
                            new EntryData("CheckingAccount", 0, 450, CurrentDate));
            Send(command);

            command = Ledger_CreateTransaction("2121212")
                     .AddEntries(bussinessLedger,
                            new EntryData("CostOfGoodsSold", 555, 0, CurrentDate),
                            new EntryData("CheckingAccount", 0, 555, CurrentDate));

            Send(command);

            command = Ledger_CreateTransaction("21")
                  .AddEntries(bussinessLedger,
                            new EntryData("CostOfGoodsSold", 8597, 0, CurrentDate),
                            new EntryData("CheckingAccount", 0, 8597, CurrentDate));
            Send(command);

            command = Ledger_CreateTransaction("22")
                  .AddEntries(bussinessLedger,
                            new EntryData("CostOfGoodsSold", 88800, 0, CurrentDate.AddYears(-1)),
                            new EntryData("CheckingAccount", 0, 88800, CurrentDate.AddYears(-1)));
            Send(command);

            command = Ledger_CreateTransaction("3")
                 .AddEntries(bussinessLedger,
                            new EntryData("CheckingAccount", 0, 99900, CurrentDate.AddDays(-7)),
                            new EntryData("Inventory", 99900, 0, CurrentDate.AddDays(-7)));
            Send(command);

            command = Ledger_CreateTransaction("31212")
                      .AddEntries(bussinessLedger,
                            new EntryData("CheckingAccount", 0, 54000, CurrentDate.AddDays(-7)),
                            new EntryData("Inventory", 54000, 0, CurrentDate.AddDays(-7)));
            Send(command);

            command = Ledger_CreateTransaction("31")
                 .AddEntries(bussinessLedger,
                            new EntryData("Refunds", 0, 4500, CurrentDate.AddMonths(-1)),
                            new EntryData("Inventory", 4500, 0, CurrentDate.AddMonths(-1)));
            Send(command);

            command = Ledger_CreateTransaction("32")
                    .AddEntries(bussinessLedger,
                            new EntryData("OtherIncome", 48966, 0, CurrentDate),
                            new EntryData("CheckingAccount", 0, 48966, CurrentDate));
            Send(command);

            command = Ledger_CreateTransaction("32")
                   .AddEntries(bussinessLedger,
                            new EntryData("Sales", 0, 48966, CurrentDate),
                            new EntryData("CheckingAccount", 48966, 0, CurrentDate));
            Send(command);

            command = Ledger_CreateTransaction("32")
                   .AddEntries(bussinessLedger,
                            new EntryData("Sales", 0, 966, CurrentDate),
                            new EntryData("CheckingAccount", 966, 0, CurrentDate));
            Send(command);

            command = Ledger_CreateTransaction("32")
                     .AddEntries(bussinessLedger,
                            new EntryData("Sales", 0, 4866, CurrentDate),
                            new EntryData("CheckingAccount", 4866, 0, CurrentDate));
            Send(command);

            command = Ledger_CreateTransaction("51")
                 .AddEntries(bussinessLedger,
                            new EntryData("Equity", 96557, 0, CurrentDate),
                            new EntryData("CheckingAccount", 0, 96557, CurrentDate));
            Send(command);

            command = Ledger_CreateTransaction("52")
                .AddEntries(bussinessLedger,
                            new EntryData("CostOfGoodsSold", 1500, 0, CurrentDate),
                            new EntryData("CreditCard", 0, 1500, CurrentDate));
            Send(command);

            command = Ledger_CreateTransaction("55")
                .AddEntries(bussinessLedger,
                            new EntryData("CheckingAccount", 0, 1500, CurrentDate),
                            new EntryData("CreditCard", 1500, 0, CurrentDate));
            Send(command);

            command = Ledger_CreateTransaction("56")
                .AddEntries(bussinessLedger,
                            new EntryData("CreditCard", 0, 1500, CurrentDate),
                            new EntryData("Auto", 1500, 0, CurrentDate));
            Send(command);

            command = Ledger_CreateTransaction("57")
                 .AddEntries(bussinessLedger,
                            new EntryData("CheckingAccount", 0, 500, CurrentDate),
                            new EntryData("Travel", 250, 0, CurrentDate),
                            new EntryData("Rent", 250, 0, CurrentDate));
            Send(command);

            var addUser = new Ledger_User_AddCommand { LedgerId = BusinessLedgerId, UserId = "1" };
            Send(addUser);
        }


        public void Ledger_Add_Account(String name, AccountTypeEnum typeEnum, AccountLabelEnum labelEnum)
        {
            var command = new Ledger_Account_CreateCommand
            {
                AccountId = name,
                AccountTypeEnum = typeEnum,
                AccountLabelEnum = labelEnum,
                LedgerId = BusinessLedgerId,
                Name = name,
                InterestRatePerc = InterestRate,
                MinMonthPaymentInCents = MinMonthPay,
            };

            Send(command);
        }

        public void Ledger_Add_Account(String name, AccountTypeEnum typeEnum, AccountLabelEnum labelEnum, string accountId)
        {
            var command = new Ledger_Account_CreateCommand
            {
                AccountId = accountId,
                AccountTypeEnum = typeEnum,
                AccountLabelEnum = labelEnum,
                LedgerId = BusinessLedgerId,
                Name = name,
                InterestRatePerc = InterestRate,
                MinMonthPaymentInCents = MinMonthPay,
            };

            Send(command);
        }

        public void Ledger_Update(string accountId, AccountTypeEnum accountType, string name, string parentAccountId, AccountLabelEnum accountLabel)
        {
            var command = new Ledger_Account_UpdateCommand
            {
                AccountId = accountId,
                LedgerId = BusinessLedgerId,
                Name = name,
                ParentAccountId = parentAccountId,
            };

            Send(command);
        }

        public Transaction_CreateCommand Ledger_CreateTransaction(string id, TransactionType type = TransactionType.Bill)
        {
            var command = _objectRepository.Load<TransactionDto, Transaction_CreateCommand>(new TransactionDto { TransactionId = id, LedgerId = BusinessLedgerId, Type = type });

            return command;
        }

        #endregion

        #region Affiliates

        public void CreateAffiliates()
        {
            var devAffiliateCommand = new Affiliate_CreateCommand
                              {
                                  Id = "d0d6da2b-ab71-43c1-a000-aba274901cb4",
                                  Name = "Default"
                              };
            Send(devAffiliateCommand);

            var updateDevCommand = new Affiliate_UpdateCommand
                          {
                              ApplicationId = "d0d6da2b-ab71-43c1-a000-aba274901cb4",
                              ApplicationName = "Default",
                              AssemblyName = null,
                              ChargifySharedKey = "QwBWs6T4QRh8ae-tfKiN",
                              ChargifyUrl = "https://mpower-local.chargify.com",
                              ContactPhoneNumber = "(888) 123-4567",
                              DisplayName = "mPowering Networks",
                              EmailSuffix = "@mpowering.com",
                              JanrainAppApiKey = "12967abb698a65070d425f07b679091d1ea44b63",
                              JanrainAppUrl = "http://mpower.rpxnow.com/openid/embed?token_url=http%3A%2F%2Flocalhost%3A8080%2Fapi%2Fjanrain%2Fsignin",
                              LegalName = "MPOWER DEVELOPMENT",
                              MembershipApiKey = "B89DA1C7DC2B40319802BCF4F6B2C122",
                              SmtpCredentialsEmail = "admin@mpowering.com",
                              SmtpCredentialsPassword = "mP2009!",
                              SmtpEnableSsl = true,
                              SmtpHost = "secure.emailsrvr.com",
                              SmtpPort = 587,
                              UrlPaths = new List<string> { "http://localhost:8081/", "http://orsich-pc:8080", "http://localhost:8080/" },
                              PfmEnabled = true,
                              BfmEnabled = true,
                              CreditAppEnabled = true,
                          };

            Send(updateDevCommand);

            _affiliatesController.SyncChargifyProducts(devAffiliateCommand.Id);

            var betaAffiliateCommand = new Affiliate_CreateCommand
            {
                Id = "d0d6da2b-ab71-43c1-a000-aba274901cc5",
                Name = "Dev"
            };
            Send(betaAffiliateCommand);

            var updateBetaCommand = new Affiliate_UpdateCommand
            {
                ApplicationId = "d0d6da2b-ab71-43c1-a000-aba274901cc5",
                ApplicationName = "Dev",
                AssemblyName = "mPowerCorporate",
                ChargifySharedKey = "knKdjNqio7JvHZmZhxeb",
                ChargifyUrl = "https://mpower-dev.chargify.com",
                ContactPhoneNumber = "(888) 123-4567",
                DisplayName = "mPowering Networks",
                EmailSuffix = "@mpowering.com",
                JanrainAppApiKey = "12967abb698a65070d425f07b679091d1ea44b63",
                JanrainAppUrl = "http://paralect.rpxnow.com/openid/embed?token_url=http%3A%2F%2Flocalhost%3A8080%2Fapi%2Fjanrain%2Fsignin",
                LegalName = "MPOWER DEVELOPMENT",
                MembershipApiKey = "B89DA1C7DC2B40319802BCF4F6B2C122",
                SmtpCredentialsEmail = "admin@mpowering.com",
                SmtpCredentialsPassword = "mP2009!",
                SmtpEnableSsl = true,
                SmtpHost = "secure.emailsrvr.com",
                SmtpPort = 587,
                UrlPaths = new List<string> { "https://dev.mpowering.com/", "http://dev.mpowering.com/" }
            };

            Send(updateBetaCommand);

            _affiliatesController.SyncChargifyProducts(betaAffiliateCommand.Id);

            var stageAffiliateCommand = new Affiliate_CreateCommand
            {
                Id = "d0d6da2b-ab71-43c1-a000-aba274901cd6",
                Name = "Staging"
            };
            Send(stageAffiliateCommand);

            var updateStageCommand = new Affiliate_UpdateCommand
            {
                ApplicationId = "d0d6da2b-ab71-43c1-a000-aba274901cd6",
                ApplicationName = "Staging",
                AssemblyName = null,
                ChargifySharedKey = "6RA2br3vAnRUN0LS45fV",
                ChargifyUrl = "https://mpower-staging.chargify.com",
                ContactPhoneNumber = "(888) 123-4567",
                DisplayName = "mPowering Networks",
                EmailSuffix = "@mpowering.com",
                JanrainAppApiKey = "12967abb698a65070d425f07b679091d1ea44b63",
                JanrainAppUrl = "http://paralect.rpxnow.com/openid/embed?token_url=http%3A%2F%2Flocalhost%3A8080%2Fapi%2Fjanrain%2Fsignin",
                LegalName = "MPOWER DEVELOPMENT",
                MembershipApiKey = "B89DA1C7DC2B40319802BCF4F6B2C122",
                SmtpCredentialsEmail = "admin@mpowering.com",
                SmtpCredentialsPassword = "mP2009!",
                SmtpEnableSsl = true,
                SmtpHost = "secure.emailsrvr.com",
                SmtpPort = 587,
                UrlPaths = new List<string> { "https://staging.mpowering.com/", "http://staging.mpowering.com/" }
            };

            Send(updateStageCommand);

            _affiliatesController.SyncChargifyProducts(stageAffiliateCommand.Id);
        }
        #endregion

        #region Users

        public void CreateUsers()
        {
            var command = new User_CreateCommand
                              {
                                  ApplicationId = "d0d6da2b-ab71-43c1-a000-aba274901cb4",
                                  CreateDate = DateTime.Now,
                                  Email = "test@test2.com",
                                  FirstName = "Demo first",
                                  LastName = "Demo last",
                                  IsActive = true,
                                  PasswordHash = SecurityUtil.GetMD5Hash("1"),
                                  UserId = UserId,
                                  UserName = "demo",
                              };

            Send(command);

            ////var yodleeCmd = new User_AddYodleeAccountCommand
            ////{
            ////    UserId = command.UserId,
            ////    EmailAddress = command.Email,
            ////    LoginName = command.UserName,
            ////    Password = "b7babdc1-7842-4516-9315-be5192d72ed5",
            ////};

            ////Send(yodleeCmd);

            var addPermissionCommand = new User_AddPermissionCommand { UserId = command.UserId, Permission = UserPermissionEnum.AffiliateAdminView };
            Send(addPermissionCommand);
            addPermissionCommand = new User_AddPermissionCommand { UserId = command.UserId, Permission = UserPermissionEnum.AffiliateAdminEdit };
            Send(addPermissionCommand);
            addPermissionCommand = new User_AddPermissionCommand { UserId = command.UserId, Permission = UserPermissionEnum.GlobalAdminView };
            Send(addPermissionCommand);
            addPermissionCommand = new User_AddPermissionCommand { UserId = command.UserId, Permission = UserPermissionEnum.GlobalAdminEdit };
            Send(addPermissionCommand);

            var subscribeCommand = new User_Subscription_SubscribeCommand
            {
                BillingAddress = "n/a",
                BillingCity = "n/a",
                BillingCountry = "n/a",
                BillingState = "n/a",
                BillingZip = "n/a",
                CVV = "n/a",
                Email = command.Email,
                FirstName = command.FirstName,
                LastName = command.LastName,
                Organization = "Mpower",
                UserId = command.UserId,
                ChargifyCustomerSystemId = _idGenerator.Generate(),
                ProductName = "PFM Suite",
                ProductPriceInCents = 2499,
                ProductHandle = "1",
            };

            Send(subscribeCommand);
        }

        #endregion

        #region Credit Identities

        private void CreateCreditIdentity()
        {

            for (int i = 10; i < 13; i++)
            {
                var command = new CreditIdentity_CreateCommand
                {
                    UserId = UserId,
                    Data = new CreditIdentityData
                    {
                        Address = "Main Street, NY",
                        Address2 = "NY",
                        City = "New York",
                        State = "New York",
                        PostalCode = "123456",
                        FirstName = "Demo first",
                        MiddleName = "Demo middle",
                        LastName = "Demo last",
                        SocialSecurityNumber = _encrypter.Encode("0000000" + i),
                        DateOfBirth = new DateTime(1980, 1, i),
                        ClientKey = ClientKey + i,
                        AlertSubscriptionId = "qwertyuiop",
                    },
                };

                Send(command);
            }



        }

        private void SetVerificationQuestions()
        {

            for (int i = 10; i < 13; i++)
            {


                var questions = new List<VerificationQuestionData>
                {
                    new VerificationQuestionData
                        {
                            SequenceNumber = 1,
                            IsFakeQuestion = false,
                            IsLastChanceQuestion = false,
                            QuestionType = "type 1",
                            Question = "How old are you?",
                            Answers = new List<VerificationAnswerData>
                            {
                                new VerificationAnswerData
                                    {
                                        SequenceNumber = 1,
                                        IsCorrect = false,
                                        Answer = "< 20",
                                    },
                                new VerificationAnswerData
                                    {
                                        SequenceNumber = 2,
                                        IsCorrect = true,
                                        Answer = "from 20 to 30",
                                    },
                                new VerificationAnswerData
                                    {
                                        SequenceNumber = 3,
                                        IsCorrect = false,
                                        Answer = "> 30",
                                    },
                            },
                        },
                    new VerificationQuestionData
                        {
                            SequenceNumber = 2,
                            IsFakeQuestion = false,
                            IsLastChanceQuestion = false,
                            QuestionType = "type 2",
                            Question = "What is your Name",
                            Answers = new List<VerificationAnswerData>
                            {
                                new VerificationAnswerData
                                    {
                                        SequenceNumber = 1,
                                        IsCorrect = false,
                                        Answer = "John",
                                    },
                                new VerificationAnswerData
                                    {
                                        SequenceNumber = 2,
                                        IsCorrect = false,
                                        Answer = "Mike",
                                    },
                                new VerificationAnswerData
                                    {
                                        SequenceNumber = 3,
                                        IsCorrect = true,
                                        Answer = "Demo",
                                    },
                            },
                        },
                };

                var command = new CreditIdentity_Questions_SetCommand
                                  {
                                      ClientKey = ClientKey + i,
                                      Questions = questions,
                                  };

                Send(command);
            }
        }

        private void AddCreditReports()
        {
            for (int i = 10; i < 13; i++)
            {
                Send(GenerateReport(i));
                Send(GenerateReport(i));

            }
        }

        private ICommand GenerateReport(int i)
        {
            var borrower = new BorrowerData
            {
                Names = new List<BorrowerNameData>
                {
                    new BorrowerNameData
                    {
                        Bureau = "Bureau test",
                        Description = "Description",
                        FirstName = "First Name",
                        LastName = "Last Name",
                        InquiryDate = DateTime.Now,
                        MiddleName = "V",
                        Prefix = "Mr",
                        Reference = "Ref",
                        Suffix = "V"
                    },
                    new BorrowerNameData
                    {
                        Bureau = "Bureau test",
                        Description = "Description",
                        FirstName = "First Name",
                        LastName = "Last Name",
                        InquiryDate = DateTime.Now,
                        MiddleName = "V",
                        Prefix = "Mr",
                        Reference = "Ref",
                        Suffix = "V"
                    }
                },
                BirthDates = new List<BorrowerBirthDateData>
                {
                    new BorrowerBirthDateData
                    {
                        Age = 23,
                        BirthDate = DateTime.Now.AddYears(23),
                        BirthDay = 4,
                        BirthMonth = 3,
                        BirthYear = 1988
                    },
                    new BorrowerBirthDateData
                    {
                        Age = 23,
                        BirthDate = DateTime.Now.AddYears(23),
                        BirthDay = 4,
                        BirthMonth = 3,
                        BirthYear = 1988
                    }
                },
                Addresses = new List<AddressData>
                {
                    new AddressData
                    {
                        City = "New York",
                        Country = "USA",
                        County = "County",
                        HouseNumber = "114",
                        PostalCode = "221000",
                        State = "Florida",
                        StreetName = "street",
                        StreetType = "S",
                        Unit = "Unit"
                    }
                },
                CreditScores = new List<BorrowerCreditScoreData>
                {
                    new BorrowerCreditScoreData
                    {
                        Bureau = "Bureau 1",
                        Score = 500,
                        InquiryDate = DateTime.Now.AddDays(5),
                        CreditScoreFactors = new List<BorrowerCreditScoreFactorData>
                        {
                            new BorrowerCreditScoreFactorData
                            {
                                Bureau = "Bureau 1",
                                FactorAbbreviation = "FactorAbbreviation",
                                FactorDescription = "FactorDescription",
                                FactorRank = 4,
                                FactorSymbol = "S"
                            },
                            new BorrowerCreditScoreFactorData
                            {
                                Bureau = "Bureau 1",
                                FactorAbbreviation = "FactorAbbreviation",
                                FactorDescription = "FactorDescription",
                                FactorRank = 4,
                                FactorSymbol = "S"
                            }
                        }
                    }
                },
                CreditStatements = new List<BorrowerCreditStatementData>
                {
                    new BorrowerCreditStatementData
                    {
                        Bureau = "Bureau",
                        InquiryDate = DateTime.Now,
                        Statement = "Statement",
                        StatementTypeAbbreviation = "S",
                        StatementTypeDescription = "Descr",
                        StatementTypeRank = 3,
                        StatementTypeSymbol = "StatementTypeSymbol"
                    }
                },
                Employers = new List<BorrowerEmployerData>
                {
                    new BorrowerEmployerData
                    {
                        Address = new AddressData
                        {
                            City = "New York",
                            Country = "USA",
                            County = "County",
                            HouseNumber = "114",
                            PostalCode = "221000",
                            State = "Florida",
                            StreetName = "street",
                            StreetType = "S",
                            Unit = "Unit"
                        },
                    }
                },
                PreviousAddresses = new List<AddressData>
                {
                    new AddressData
                    {
                        City = "New York",
                        Country = "USA",
                        County = "County",
                        HouseNumber = "114",
                        PostalCode = "221000",
                        State = "Florida",
                        StreetName = "street",
                        StreetType = "S",
                        Unit = "Unit"
                    }
                },
                SocialSecurityNumber = "xx-xxx-1345",
                SocialSecurityNumbers = new List<BorrowerSocialData>
                {
                    new BorrowerSocialData
                    {
                        Bureau = "Bureau 13",
                        InquiryDate = DateTime.Now,
                        SocialSecurityNumber = "xx-xxx-1345",
                    }
                },
                Telephones = new List<BorrowerTelephoneData>
                {
                    new BorrowerTelephoneData
                    {
                        AreaCode = "A",
                        Extension = "Ext",
                        Number = "1"
                    }
                }
            };

            return new CreditIdentity_Report_AddCommand
             {
                 ClientKey = ClientKey + i,
                 CreditReportId = Guid.NewGuid().ToString(),
                 CreditScoreId = Guid.NewGuid().ToString(),
                 Data = new CreditReportData
                 {
                     Borrowers = new List<BorrowerData> { borrower },
                     Inquiries = new List<InquiryData>
                     {
                         new InquiryData
                         {
                             Bureau = "Bureau 1",
                             InquiryDate = DateTime.Now,
                             IndustryCodeAbbreviation = "I",
                             IndustryCodeDescription = "D",
                             IndustryCodeRank = 1,
                             InquiryType = "InquiryType",
                             SubscriberName = "Brett",
                             SubscriberNumber = "112123"
                         }
                    },
                     Messages = new List<MessageData>
                    {
                        new MessageData
                            {
                                Text = "Hi. I am a message",
                                CodeDescription = "code descr",
                                CodeSymbol = "Symbol",
                                Rank = 12,
                                TypeDescription = "type desc",
                                TypeSymbol = "TS"
                            }
                    },
                     PublicRecords = new List<PublicRecordData>
                    {
                        new PublicRecordData
                            {
                                Bureau = "Bureau",
                                Status = "status",
                                IndustryCodeDescription = "descr",
                                ClassificationDescription =
                                    "classification descr",
                                ClassificationRank = 12,
                                ClassificationSymbol = "S",
                                CourtName = "court name",
                                CustomRemarks = new List<string> {"remark"},
                                DateExpires = DateTime.Now,
                                DateFiled = DateTime.Now,
                                DateVerified = DateTime.Now,
                                DesignatorDescription = "s",
                                IndustryCodeSymbol = "as",
                                IndustryRank = 1,
                                ReferenceNumber = "1",
                                SubscriberCode = "code",
                                Type = "type"
                            }
                    },
                     Creditors = new List<CreditorData>
                    {
                        new CreditorData
                            {
                                Address = new AddressData
                                            {
                                                City = "New York",
                                                Country = "USA",
                                                County = "County",
                                                HouseNumber = "114",
                                                PostalCode = "221000",
                                                State = "Florida",
                                                StreetName = "street",
                                                StreetType = "S",
                                                Unit = "Unit"
                                            }
                            }
                    },
                     AccountsGroups = new List<AccountsGroupData>
                    {
                        new AccountsGroupData
                            {
                                AccountTypeAbbreviation = "ATA",
                                AccountTypeDescription = "descr",
                                AccountTypeSymbol = "S",
                                Accounts = new List<AccountData>
                                {
                                    new AccountData
                                        {
                                            Bureau = "Bureau 1",
                                            IndustryCodeDescription = "Descr",
                                            DateReported = DateTime.Now,
                                            DateVerified = DateTime.Now,
                                            IndustryCodeSymbol = "ICS",
                                            HighBalance = 221232,
                                            DisputeDescription = "descr",
                                            AccountStatusDate = DateTime.Now,
                                            AccountNumber = "Number",
                                            DateOpened = DateTime.Now,
                                            DateClosed = DateTime.Now,
                                            CurrentBalance = 12322,
                                            CreditorName = "Creditor Name",
                                            AccountCondition = "Account condition",
                                            AccountConditionRank = 11,
                                            AccountConditionSymbol = "symbol",
                                            AccountDesignator = "Designator",
                                            OpenClosedDescription = "OCD",
                                            OpenClosedSymbol = "S",
                                            PayStatusDescription = "descr",
                                            PayStatusSymbol = "PSS",
                                            SubscriberCode = "11",
                                            VerificationIndicator = "123",
                                            AccountTypeAbbreviation = "Recurring",
                                            AccountTypeSymbol = "R",
                                            AccountTypeDescription = "Recurring Line",
                                            AmountPastDue = 50,
                                            MonthlyPayStatus = new List<MonthlyPayStatusData>
                                            {
                                                new MonthlyPayStatusData{Date = DateTime.Now,Status = "OnTime"},
                                                new MonthlyPayStatusData{Date = DateTime.Now.AddMonths(-1),Status = "OnTime"},
                                                new MonthlyPayStatusData{Date = DateTime.Now.AddMonths(-2),Status = "OnTime"},
                                                new MonthlyPayStatusData{Date = DateTime.Now.AddMonths(-3),Status = "OnTime"},
                                                new MonthlyPayStatusData{Date = DateTime.Now.AddMonths(-4),Status = "OnTime"},
                                                new MonthlyPayStatusData{Date = DateTime.Now.AddMonths(-5),Status = "OnTime"},
                                                new MonthlyPayStatusData{Date = DateTime.Now.AddYears(-1),Status = "OnTime"},
                                                new MonthlyPayStatusData{Date = DateTime.Now.AddYears(-2),Status = "OnTime"},
                                            }
                                        }
                                }
                            }
                    },
                     CurrentVersion = "version 1.0",
                     Score = 692 + i,
                     ScoreDate = DateTime.Now.AddDays(1000 - 10*i),
                     QualitativeRank = 61+i,
                     PopulationRank = 70 + (new Random()).Next(-20, 20),
                     BureauSource = "dummy bureau source",
                     NegativeFactors = new List<string> { "factor 1", "factor 2" },
                     IsDeceased = false,
                     IsFraud = false,
                     SafetyCheckPassed = true,
                     IsFrozen = false,
                     InquiriesInLastTwoYears = 0,
                     PublicRecordCount = 0,
                     ClosedAccountsCount = 1,
                     DeliquentAccountsCount = 1,
                     DerogatoryAccountsCount = 1,
                     OpenAccountsCount = 1,
                     TotalAccountsCount = 3,
                     TotalBalances = 321321.321,
                     TotalMonthlyPayments = 123123.123,
                     Status = "newly added",
                 }
             };
        }

        private void AddCreditAlerts()
        {
            for (int i = 10; i < 13; i++)
            {

                var creditAlerts = new List<AlertData>
                {
                    new AlertData
                        {
                            Type = AlertTypeEnum.Account,
                            Date = DateTime.Now,
                            Message = "First"
                        },
                    new AlertData
                        {
                            Type = AlertTypeEnum.Account,
                            Date = DateTime.Now,
                            Message = "Second"
                        },
                };

                var command = new CreditIdentity_Alerts_AddCommand
                {
                    ClientKey = ClientKey + i,
                    Alerts = creditAlerts,
                };

                Send(command);

            }
        }

        #endregion
    }

    public static class TestDataExtentions
    {
        public static Transaction_CreateCommand AddEntries(this Transaction_CreateCommand tran, LedgerDocument ledger, params EntryData[] entries)
        {
            var transactionGenerator = new TransactionGenerator(null);

            foreach (var entryData in entries)
            {
                entryData.Payee = "Payee " + entryData.AccountId;
                entryData.Memo = "Memo " + entryData.AccountId;
            }

            if (tran.Entries == null)
                tran.Entries = new List<ExpandedEntryData>();

            tran.Entries.AddRange(TransactionGenerator.ExpandEntryData(ledger, entries));
            return tran;
        }
    }
}
