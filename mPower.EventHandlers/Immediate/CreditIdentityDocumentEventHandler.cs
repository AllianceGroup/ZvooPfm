using System;
using System.Linq;
using MongoDB.Driver.Builders;
using Paralect.ServiceBus;
using mPower.Documents.DocumentServices.Credit;
using mPower.Documents.Documents.Credit.CreditIdentity;
using mPower.Domain.Accounting.CreditIdentity.Data;
using mPower.Domain.Accounting.CreditIdentity.Events;
using mPower.Domain.Membership.User.Events;
using mPower.Framework.Environment;
using mPower.Framework.Utils.CreditCalculator;

namespace mPower.EventHandlers.Immediate
{
    public class CreditIdentityDocumentEventHandler :
        IMessageHandler<CreditIdentity_CreatedEvent>,
        IMessageHandler<CreditIdentity_DeletedEvent>,
        IMessageHandler<CreditIdentity_Report_AddedEvent>,
        IMessageHandler<CreditIdentity_Questions_SetEvent>,
        IMessageHandler<CreditIdentity_EnrolledEvent>,
        IMessageHandler<CreditIdentity_CanceledEnrollEvent>,
        IMessageHandler<CreditIdentity_MarkedAsVerifiedEvent>,
        IMessageHandler<User_Subscription_DeletedEvent>
        
    {
        private readonly CreditIdentityDocumentService _creditIdentityService;
        private readonly IIdGenerator _idGenerator;

        public CreditIdentityDocumentEventHandler(CreditIdentityDocumentService creditIdentityService, IIdGenerator idGenerator)
        {
            _creditIdentityService = creditIdentityService;
            _idGenerator = idGenerator;
        }

        public void Handle(CreditIdentity_CreatedEvent message)
        {
            var creditIdentity = new CreditIdentityDocument
                                     {
                                         UserId = message.UserId,
                                         Address = message.Data.Address,
                                         Address2 = message.Data.Address2,
                                         City = message.Data.City,
                                         State = message.Data.State,
                                         PostalCode = message.Data.PostalCode,
                                         FirstName = message.Data.FirstName,
                                         MiddleName = message.Data.MiddleName,
                                         LastName = message.Data.LastName,
                                         Suffix = message.Data.Suffix,
                                         SocialSecurityNumber = message.Data.SocialSecurityNumber,
                                         BirthYear = message.Data.DateOfBirth.Year,
                                         DateOfBirth = message.Data.DateOfBirth,
                                         ClientKey = message.Data.ClientKey,
                                         AlertSubscriptionId = message.Data.AlertSubscriptionId,
                                     };

            _creditIdentityService.Save(creditIdentity);
        }

        public void Handle(CreditIdentity_DeletedEvent message)
        {
            _creditIdentityService.RemoveById(message.ClientKey);
        }

        public void Handle(CreditIdentity_Report_AddedEvent message)
        {
            var creditReport = ToDocument(message.CreditReportId, message.Data);
            
            _creditIdentityService.AddCreditReport(message.ClientKey, creditReport);
        }

        public void Handle(CreditIdentity_Questions_SetEvent message)
        {
            var questions = message.Questions.Select(q =>
                                                     new VerificationQuestionDocument
                                                         {
                                                             Id = q.Id,
                                                             Answers = q.Answers.Select(a => new VerificationAnswerDocument
                                                                                                 {
                                                                                                     Id = _idGenerator.Generate(),
                                                                                                     IsCorrect = a.IsCorrect,
                                                                                                     Answer = a.Answer,
                                                                                                     SequenceNumber = a.SequenceNumber,
                                                                                                 }).ToList(),
                                                             IsFakeQuestion = q.IsFakeQuestion,
                                                             IsLastChanceQuestion = q.IsLastChanceQuestion,
                                                             QuestionType = q.QuestionType,
                                                             SequenceNumber = q.SequenceNumber,
                                                             Question = q.Question,
                                                            
                                                         }).ToList();

            _creditIdentityService.SetValidationQuestions(message.ClientKey, questions);
        }

        public void Handle(CreditIdentity_EnrolledEvent message)
        {
            var query = Query.EQ("_id", message.CreditIdentityId);

            var update = Update<CreditIdentityDocument>.Set(x => x.Enroll.MemberId, message.MemberId)
                               .Set(x => x.Enroll.IsEnrolled, true);
            if (!String.IsNullOrEmpty(message.ActivationCode))
            {
                update.Set(x => x.Enroll.ActivationCode, message.ActivationCode);
            }

            if (!String.IsNullOrEmpty(message.SalesId))
            {
                update.Set(x => x.Enroll.SalesId, message.SalesId);
            }

            _creditIdentityService.Update(query, update);
        }

        public void Handle(CreditIdentity_CanceledEnrollEvent message)
        {
            var query = Query.EQ("_id", message.CreditIdentityId);

            var update = Update<CreditIdentityDocument>.Set(x => x.Enroll.IsEnrolled, false);

            _creditIdentityService.Update(query, update);
        }

        public void Handle(CreditIdentity_MarkedAsVerifiedEvent message)
        {
            _creditIdentityService.MarkAsVerified(message.ClientKey, message.Date, message.IpAddress);
        }

        #region Mapping

        private CreditReportDocument ToDocument(string id, CreditReportData creditReportData)
        {
            return new CreditReportDocument
                       {
                           Id = id,
                           Borrowers = creditReportData.Borrowers == null ? null : creditReportData.Borrowers.Select(b => ToDocument(_idGenerator.Generate(), b)).ToList(),
                           Inquiries = creditReportData.Inquiries == null ? null : creditReportData.Inquiries.Select(i => ToDocument(_idGenerator.Generate(), i)).ToList(),
                           Messages = creditReportData.Messages == null ? null : creditReportData.Messages.Select(m => ToDocument(_idGenerator.Generate(), m)).ToList(),
                           PublicRecords = creditReportData.PublicRecords == null ? null : creditReportData.PublicRecords.Select(p => ToDocument(_idGenerator.Generate(), p)).ToList(),
                           Creditors = creditReportData.Creditors == null ? null : creditReportData.Creditors.Select(c => ToDocument(_idGenerator.Generate(), c)).ToList(),
                           AccountsGroups = creditReportData.AccountsGroups == null ? null : creditReportData.AccountsGroups.Select(ag => ToDocument(_idGenerator.Generate(), ag)).ToList(),
                           CurrentVersion = creditReportData.CurrentVersion,
                           Score = creditReportData.Score,
                           ScoreDate = creditReportData.ScoreDate,
                           Grade = CreditCalculator.CreditGrade(creditReportData.PopulationRank),
                           QualitativeRank = creditReportData.QualitativeRank,
                           PopulationRank = creditReportData.PopulationRank,
                           BureauSource = creditReportData.BureauSource,
                           NegativeFators = creditReportData.NegativeFactors,
                           IsDeceased = creditReportData.IsDeceased,
                           IsFraud = creditReportData.IsFraud,
                           SafetyCheckPassed = creditReportData.SafetyCheckPassed,
                           IsFrozen = creditReportData.IsFrozen,
                           InquiriesInLastTwoYears = creditReportData.InquiriesInLastTwoYears,
                           PublicRecordCount = creditReportData.PublicRecordCount,
                           ClosedAccountsCount = creditReportData.ClosedAccountsCount,
                           DeliquentAccountsCount = creditReportData.DeliquentAccountsCount,
                           DerogatoryAccountsCount = creditReportData.DerogatoryAccountsCount,
                           OpenAccountsCount = creditReportData.OpenAccountsCount,
                           TotalAccountsCount = creditReportData.TotalAccountsCount,
                           TotalBalances = creditReportData.TotalBalances,
                           TotalMonthlyPayments = creditReportData.TotalMonthlyPayments,
                           Status = creditReportData.Status,
                           ReportData = creditReportData.ReportData
                       };
        }

        private BorrowerDocument ToDocument(string id, BorrowerData data)
        {
            return new BorrowerDocument
                       {
                           Id = id,
                           Names = data.Names == null ? null : Enumerable.ToList<BorrowerNameDocument>(data.Names.Select(n => ToDocument(_idGenerator.Generate(), n))),
                           BirthDates = data.BirthDates == null ? null : Enumerable.ToList<BorrowerBirthDateDocument>(data.BirthDates.Select(n => ToDocument(_idGenerator.Generate(), n))),
                           Addresses = data.Addresses == null ? null : Enumerable.ToList<AddressDocument>(data.Addresses.Select(n => ToDocument(_idGenerator.Generate(), n))),
                           CreditScores = data.CreditScores == null ? null : Enumerable.ToList<BorrowerCreditScoreDocument>(data.CreditScores.Select(n => ToDocument(_idGenerator.Generate(), n))),
                           CreditStatements = data.CreditStatements == null ? null : Enumerable.ToList<BorrowerCreditStatementDocument>(data.CreditStatements.Select(n => ToDocument(_idGenerator.Generate(), n))),
                           Employers = data.Employers == null ? null : Enumerable.ToList<BorrowerEmployerDocument>(data.Employers.Select(n => ToDocument(_idGenerator.Generate(), n))),
                           PreviousAddresses = data.PreviousAddresses == null ? null : Enumerable.ToList<AddressDocument>(data.PreviousAddresses.Select(n => ToDocument(_idGenerator.Generate(), n))),
                           SocialSecurityNumber = data.SocialSecurityNumber,
                           SocialSecurityNumbers = data.SocialSecurityNumbers == null ? null : Enumerable.ToList<BorrowerSocialDocument>(data.SocialSecurityNumbers.Select(n => ToDocument(_idGenerator.Generate(), n))),
                           Telephones = data.Telephones == null ? null : Enumerable.ToList<BorrowerTelephoneDocument>(data.Telephones.Select(n => ToDocument(_idGenerator.Generate(), n)))
                       };
        }

        private BorrowerTelephoneDocument ToDocument(string id, BorrowerTelephoneData data)
        {
            return new BorrowerTelephoneDocument()
                       {
                           Id = id,
                           AreaCode = data.AreaCode,
                           Extension = data.Extension,
                           Number = data.Number
                       };
        }

        private BorrowerSocialDocument ToDocument(string id, BorrowerSocialData data)
        {
            return new BorrowerSocialDocument()
                       {
                           Id = id,
                           Bureau = data.Bureau,
                           InquiryDate = data.InquiryDate,
                           SocialSecurityNumber = data.SocialSecurityNumber
                       };
        }

        private BorrowerEmployerDocument ToDocument(string id, BorrowerEmployerData data)
        {
            return new BorrowerEmployerDocument()
                       {
                           Id = id,
                           Name = data.Name,
                           Address = ToDocument(_idGenerator.Generate(), data.Address),
                           DateReported = data.DateReported,
                           DateUpdated = data.DateUpdated
                       };
        }

        private BorrowerCreditStatementDocument ToDocument(string id, BorrowerCreditStatementData data)
        {
            return new BorrowerCreditStatementDocument()
                       {
                           Id = id,
                           Bureau = data.Bureau,
                           InquiryDate = data.InquiryDate,
                           Statement = data.Statement,
                           StatementTypeAbbreviation = data.StatementTypeAbbreviation,
                           StatementTypeDescription = data.StatementTypeDescription,
                           StatementTypeRank = data.StatementTypeRank,
                           StatementTypeSymbol = data.StatementTypeSymbol
                       };
        }

        private BorrowerCreditScoreDocument ToDocument(string id, BorrowerCreditScoreData data)
        {
            return new BorrowerCreditScoreDocument()
                       {
                           Id = id,
                           Score = data.Score,
                           Bureau = data.Bureau,
                           CreditScoreFactors = data.CreditScoreFactors == null ? null :  Enumerable.ToList<BorrowerCreditScoreFactorDocument>(data.CreditScoreFactors.Select(n => ToDocument(_idGenerator.Generate(), n))),
                           InquiryDate = data.InquiryDate
                       };
        }

        private static BorrowerCreditScoreFactorDocument ToDocument(string id, BorrowerCreditScoreFactorData data)
        {
            return new BorrowerCreditScoreFactorDocument()
                       {
                           Id = id,
                           Bureau = data.Bureau,
                           FactorAbbreviation = data.FactorAbbreviation,
                           FactorDescription = data.FactorDescription,
                           FactorRank = data.FactorRank,
                           FactorSymbol = data.FactorSymbol
                       };
        }

        private static BorrowerBirthDateDocument ToDocument(string id, BorrowerBirthDateData data)
        {
            return new BorrowerBirthDateDocument()
            {
                Id = id,
                Age = data.Age,
                BirthDate = data.BirthDate,
                BirthDay = data.BirthDay,
                BirthMonth = data.BirthMonth,
                BirthYear = data.BirthYear
            };
        }

        private static BorrowerNameDocument ToDocument(string id, BorrowerNameData data)
        {
            return new BorrowerNameDocument
                       {
                           Id = id,
                           FirstName = data.FirstName,
                           MiddleName = data.MiddleName,
                           LastName = data.LastName,
                           Prefix = data.Prefix,
                           Description = data.Description,
                           InquiryDate = data.InquiryDate,
                           Suffix = data.Suffix,
                           Reference = data.Reference,
                           Bureau = data.Bureau,
                       };
        }

        private static InquiryDocument ToDocument(string id, InquiryData data)
        {
            return new InquiryDocument
                       {
                           Id = id,
                           Bureau = data.Bureau,
                           IndustryCodeAbbreviation = data.IndustryCodeAbbreviation,
                           IndustryCodeDescription = data.IndustryCodeDescription,
                           IndustryCodeRank = data.IndustryCodeRank,
                           InquiryDate = data.InquiryDate,
                           InquiryType = data.InquiryType,
                           SubscriberName = data.SubscriberName,
                           SubscriberNumber = data.SubscriberNumber,
                       };
        }

        private static MessageDocument ToDocument(string id, MessageData data)
        {
            return new MessageDocument
                       {
                           Id = id,
                           CodeDescription = data.CodeDescription,
                           CodeSymbol = data.CodeSymbol,
                           Text = data.Text,
                           TypeDescription = data.TypeDescription,
                           TypeSymbol = data.TypeSymbol,
                           Rank = data.Rank,
                       };
        }

        private static PublicRecordDocument ToDocument(string id, PublicRecordData data)
        {
            return new PublicRecordDocument
                       {
                           Id = id,
                           DateFiled = data.DateFiled,
                           DateVerified = data.DateVerified,
                           DateExpires = data.DateExpires,
                           ClassificationDescription = data.ClassificationDescription,
                           ClassificationSymbol = data.ClassificationSymbol,
                           ClassificationRank = data.ClassificationRank,
                           CourtName = data.CourtName,
                           DesignatorDescription = data.DesignatorDescription,
                           Bureau = data.Bureau,
                           IndustryCodeDescription = data.IndustryCodeDescription,
                           IndustryCodeSymbol = data.IndustryCodeSymbol,
                           IndustryRank = data.IndustryRank,
                           ReferenceNumber = data.ReferenceNumber,
                           CustomRemarks = data.CustomRemarks,
                           SubscriberCode = data.SubscriberCode,
                           Status = data.Status,
                           Type = data.Type,
                           //Item = data.Item, //Removed because the dynamic was causing errors.
                       };
        }

        private CreditorDocument ToDocument(string id, CreditorData data)
        {
            return new CreditorDocument
                       {
                           Id = id,
                           Name = data.Name,
                           IndustryCodeDescription = data.IndustryCodeDescription,
                           IndustryCodeSymbol = data.IndustryCodeSymbol,
                           IndustryCodeRank = data.IndustryCodeRank,
                           Bureau = data.Bureau,
                           InquiryDate = data.InquiryDate,
                           Code = data.Code,
                           Telephone = data.Telephone,
                           Address = ToDocument(_idGenerator.Generate(), data.Address),
                       };
        }

        private AccountsGroupDocument ToDocument(string id, AccountsGroupData data)
        {
            return new AccountsGroupDocument
                       {
                           Id = id,
                           AccountTypeDescription = data.AccountTypeDescription,
                           AccountTypeSymbol = data.AccountTypeSymbol,
                           AccountTypeAbbreviation = data.AccountTypeAbbreviation,
                           Accounts = data.Accounts == null ? null : data.Accounts.Select(a => ToDocument(_idGenerator.Generate(), a)).ToList(),
                       };
        }

        private static AccountDocument ToDocument(string id, AccountData data)
        {
            return new AccountDocument
                       {
                           Id = id,
                           AccountCondition = data.AccountCondition,
                           AccountConditionSymbol = data.AccountConditionSymbol,
                           AccountConditionRank = data.AccountConditionRank,
                           AccountDesignator = data.AccountDesignator,
                           AccountNumber = data.AccountNumber,
                           Bureau = data.Bureau,
                           CreditorName = data.CreditorName,
                           CurrentBalance = data.CurrentBalance,
                           AccountStatusDate = data.AccountStatusDate,
                           DateClosed = data.DateClosed,
                           DateOpened = data.DateOpened,
                           DateReported = data.DateReported,
                           DateVerified = data.DateVerified,
                           DisputeDescription = data.DisputeDescription,
                           HighBalance = data.HighBalance,
                           IndustryCodeDescription = data.IndustryCodeDescription,
                           IndustryCodeSymbol = data.IndustryCodeSymbol,
                           OpenClosedDescription = data.OpenClosedDescription,
                           OpenClosedSymbol = data.OpenClosedSymbol,
                           PayStatusDescription = data.PayStatusDescription,
                           PayStatusSymbol = data.PayStatusSymbol,
                           Remarks = data.Remarks,
                           SubscriberCode = data.SubscriberCode,
                           VerificationIndicator = data.VerificationIndicator,
                           AccountTypeAbbreviation = data.AccountTypeAbbreviation,
                           AccountTypeDescription = data.AccountTypeDescription,
                           AccountTypeSymbol = data.AccountTypeSymbol,
                           AmountPastDue = data.AmountPastDue,
                           Collateral = data.Collateral,
                           CreditLimit = data.CreditLimit,
                           CreditTypeAbbreviation = data.CreditTypeAbbreviation,
                           CreditTypeDescription = data.CreditTypeDescription,
                           CreditTypeSymbol = data.CreditTypeSymbol,
                           DateLastPayment = data.DateLastPayment,
                           DatePastDue = data.DatePastDue,
                           DateWorstPayStatus = data.DateWorstPayStatus,
                           Late30Count = data.Late30Count,
                           Late60Count = data.Late60Count,
                           Late90Count = data.Late90Count,
                           MonthlyPayment = data.MonthlyPayment,
                           MonthsReviewed = data.MonthsReviewed,
                           PayStartDate = data.PayStartDate,
                           PayStatus = data.PayStatus,
                           TermMonths = data.TermMonths,
                           WorstPayStatusCount = data.WorstPayStatusCount,
                           MonthlyPayStatus = data.MonthlyPayStatus == null ? null : data.MonthlyPayStatus.Select(ToDocument).ToList()


                       };
        }

        private static MonthlyPayStatusDocument ToDocument(MonthlyPayStatusData data)
        {
            if (data == null)
                return null;

            return new MonthlyPayStatusDocument()
            {

                Changed = data.Changed,
                Date = data.Date,
                Status = data.Status
            };

        }

        private static AddressDocument ToDocument(string id, AddressData data)
        {
            return new AddressDocument
                       {
                           Id = id,
                           HouseNumber = data.HouseNumber,
                           StreetName = data.StreetName,
                           StreetType = data.StreetType,
                           Unit = data.Unit,
                           City = data.City,
                           State = data.State,
                           PostalCode = data.PostalCode,
                           County = data.County,
                           Country = data.Country,
                       };
        }

        #endregion

        public void Handle(User_Subscription_DeletedEvent message)
        {
            if(!String.IsNullOrEmpty(message.CreditIdentityId))
            {

              var query = Query.EQ("_id", message.CreditIdentityId);

            _creditIdentityService.Remove(query);
           }
        }
    }
}
