using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Paralect.Domain;

namespace mPower.Domain.Membership.User.Commands
{
   public class User_Realestate_IncludeInWorthCommand : Command
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public bool IsIncludedInWorth { get; set; }
    }
}
