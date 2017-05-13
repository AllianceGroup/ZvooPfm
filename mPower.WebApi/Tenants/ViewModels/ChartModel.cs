using System.Collections.Generic;

namespace mPower.WebApi.Tenants.ViewModels
{
    public class ChartModel
    {
        public List<long> Data { get; set; }
        public List<string> Labels { get; set; }
        public List<string> Series { get; set; }

        public ChartModel()
        {
            Data = new List<long>();
            Labels = new List<string>();
            Series = new List<string>();
        }
    }
}
