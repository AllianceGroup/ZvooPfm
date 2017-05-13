namespace Default.ViewModel.Areas.Credit.Verification
{
    public class VerificationAnswer
    {
        public string QuestionId { get; set; }
        public bool IsCorrect { get; set; }
        public string Answer { get; set; }
        public int SequenceNumber { get; set; }
		public string ElementId { get; set; }
    }
}