using System;
using System.Collections.Generic;
using Paralect.Domain;
using mPower.Domain.Accounting.CreditIdentity.Data;
using mPower.Domain.Accounting.CreditIdentity.Events;
using mPower.Framework;

namespace mPower.Domain.Accounting.CreditIdentity
{
    public class CreditIdentityAR : MpowerAggregateRoot
    {
        public string UserId;
        public string UserFullName;
        public string SocialSecurityNumber;

        /// <summary>
        /// For object reconstruction
        /// </summary>
        public CreditIdentityAR() { }

        public CreditIdentityAR(string userId, CreditIdentityData data, ICommandMetadata metadata)
        {
            SetCommandMetadata(metadata);
            Apply(new CreditIdentity_CreatedEvent
                      {
                          UserId = userId,
                          Data = data,
                      });
        }

        public void Delete()
        {
            Apply(new CreditIdentity_DeletedEvent
                      {
                          ClientKey = _id,
                      });
        }

        public void AddReport(string reportId, string scoreId, CreditReportData data)
        {
            Apply(new CreditIdentity_Report_AddedEvent
                      {
                          UserId = UserId,
                          UserFullName = UserFullName,
                          ClientKey = _id,
                          CreditReportId = reportId,
                          CreditScoreId = scoreId,
                          SocialSecurityNumber = SocialSecurityNumber,
                          Data = data,
                      });
        }

        public void SetQuestions(List<VerificationQuestionData> questions)
        {
            Apply(new CreditIdentity_Questions_SetEvent
                      {
                          ClientKey = _id,
                          Questions = questions,
                      });
        }

        public void AddAlerts(List<AlertData> alerts)
        {
            Apply(new CreditIdentity_Alerts_AddedEvent
                      {
                          ClientKey = _id,
                          Alerts = alerts
                      });
        }

        public void Enroll(string activationCode, string memberId, string salesId)
        {
            Apply(new CreditIdentity_EnrolledEvent()
            {
                CreditIdentityId = _id,
                ActivationCode = activationCode,
                MemberId = memberId,
                SalesId = salesId
            });
        }

        public void CancelEnroll()
        {
            Apply(new CreditIdentity_CanceledEnrollEvent()
            {
                CreditIdentityId = _id
            });
        }

        public void MarkAsVerified(DateTime date, string ipAddress)
        {
            Apply(new CreditIdentity_MarkedAsVerifiedEvent
                      {
                          ClientKey = _id,
                          Date = date,
                          IpAddress = ipAddress,
                      });
        }

        #region Object Reconstruction

        protected void On(CreditIdentity_CreatedEvent created)
        {
            _id = created.Data.ClientKey;
            UserId = created.UserId;
            UserFullName = GetFullName(created.Data.FirstName, created.Data.MiddleName, created.Data.LastName);
            SocialSecurityNumber = created.Data.SocialSecurityNumber;
        }

        #endregion

        private string GetFullName(string firstName, string middleName, string lastName)
        {
            var fullName = string.Empty;

            if (!string.IsNullOrEmpty(firstName))
            {
                fullName += firstName;
            }

            if (!string.IsNullOrEmpty(middleName))
            {
                fullName += " " + middleName;
            }

            if (!string.IsNullOrEmpty(lastName))
            {
                fullName += " " + lastName;
            }

            return fullName.Trim();
        }
    }
}
