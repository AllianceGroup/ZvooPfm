using System;
using System.Collections.Generic;
using mPower.Domain.Membership.Enums;

namespace mPower.Domain.Membership.User
{
    public class PermissionCollection
    {
        public PermissionCollection()
        {
            Permissions = new Dictionary<int, UserPermissionEnum>();
        }

        public Dictionary<int, UserPermissionEnum> Permissions { get; set; }

        public Int32 Count
        {
            get { return Permissions.Count; }
        }

        public void Add(UserPermissionEnum permission)
        {
            Permissions.Add((int)permission, permission);
        }

        public void Remove(UserPermissionEnum permission)
        {
            Permissions.Remove((int)permission);
        }

        public bool HasPermission(UserPermissionEnum permission)
        {
            return Permissions.ContainsKey((int)permission);
        }
    }
}
