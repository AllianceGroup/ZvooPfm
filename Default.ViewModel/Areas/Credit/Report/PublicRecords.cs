using System;

namespace Default.ViewModel.Areas.Credit.Report
{
    public class PublicRecord
    {
        public DateTime DateFiled { get; set; }
        public DateTime DateVerified { get; set; }
        public string Status { get; set; }
        public string Classification { get; set; }
        public string CourtName { get; set; }
        public string DesignatorDescrip { get; set; }
        public string Bureau { get; set; }
        public string RefNumber { get; set; }
        public string Notes { get; set; }
        public string Industry { get; set; }
        public double LiabilityAmt { get; set; }
        public double AssetAmt { get; set; }
        public double ExemptAmt { get; set; }
        public DateTime DateResolved { get; set; }
    }
}
