namespace Default.ViewModel.Areas.Credit.IdGuard
{
	public class CreditAlertSignUp
	{
		public int LowScore { get; set; }
		public bool LowEmail { get; set; }
		public bool LowText { get; set; }
		public bool LowRss { get; set; }
		public int HighScore { get; set; }
		public bool HighEmail { get; set; }
		public bool HighText { get; set; }
		public bool HighRss { get; set; }
	}
}