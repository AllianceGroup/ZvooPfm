using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mPower.Domain.Membership.User;
using mPower.Tests.Environment;
using mPower.Yodlee.Domain.User;
using mPower.Yodlee.Domain.User.Commands;
using mPower.Yodlee.Domain.User.Events;
using Paralect.Domain;

namespace mPower.Accounting.Tests.UnitTests.Yodlee.User
{
    public abstract class UserTest : AggregateTest<UserAR>
    {

        private string UserId = "106cc856-a26b-44c8-bdff-306a9828e2e7";
        private string Username = "brettallredtestuser";
        private string Password = "Password1234";
        private string Email = "brettallredTestAcct@gmail.com";
        private DateTime _currentDate;

        protected UserTest()
        {
            _currentDate = DateTime.Now;
        }

        public ICommand User_Create()
        {
            return new YodleeUser_CreateCommand()
                       {
                           Email = Email,
                           PasswordHash = Password,
                           UserId = UserId,
                           UserName = Username,
                           CreateDate = _currentDate,
                           IsActive = true
                       };
        }

        public IEvent User_Created()
        {
            return new YodleeUser_CreatedEvent()
            {
                Email = Email,
                PasswordHash = Password,
                UserId = UserId,
                UserName = Username,
                CreateDate = _currentDate,
                IsActive = true
            };
        }

    }
}
