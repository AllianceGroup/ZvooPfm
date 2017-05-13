using System.Collections.Generic;
using mPower.Domain.Membership.User.Commands;
using mPower.Domain.Membership.User.Events;
using NUnit.Framework;

namespace mPower.Tests.UnitTests.Domain.Membership.User
{
    public class user_update_security_question_test : UserTest
    {
        private const string question = "City where your mother was born?";
        private const string answer = "Minsk";

        public override IEnumerable<Paralect.Domain.IEvent> Given()
        {
            yield return User_Created();
        }

        public override IEnumerable<Paralect.Domain.ICommand> When()
        {
            yield return new User_UpdateSecurityQuestionCommand()
            {
                Answer = answer,
                Question = question,
                UserId = _id
            };
        }

        public override IEnumerable<Paralect.Domain.IEvent> Expected()
        {
            yield return new User_UpdatedSecurityQuestionEvent()
            {
                Answer = answer,
                Question = question,
                UserId = _id
            };
        }

        [Test]
        public void Test()
        {
            Validate();

            DispatchEvents(() =>
            {
                var user = _userDocumentService.GetById(_id);
                Assert.AreEqual(user.PasswordAnswer, answer);
                Assert.AreEqual(user.PasswordQuestion, question);
            });
        }
    }
}
