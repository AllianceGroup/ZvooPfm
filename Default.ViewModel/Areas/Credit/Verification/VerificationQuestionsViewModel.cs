using System.Collections.Generic;

namespace Default.ViewModel.Areas.Credit.Verification
{
    public class VerificationQuestionsViewModel
    {
        public string ClientKey { get; set; }

        public List<VerificationQuestion> Questions { get; set; }

        public VerificationQuestionsViewModel()
        {
            Questions = new List<VerificationQuestion>();
        }
       
    }
}
