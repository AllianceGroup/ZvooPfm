using System.Collections.Generic;
using mPower.Domain.Accounting.Enums;

namespace Default.ViewModel.Areas.Business.BusinessController
{
    public class Account
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Type { get; set; }

        public AccountTypeEnum TypeEnum { get; set; }

        public string Label { get; set; }

        public AccountLabelEnum LabelEnum { get; set; }

        public string Number { get; set; }

        public string Balance { get; set; }
        
        public string ParentAccountId { get; set; }

        public int Level { get; set; }

        public bool IsCoreAccount { get; set; }

        public int Order { get; set; }

        public List<Account> Children { get; set; }

        public Account()
        {
            Children = new List<Account>();
        }
    }
}