using System.Collections.Generic;
using System.Linq;
using mPower.Domain.Accounting.CreditIdentity.Commands;
using mPower.Domain.Accounting.CreditIdentity.Data;
using mPower.Framework.Mvc;
using TransUnionWrapper.Model;

namespace Default.Factories.Commands.Credit
{
    public class CreditIdentity_Questions_SetCommandFactory : IObjectFactory<IEnumerable<CreditIdentityVerificationQuestion>, CreditIdentity_Questions_SetCommand>
    {
        public CreditIdentity_Questions_SetCommand Load(IEnumerable<CreditIdentityVerificationQuestion> questions)
        {
            return new CreditIdentity_Questions_SetCommand
            {
                Questions = questions.Select(q =>
                new VerificationQuestionData
                {
                    Answers = q.Answers.Select(a =>
                                    new VerificationAnswerData
                                    {
                                        IsCorrect = a.IsCorrect,
                                        Answer = a.Answer,
                                        SequenceNumber = a.SequenceNumber,
                                    }).ToList(),
                    IsFakeQuestion = q.IsFakeQuestion,
                    IsLastChanceQuestion =
                        q.IsLastChanceQuestion,
                    QuestionType = q.QuestionType,
                    SequenceNumber = q.SequenceNumber,
                    Question = q.Question,
                    Id = q.Id,

                }).ToList(),

            };
        }
    }
}