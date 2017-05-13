using System.Collections.Generic;
using System.Linq;
using mPower.Domain.Accounting.CreditIdentity.Commands;
using mPower.Domain.Accounting.CreditIdentity.Data;
using mPower.Framework.Environment;
using mPower.Framework.Mvc;
using TransUnionWrapper.Model;

namespace Default.Factories.Commands.Credit
{
    public class CreditIdentity_Report_AddCommandFactory : 
        IObjectFactory<CreditReport, CreditIdentity_Report_AddCommand>
    {
        private readonly IIdGenerator _idGenerator;

        public CreditIdentity_Report_AddCommandFactory(IIdGenerator idGenerator)
        {
            _idGenerator = idGenerator;
        }

        public CreditIdentity_Report_AddCommand Load(CreditReport report)
        {
            var cmd = new CreditIdentity_Report_AddCommand
            {
                CreditReportId = _idGenerator.Generate(),
                CreditScoreId = _idGenerator.Generate(),
                Data = new CreditReportData
                {
                    Borrowers =
                        report.Borrowers == null
                            ? new List<BorrowerData>()
                            : report.Borrowers.Select(Map).ToList(),
                    Inquiries =
                        report.Inquiries == null
                            ? new List<InquiryData>()
                            : report.Inquiries.Select(Map).ToList(),
                    Messages =
                        report.Messages == null
                            ? new List<MessageData>()
                            : report.Messages.Select(Map).ToList(),
                    PublicRecords =
                        report.PublicRecords == null
                            ? new List<PublicRecordData>()
                            : report.PublicRecords.Select(Map).ToList(),
                    AccountsGroups =
                        report.TradeLinePartitions == null
                            ? new List<AccountsGroupData>()
                            : report.TradeLinePartitions.Select(Map).ToList(),
                    Creditors =
                        report.Subscribers == null
                            ? new List<CreditorData>()
                            : report.Subscribers.Select(Map).ToList(),
                    BureauSource = report.BureauSource,
                    Score = report.Score,
                    ClosedAccountsCount = report.ClosedAccountsCount,
                    CurrentVersion = report.CurrentVersion,
                    DeliquentAccountsCount = report.DeliquentAccountsCount,
                    DerogatoryAccountsCount = report.DerogatoryAccountsCount,
                    IsDeceased = report.IsDeceased,
                    IsFraud = report.IsFraud,
                    IsFrozen = report.IsFrozen,
                    OpenAccountsCount = report.OpenAccountsCount,
                    PopulationRank = report.PopulationRank,
                    PublicRecordCount = report.PublicRecordCount,
                    QualitativeRank = report.QualitativeRank,
                    SafetyCheckPassed = report.SafetyCheckPassed,
                    ScoreDate = report.ScoreDate,
                    Status = report.Status,
                    TotalAccountsCount = report.TotalAccountsCount,
                    TotalBalances = report.TotalBalances,
                    TotalMonthlyPayments = report.TotalMonthlyPayments,
                    InquiriesInLastTwoYears = report.InquiriesInLastTwoYears,
                    NegativeFactors = report.NegativeFators,
                    ReportData = report.CreditReportData
                }
            };
            
            return cmd;
        }

        #region Object Mapping

        private static BorrowerData Map(Borrower data)
        {
            if (data == null)
                return null;

            return new BorrowerData
            {
                Addresses = data.Addresses == null ? null : data.Addresses.Select(Map).ToList(),
                BirthDates = data.BirthDates == null ? null : data.BirthDates.Select(Map).ToList(),
                CreditScores = data.CreditScores == null ? null : data.CreditScores.Select(Map).ToList(),
                CreditStatements =
                    data.CreditStatements == null ? null : data.CreditStatements.Select(Map).ToList(),
                Employers = data.Employers == null ? null : data.Employers.Select(Map).ToList(),
                Names = data.Names == null ? null : data.Names.Select(Map).ToList(),
                PreviousAddresses =
                    data.PreviousAddresses == null ? null : data.PreviousAddresses.Select(Map).ToList(),
                SocialSecurityNumber = data.SocialSecurityNumber,
                SocialSecurityNumbers =
                    data.SocialSecurityNumbers == null
                        ? null
                        : data.SocialSecurityNumbers.Select(Map).ToList(),
                Telephones = data.Telephones == null ? null : data.Telephones.Select(Map).ToList()
            };
        }

        private static AddressData Map(Address data)
        {
            if (data == null)
                return null;

            return new AddressData
            {
                City = data.City,
                Country = data.Country,
                County = data.County,
                HouseNumber = data.HouseNumber,
                PostalCode = data.PostalCode,
                State = data.State,
                StreetName = data.StreetName,
                StreetType = data.StreetType,
                Unit = data.Unit,
            };
        }

        private static BorrowerBirthDateData Map(BorrowerBirthDate data)
        {
            if (data == null)
                return null;

            return new BorrowerBirthDateData
            {
                BirthYear = data.BirthYear,
                Age = data.Age,
                BirthDate = data.BirthDate,
                BirthDay = data.BirthDay,
                BirthMonth = data.BirthMonth
            };
        }

        private static BorrowerCreditScoreData Map(BorrowerCreditScore data)
        {
            if (data == null)
                return null;

            return new BorrowerCreditScoreData
            {
                Bureau = data.Bureau,
                InquiryDate = data.InquiryDate,
                Score = data.Score,
                CreditScoreFactors =
                    data.CreditScoreFactors == null ? null : data.CreditScoreFactors.Select(Map).ToList(),
            };
        }

        private static BorrowerCreditScoreFactorData Map(BorrowerCreditScoreFactor data)
        {
            if (data == null)
                return null;

            return new BorrowerCreditScoreFactorData
            {
                Bureau = data.Bureau,
                FactorAbbreviation = data.FactorAbbreviation,
                FactorDescription = data.FactorDescription,
                FactorRank = data.FactorRank,
                FactorSymbol = data.FactorSymbol
            };
        }

        private static BorrowerCreditStatementData Map(BorrowerCreditStatement data)
        {
            if (data == null)
                return null;

            return new BorrowerCreditStatementData
            {
                Bureau = data.Bureau,
                InquiryDate = data.InquiryDate,
                Statement = data.Statement,
                StatementTypeAbbreviation = data.StatementTypeAbbreviation,
                StatementTypeDescription = data.StatementTypeDescription,
                StatementTypeRank = data.StatementTypeRank,
                StatementTypeSymbol = data.StatementTypeSymbol,
            };
        }

        private static BorrowerEmployerData Map(BorrowerEmployer data)
        {
            if (data == null)
                return null;

            return new BorrowerEmployerData
            {
                Address = Map(data.Address),
                DateReported = data.DateReported,
                DateUpdated = data.DateUpdated,
                Name = data.Name
            };
        }

        private static BorrowerNameData Map(BorrowerName data)
        {
            if (data == null)
                return null;

            return new BorrowerNameData
            {
                Bureau = data.Bureau,
                Description = data.Description,
                FirstName = data.FirstName,
                MiddleName = data.MiddleName,
                LastName = data.LastName,
                InquiryDate = data.InquiryDate,
                Suffix = data.Suffix,
                Prefix = data.Prefix,
                Reference = data.Reference
            };
        }

        private static BorrowerSocialData Map(BorrowerSocial data)
        {
            if (data == null)
                return null;

            return new BorrowerSocialData
            {
                Bureau = data.Bureau,
                InquiryDate = data.InquiryDate,
                SocialSecurityNumber = data.SocialSecurityNumber
            };
        }

        private static BorrowerTelephoneData Map(BorrowerTelephone data)
        {
            if (data == null)
                return null;

            return new BorrowerTelephoneData
            {
                AreaCode = data.AreaCode,
                Extension = data.Extension,
                Number = data.Number
            };
        }

        private static InquiryData Map(Inquiry data)
        {
            if (data == null)
                return null;

            return new InquiryData
            {
                Bureau = data.Bureau,
                InquiryDate = data.InquiryDate,
                IndustryCodeAbbreviation = data.IndustryCodeAbbreviation,
                IndustryCodeDescription = data.IndustryCodeDescription,
                IndustryCodeRank = data.IndustryCodeRank,
                InquiryType = data.InquiryType,
                SubscriberName = data.SubscriberName,
                SubscriberNumber = data.SubscriberNumber
            };
        }

        private static MessageData Map(Message data)
        {
            if (data == null)
                return null;

            return new MessageData
            {
                CodeDescription = data.CodeDescription,
                CodeSymbol = data.CodeSymbol,
                Rank = data.Rank,
                Text = data.Text,
                TypeDescription = data.TypeDescription,
                TypeSymbol = data.TypeSymbol,
            };
        }

        private static PublicRecordData Map(PublicRecord data)
        {
            if (data == null)
                return null;

            return new PublicRecordData
            {
                Bureau = data.Bureau,
                IndustryCodeDescription = data.IndustryCodeDescription,
                Status = data.Status,
                ClassificationDescription = data.ClassificationDescription,
                ClassificationRank = data.ClassificationRank,
                ClassificationSymbol = data.ClassificationSymbol,
                CourtName = data.CourtName,
                CustomRemarks = data.CustomRemarks,
                DateExpires = data.DateExpires,
                DateFiled = data.DateFiled,
                DateVerified = data.DateVerified,
                DesignatorDescription = data.DesignatorDescription,
                IndustryCodeSymbol = data.IndustryCodeSymbol,
                IndustryRank = data.IndustryRank,
                Item = data.Item,
                ReferenceNumber = data.ReferenceNumber,
                SubscriberCode = data.SubscriberCode,
                Type = data.Type
            };
        }

        private static AccountsGroupData Map(TradeLinePartition data)
        {
            if (data == null)
                return null;

            return new AccountsGroupData
            {
                AccountTypeAbbreviation = data.AccountTypeAbbreviation,
                AccountTypeDescription = data.AccountTypeDescription,
                AccountTypeSymbol = data.AccountTypeSymbol,
                Accounts = data.TradeLines == null ? null : data.TradeLines.Select(Map).ToList()
            };
        }

        private static AccountData Map(TradeLine data)
        {
            if (data == null)
                return null;

            return new AccountData
            {
                AccountCondition = data.AccountCondition,
                AccountConditionRank = data.AccountConditionRank,
                AccountConditionSymbol = data.AccountConditionSymbol,
                AccountDesignator = data.AccountDesignator,
                AccountNumber = data.AccountNumber,
                AccountStatusDate = data.AccountStatusDate,
                Bureau = data.Bureau,
                CreditorName = data.CreditorName,
                CurrentBalance = data.CurrentBalance,
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
                MonthlyPayStatus = data.MonthlyPayStatus == null ? null : data.MonthlyPayStatus.Select(Map).ToList()
            };
        }

        private static MonthlyPayStatusData Map(MonthlyPayStatus data)
        {
            if (data == null)
                return null;

            return new MonthlyPayStatusData
            {
                Changed = data.Changed,
                Date = data.Date,
                Status = data.Status
            };
        }

        private static CreditorData Map(Subscriber data)
        {
            if (data == null)
                return null;

            return new CreditorData
            {
                Address = Map(data.Address),
                Bureau = data.Bureau,
                Code = data.Code,
                IndustryCodeDescription = data.IndustryCodeDescription,
                IndustryCodeRank = data.IndustryCodeRank,
                IndustryCodeSymbol = data.IndustryCodeSymbol,
                InquiryDate = data.InquiryDate,
                Name = data.Name,
                Telephone = data.Telephone
            };
        }

        #endregion
    }
}