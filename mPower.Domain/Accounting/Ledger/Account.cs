using System;
using mPower.Domain.Accounting.Enums;

namespace mPower.Domain.Accounting.Ledger
{
    public class Account
    {
        public String Id { get; set; }
        public AccountTypeEnum TypeEnum { get; set; }
        public Int64 CurrentBalance { get; set; }
        public AccountLabelEnum LabelEnum { get; set; }
        public String Name { get; set; }
        public String ParentId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Account(string id, AccountTypeEnum typeEnum, AccountLabelEnum labelEnum, string name, string parentId)
        {
            Id = id;
            TypeEnum = typeEnum;
            LabelEnum = labelEnum;
            Name = name;
            ParentId = parentId;
        }

        /// <summary>
        /// for object reconstruction
        /// </summary>
        public Account() { }

    }
}