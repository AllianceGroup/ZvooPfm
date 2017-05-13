﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Paralect.Domain;

namespace mPower.Domain.Membership.User.Events
{
    public class User_Realestate_IncludeInWorthEvent : Event
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public bool IsIncludedInWorth { get; set; }
    }
}
