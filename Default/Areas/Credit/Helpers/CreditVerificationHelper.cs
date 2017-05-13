using System.Collections.Generic;
using System.Linq;
using Default.ViewModel.Areas.Credit.Verification;
using mPower.Documents.Documents.Credit.CreditIdentity;

namespace Default.Areas.Credit.Helpers {
	public class CreditVerificationHelper {
		public static bool VerifyCreditQuestions(string clientKey, IEnumerable<VerificationQuestionDocument> questions, IEnumerable<QuestionaireAnswer> answers = null) {
			bool valid = UserAnsweredQuestions(questions, answers) && UserAnswersSameCountAsAnswers(questions, answers);

			if(valid) {
				using(var userAnswerEnumerator = answers.GetEnumerator()) {
					while(valid && userAnswerEnumerator.MoveNext()) {
						var userAnswer = userAnswerEnumerator.Current;
						var question = questions.SingleOrDefault(x => x.Id == userAnswer.QuestionId);

						if(question != null && valid) {
							var answersForQuestion = question.Answers.Where(x => userAnswer.Answers.Contains(x.Answer));
							var correctAnswersCount = question.Answers.Where(x => x.IsCorrect).Count();

							valid = (answersForQuestion != null && answersForQuestion.Count() == correctAnswersCount);
							using(var userAnswersEnumerator = answersForQuestion.GetEnumerator()) {
								while(valid && userAnswersEnumerator.MoveNext()) {
									var answer = userAnswersEnumerator.Current;

									valid = (answer != null && answer.IsCorrect);
								}
							}
						} else {
							valid = false;
						}
					}
				}
			}

			return valid;
		}

		private static bool UserAnswersSameCountAsAnswers(IEnumerable<VerificationQuestionDocument> questions, IEnumerable<QuestionaireAnswer> answers) {
			int userAnswersCount = answers.Select(x => x.Answers.Count).Sum();
			int realAnswersCount = questions.Select(x => x.Answers.Where(y => y.IsCorrect == true).Count()).Sum();

			return userAnswersCount == realAnswersCount;
		}

		private static bool UserAnsweredQuestions(IEnumerable<VerificationQuestionDocument> questions, IEnumerable<QuestionaireAnswer> answers) {
			bool valid = false;

			if(questions != null && answers != null) {
				foreach(var item in answers) {
					valid = (item.Answers != null || item.Answers.Count > 0);

					if(!valid) {
						break;
					}
				}
			}

			return valid;
		}
	}
}
