using System.Collections.Generic;

namespace Default.ViewModel.Areas.Finance.DebtToolsController
{
    public class ChartViewModel
    {
        public List<dynamic> Data { get; set; }
        public List<string> Labels { get; set; }
        public string XKey { get; set; }
        public List<string> YKeys { get; set; }
        public bool gridLines { get; set; }
    }

}