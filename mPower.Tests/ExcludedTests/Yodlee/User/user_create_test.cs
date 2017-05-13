using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Paralect.Domain;

namespace mPower.Accounting.Tests.UnitTests.Yodlee.User
{
    public class user_create_test : UserTest
    {
        public override IEnumerable<IEvent> Given()
        {
            yield break;
        }

        public override IEnumerable<ICommand> When()
        {
            yield return User_Create();
        }
        
        public override IEnumerable<IEvent> Expected()
        {
            yield return User_Created();
        }

        

        [Test]
        public void Test()
        {
            Validate();

        }
    }
}
