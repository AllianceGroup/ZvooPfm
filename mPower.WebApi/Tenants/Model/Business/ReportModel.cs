using mPower.Framework.Utils;

namespace mPower.WebApi.Tenants.Model.Business
{
    public class ReportModel
    {
        public string From { get; set; }

        public string To { get; set; }

        public DateRangeFormatEnum Format { get; set; }

        public int Dates { get; set; }

        public ReportModel()
        {
            Format = DateRangeFormatEnum.Total;
            Dates = 1;
        }
    }
}
