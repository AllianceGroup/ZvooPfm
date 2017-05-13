using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace mPower.Documents.Documents.Credit.CreditIdentity
{
    public class CreditIdentityDocument
    {
        [BsonId]
        public string ClientKey { get; set; }

        public string UserId { get; set; }

        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string SocialSecurityNumber { get; set; }
        public string Suffix { get; set; }
        public int BirthYear { get; set; }
        public DateTime DateOfBirth { get; set; }

        public int QuestionaryAttempts { get; set; }
        public string AlertSubscriptionId { get; set; }

        public bool IsVerified { get; set; }
        public DateTime VerificationDate { get; set; }
        public string VerificationIpAddress { get; set; }

        public List<CreditReportDocument> CreditReports { get; set; }
        public List<CreditScoreDocument> CreditScores { get; set; }
        public List<VerificationQuestionDocument> Questions { get; set; }
        public List<AlertDocument> Alerts { get; set; }

        public EnrollDocument Enroll { get; set; }

        public bool IsEnrolled
        {
            get { return Enroll.IsEnrolled; }
        }

        public CreditIdentityDocument()
        {
            QuestionaryAttempts = 0;
            IsVerified = false;
            CreditReports = new List<CreditReportDocument>();
            CreditScores = new List<CreditScoreDocument>();
            Questions = new List<VerificationQuestionDocument>();
            Alerts = new List<AlertDocument>();
            Enroll = new EnrollDocument();
        }
    }
}
