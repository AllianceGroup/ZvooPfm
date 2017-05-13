using System.Collections;
using System.Collections.Generic;

namespace Default.ViewModel.GettingStartedController
{
    public class SearchInstitutionsResultPartialViewModel
    {
        public IEnumerable<InstitutionModel> ContentServices { get; set; }
        public string PostUrl { get; set; }
        
    }
}