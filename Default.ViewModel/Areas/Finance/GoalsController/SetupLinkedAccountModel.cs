using System.Collections.Generic;

namespace Default.ViewModel.Areas.Finance.GoalsController
{
    public class SetupLinkedAccountModel
    {
        public List<GoalsLinkedAccount> Accounts { get; set; }

        public string LinkedAccountId { get; set; }

        public bool FromGoalCreationWizard { get; set; }


        public SetupLinkedAccountModel()
        {
            Accounts = new List<GoalsLinkedAccount>();
        }
    }

    public class GoalsLinkedAccount
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public long BalanceInCents { get; set; }

        public string Group { get; set; }
    }
}