using System.Collections.Generic;
using Default.Areas.Credit.Helpers;
using NUnit.Framework;
using mPower.Documents.Documents.Credit.CreditIdentity;
using Default.ViewModel.Areas.Credit.Verification;

namespace mPower.Tests.UnitTests.Services {

	[TestFixture]
	public class VerificationServiceTests {

		[Test]
		public void VerifyCreditQuestions_UsingSingleAnswerQuestionsAllQuestionsCorrect_ReturnsTrue() {
			#region Questions
			List<VerificationQuestionDocument> questions = new List<VerificationQuestionDocument> { 
				new VerificationQuestionDocument {
					Id = "q1",
					IsFakeQuestion = false,
					IsLastChanceQuestion = false,
					QuestionType = "test question",
					SequenceNumber = 0,
					Question = "here's a question 1",
					Answers = new List<VerificationAnswerDocument> {
						new VerificationAnswerDocument {
							Answer = "wrong 1",
							Id = "1",
							IsCorrect = false,
							SequenceNumber = 0
						},
						new VerificationAnswerDocument {
							Answer = "correct",
							Id = "2",
							IsCorrect = true,
							SequenceNumber = 1
						},
						new VerificationAnswerDocument {
							Answer = "wrong 2",
							Id = "3",
							IsCorrect = false,
							SequenceNumber = 2
						},
						new VerificationAnswerDocument {
							Answer = "wrong 4",
							Id = "4",
							IsCorrect = false,
							SequenceNumber = 3
						},
					}
				},
				new VerificationQuestionDocument {
					Id = "q2",
					IsFakeQuestion = false,
					IsLastChanceQuestion = false,
					QuestionType = "test question",
					SequenceNumber = 1,
					Question = "here's a question 2",
					Answers = new List<VerificationAnswerDocument> {
						new VerificationAnswerDocument {
							Answer = "wrong 1",
							Id = "1",
							IsCorrect = false,
							SequenceNumber = 0
						},
						new VerificationAnswerDocument {
							Answer = "wrong 2",
							Id = "2",
							IsCorrect = false,
							SequenceNumber = 1
						},
						new VerificationAnswerDocument {
							Answer = "wrong 3",
							Id = "3",
							IsCorrect = false,
							SequenceNumber = 2
						},
						new VerificationAnswerDocument {
							Answer = "correct",
							Id = "4",
							IsCorrect = true,
							SequenceNumber = 3
						},
					}
				}
			};
			#endregion

			#region Answers

			List<QuestionaireAnswer> answers = new List<QuestionaireAnswer> { 
				new QuestionaireAnswer {
					Answers = new List<string> { "correct" },
					QuestionId = "q2"
				},
				new QuestionaireAnswer {
					Answers = new List<string> { "correct" },
					QuestionId = "q1"
				}
			};

			#endregion

			bool result = CreditVerificationHelper.VerifyCreditQuestions("", questions, answers);

			Assert.IsTrue(result);
		}

		[Test]
		public void VerifyCreditQuestions_UsingMultipleAnswerQuestionsAllQuestionsCorrect_ReturnsTrue() {
			#region Questions
			List<VerificationQuestionDocument> questions = new List<VerificationQuestionDocument> { 
				new VerificationQuestionDocument {
					Id = "q1",
					IsFakeQuestion = false,
					IsLastChanceQuestion = false,
					QuestionType = "test question",
					SequenceNumber = 0,
					Question = "here's a question 1",
					Answers = new List<VerificationAnswerDocument> {
						new VerificationAnswerDocument {
							Answer = "also correct",
							Id = "1",
							IsCorrect = true,
							SequenceNumber = 0
						},
						new VerificationAnswerDocument {
							Answer = "correct",
							Id = "2",
							IsCorrect = true,
							SequenceNumber = 1
						},
						new VerificationAnswerDocument {
							Answer = "wrong 1",
							Id = "3",
							IsCorrect = false,
							SequenceNumber = 2
						},
						new VerificationAnswerDocument {
							Answer = "wrong 2",
							Id = "4",
							IsCorrect = false,
							SequenceNumber = 3
						},
					}
				},
				new VerificationQuestionDocument {
					Id = "q2",
					IsFakeQuestion = false,
					IsLastChanceQuestion = false,
					QuestionType = "test question",
					SequenceNumber = 1,
					Question = "here's a question 2",
					Answers = new List<VerificationAnswerDocument> {
						new VerificationAnswerDocument {
							Answer = "wrong 1",
							Id = "1",
							IsCorrect = false,
							SequenceNumber = 0
						},
						new VerificationAnswerDocument {
							Answer = "wrong 2",
							Id = "2",
							IsCorrect = false,
							SequenceNumber = 1
						},
						new VerificationAnswerDocument {
							Answer = "correct as well",
							Id = "3",
							IsCorrect = true,
							SequenceNumber = 2
						},
						new VerificationAnswerDocument {
							Answer = "correct",
							Id = "4",
							IsCorrect = true,
							SequenceNumber = 3
						},
					}
				}
			};
			#endregion

			#region Answers

			List<QuestionaireAnswer> answers = new List<QuestionaireAnswer> { 
				new QuestionaireAnswer {
					Answers = new List<string> { 
						"correct",
						"correct as well"
					},
					QuestionId = "q2"
				},
				new QuestionaireAnswer {
					Answers = new List<string> { 
						"correct",
						"also correct"
					},
					QuestionId = "q1"
				}
			};

			#endregion

			bool result = CreditVerificationHelper.VerifyCreditQuestions("", questions, answers);

			Assert.IsTrue(result);
		}

		[Test]
		public void VerifyCreditQuestions_UsingMultipleAndSingleAnswerQuestionsAllQuestionsCorrect_ReturnsTrue() {
			#region Questions
			List<VerificationQuestionDocument> questions = new List<VerificationQuestionDocument> { 
				new VerificationQuestionDocument {
					Id = "q1",
					IsFakeQuestion = false,
					IsLastChanceQuestion = false,
					QuestionType = "test question",
					SequenceNumber = 0,
					Question = "here's a question 1",
					Answers = new List<VerificationAnswerDocument> {
						new VerificationAnswerDocument {
							Answer = "also correct",
							Id = "1",
							IsCorrect = true,
							SequenceNumber = 0
						},
						new VerificationAnswerDocument {
							Answer = "correct",
							Id = "2",
							IsCorrect = true,
							SequenceNumber = 1
						},
						new VerificationAnswerDocument {
							Answer = "wrong 1",
							Id = "3",
							IsCorrect = false,
							SequenceNumber = 2
						},
						new VerificationAnswerDocument {
							Answer = "wrong 2",
							Id = "4",
							IsCorrect = false,
							SequenceNumber = 3
						},
					}
				},
				new VerificationQuestionDocument {
					Id = "q2",
					IsFakeQuestion = false,
					IsLastChanceQuestion = false,
					QuestionType = "test question",
					SequenceNumber = 1,
					Question = "here's a question 2",
					Answers = new List<VerificationAnswerDocument> {
						new VerificationAnswerDocument {
							Answer = "wrong 1",
							Id = "1",
							IsCorrect = false,
							SequenceNumber = 0
						},
						new VerificationAnswerDocument {
							Answer = "wrong 2",
							Id = "2",
							IsCorrect = false,
							SequenceNumber = 1
						},
						new VerificationAnswerDocument {
							Answer = "correct as well",
							Id = "3",
							IsCorrect = true,
							SequenceNumber = 2
						},
						new VerificationAnswerDocument {
							Answer = "correct",
							Id = "4",
							IsCorrect = true,
							SequenceNumber = 3
						},
					}
				},
				new VerificationQuestionDocument {
					Id = "q3",
					IsFakeQuestion = false,
					IsLastChanceQuestion = false,
					QuestionType = "test question",
					SequenceNumber = 2,
					Question = "single answer question",
					Answers = new List<VerificationAnswerDocument> {
						new VerificationAnswerDocument {
							Answer = "this is correct",
							Id = "1",
							IsCorrect = true,
							SequenceNumber = 0
						},
						new VerificationAnswerDocument {
							Answer = "wrong 1",
							Id = "2",
							IsCorrect = false,
							SequenceNumber = 1
						},
						new VerificationAnswerDocument {
							Answer = "wrong 2",
							Id = "3",
							IsCorrect = false,
							SequenceNumber = 2
						},
						new VerificationAnswerDocument {
							Answer = "wrong 3",
							Id = "4",
							IsCorrect = false,
							SequenceNumber = 3
						},
					}
				}
			};
			#endregion

			#region Answers

			List<QuestionaireAnswer> answers = new List<QuestionaireAnswer> { 
				new QuestionaireAnswer {
					Answers = new List<string> { 
						"correct",
						"correct as well"
					},
					QuestionId = "q2"
				},
				new QuestionaireAnswer {
					Answers = new List<string> { 
						"correct",
						"also correct"
					},
					QuestionId = "q1"
				},
				new QuestionaireAnswer {
					Answers = new List<string> {
						"this is correct"
					},
					QuestionId = "q3"
				}
			};

			#endregion

			bool result = CreditVerificationHelper.VerifyCreditQuestions("", questions, answers);

			Assert.IsTrue(result);
		}

		[Test]
		public void VerifyCreditQuestions_UserAnswersNoQuestion_ReturnsFalse() {
			#region Questions
			List<VerificationQuestionDocument> questions = new List<VerificationQuestionDocument> { 
				new VerificationQuestionDocument {
					Id = "q1",
					IsFakeQuestion = false,
					IsLastChanceQuestion = false,
					QuestionType = "test question",
					SequenceNumber = 0,
					Question = "here's a question 1",
					Answers = new List<VerificationAnswerDocument> {
						new VerificationAnswerDocument {
							Answer = "also correct",
							Id = "1",
							IsCorrect = true,
							SequenceNumber = 0
						},
						new VerificationAnswerDocument {
							Answer = "correct",
							Id = "2",
							IsCorrect = true,
							SequenceNumber = 1
						},
						new VerificationAnswerDocument {
							Answer = "wrong 1",
							Id = "3",
							IsCorrect = false,
							SequenceNumber = 2
						},
						new VerificationAnswerDocument {
							Answer = "wrong 2",
							Id = "4",
							IsCorrect = false,
							SequenceNumber = 3
						},
					}
				}
			};
			#endregion

			#region Answers
			List<QuestionaireAnswer> answers = new List<QuestionaireAnswer>();
			#endregion

			bool result = CreditVerificationHelper.VerifyCreditQuestions("", questions, answers);

			Assert.IsFalse(result);
		}

		[Test]
		public void VerifyCreditQuestions_AnswersAreNull_ReturnsFalse() {
			#region Questions
			List<VerificationQuestionDocument> questions = new List<VerificationQuestionDocument> { 
				new VerificationQuestionDocument {
					Id = "q1",
					IsFakeQuestion = false,
					IsLastChanceQuestion = false,
					QuestionType = "test question",
					SequenceNumber = 0,
					Question = "here's a question 1",
					Answers = new List<VerificationAnswerDocument> {
						new VerificationAnswerDocument {
							Answer = "also correct",
							Id = "1",
							IsCorrect = true,
							SequenceNumber = 0
						},
						new VerificationAnswerDocument {
							Answer = "correct",
							Id = "2",
							IsCorrect = true,
							SequenceNumber = 1
						},
						new VerificationAnswerDocument {
							Answer = "wrong 1",
							Id = "3",
							IsCorrect = false,
							SequenceNumber = 2
						},
						new VerificationAnswerDocument {
							Answer = "wrong 2",
							Id = "4",
							IsCorrect = false,
							SequenceNumber = 3
						},
					}
				}
			};
			#endregion

			#region Answers

			List<QuestionaireAnswer> answers = null;

			#endregion

			bool result = CreditVerificationHelper.VerifyCreditQuestions("", questions, answers);

			Assert.IsFalse(result);
		}

		[Test]
		public void VerifyCreditQuestions_UserAnswersAreLessThanRealAnswers_ReturnsFalse() {
			#region Questions

			List<VerificationQuestionDocument> questions = new List<VerificationQuestionDocument> { 
				new VerificationQuestionDocument {
					Id = "q1",
					IsFakeQuestion = false,
					IsLastChanceQuestion = false,
					Question = "this has multiple answers",
					SequenceNumber = 0,
					Answers = new List<VerificationAnswerDocument> {
						new VerificationAnswerDocument {
							Answer = "This is correct",
							Id = "a1",
							IsCorrect = true,
							SequenceNumber = 0
						},
						new VerificationAnswerDocument {
							Answer = "This is also correct",
							Id = "a2",
							IsCorrect = true,
							SequenceNumber = 1
						},
						new VerificationAnswerDocument {
							Answer = "This is not correct",
							Id = "a3",
							IsCorrect = false,
							SequenceNumber = 2
						},
						new VerificationAnswerDocument {
							Answer = "This is also not correct",
							Id = "a4",
							IsCorrect = false,
							SequenceNumber = 3
						}
					}
				}
			};

			#endregion

			#region Answers

			List<QuestionaireAnswer> answers = new List<QuestionaireAnswer> { 
				new QuestionaireAnswer {
					Answers = new List<string> {
						"This is correct"
					},
					QuestionId = "q1"
				}
			};

			#endregion

			bool result = CreditVerificationHelper.VerifyCreditQuestions("", questions, answers);

			Assert.IsFalse(result);
		}

		[Test]
		public void VerifyCreditQuestions_OneAnswerWrongOnMultiple_ReturnsFalse() {
			#region Questions

			List<VerificationQuestionDocument> questions = new List<VerificationQuestionDocument> { 
				new VerificationQuestionDocument {
					Id = "q1",
					IsFakeQuestion = false,
					IsLastChanceQuestion = false,
					Question = "this has multiple answers",
					SequenceNumber = 0,
					Answers = new List<VerificationAnswerDocument> {
						new VerificationAnswerDocument {
							Answer = "This is correct",
							Id = "a1",
							IsCorrect = true,
							SequenceNumber = 0
						},
						new VerificationAnswerDocument {
							Answer = "This is also correct",
							Id = "a2",
							IsCorrect = true,
							SequenceNumber = 1
						},
						new VerificationAnswerDocument {
							Answer = "This is not correct",
							Id = "a3",
							IsCorrect = false,
							SequenceNumber = 2
						},
						new VerificationAnswerDocument {
							Answer = "This is also not correct",
							Id = "a4",
							IsCorrect = false,
							SequenceNumber = 3
						}
					}
				}
			};

			#endregion

			#region Answers

			List<QuestionaireAnswer> answers = new List<QuestionaireAnswer> { 
				new QuestionaireAnswer {
					Answers = new List<string> {
						"This is correct",
						"this not even an answer"
					},
					QuestionId = "q1"
				}
			};

			#endregion

			bool result = CreditVerificationHelper.VerifyCreditQuestions("", questions, answers);

			Assert.IsFalse(result);
		}

		[Test]
		public void VerifyCreditQuestions_ExtraAnswersOnMultiple_ReturnsFalse() {
			#region Questions

			List<VerificationQuestionDocument> questions = new List<VerificationQuestionDocument> { 
				new VerificationQuestionDocument {
					Id = "q1",
					IsFakeQuestion = false,
					IsLastChanceQuestion = false,
					Question = "this has multiple answers",
					SequenceNumber = 0,
					Answers = new List<VerificationAnswerDocument> {
						new VerificationAnswerDocument {
							Answer = "This is correct",
							Id = "a1",
							IsCorrect = true,
							SequenceNumber = 0
						},
						new VerificationAnswerDocument {
							Answer = "This is also correct",
							Id = "a2",
							IsCorrect = true,
							SequenceNumber = 1
						},
						new VerificationAnswerDocument {
							Answer = "This is not correct",
							Id = "a3",
							IsCorrect = false,
							SequenceNumber = 2
						},
						new VerificationAnswerDocument {
							Answer = "This is also not correct",
							Id = "a4",
							IsCorrect = false,
							SequenceNumber = 3
						}
					}
				}
			};

			#endregion

			#region Answers

			List<QuestionaireAnswer> answers = new List<QuestionaireAnswer> { 
				new QuestionaireAnswer {
					Answers = new List<string> {
						"This is correct",
						"This is also correct",
						"this answer will make the method return false"
					},
					QuestionId = "q1"
				}
			};

			#endregion

			bool result = CreditVerificationHelper.VerifyCreditQuestions("", questions, answers);

			Assert.IsFalse(result);
		}

		[Test]
		public void VerifyCreditQuestions_MissingAnswerForQuestion_ReturnsFalse() {
			#region Questions
			List<VerificationQuestionDocument> questions = new List<VerificationQuestionDocument> { 
				new VerificationQuestionDocument {
					Id = "q1",
					IsFakeQuestion = false,
					IsLastChanceQuestion = false,
					QuestionType = "test question",
					SequenceNumber = 0,
					Question = "here's a question 1",
					Answers = new List<VerificationAnswerDocument> {
						new VerificationAnswerDocument {
							Answer = "wrong 1",
							Id = "1",
							IsCorrect = false,
							SequenceNumber = 0
						},
						new VerificationAnswerDocument {
							Answer = "correct",
							Id = "2",
							IsCorrect = true,
							SequenceNumber = 1
						},
						new VerificationAnswerDocument {
							Answer = "wrong 2",
							Id = "3",
							IsCorrect = false,
							SequenceNumber = 2
						},
						new VerificationAnswerDocument {
							Answer = "wrong 4",
							Id = "4",
							IsCorrect = false,
							SequenceNumber = 3
						},
					}
				},
				new VerificationQuestionDocument {
					Id = "q2",
					IsFakeQuestion = false,
					IsLastChanceQuestion = false,
					QuestionType = "test question",
					SequenceNumber = 1,
					Question = "here's a question 2",
					Answers = new List<VerificationAnswerDocument> {
						new VerificationAnswerDocument {
							Answer = "wrong 1",
							Id = "1",
							IsCorrect = false,
							SequenceNumber = 0
						},
						new VerificationAnswerDocument {
							Answer = "wrong 2",
							Id = "2",
							IsCorrect = false,
							SequenceNumber = 1
						},
						new VerificationAnswerDocument {
							Answer = "wrong 3",
							Id = "3",
							IsCorrect = false,
							SequenceNumber = 2
						},
						new VerificationAnswerDocument {
							Answer = "correct",
							Id = "4",
							IsCorrect = true,
							SequenceNumber = 3
						},
					}
				}
			};
			#endregion

			#region Answers

			List<QuestionaireAnswer> answers = new List<QuestionaireAnswer> { 
				new QuestionaireAnswer {
					Answers = new List<string> { "correct" },
					QuestionId = "q1"
				}
			};

			#endregion

			bool result = CreditVerificationHelper.VerifyCreditQuestions("", questions, answers);

			Assert.IsFalse(result);
		}

		[Test]
		public void VerifyCreditQuestions_MissingMultipleAnswersForQuestion_ReturnsFalse() {
			#region Questions
			List<VerificationQuestionDocument> questions = new List<VerificationQuestionDocument> { 
				new VerificationQuestionDocument {
					Id = "q1",
					IsFakeQuestion = false,
					IsLastChanceQuestion = false,
					QuestionType = "test question",
					SequenceNumber = 0,
					Question = "here's a question 1",
					Answers = new List<VerificationAnswerDocument> {
						new VerificationAnswerDocument {
							Answer = "wrong 1",
							Id = "1",
							IsCorrect = false,
							SequenceNumber = 0
						},
						new VerificationAnswerDocument {
							Answer = "correct",
							Id = "2",
							IsCorrect = true,
							SequenceNumber = 1
						},
						new VerificationAnswerDocument {
							Answer = "wrong 2",
							Id = "3",
							IsCorrect = false,
							SequenceNumber = 2
						},
						new VerificationAnswerDocument {
							Answer = "wrong 4",
							Id = "4",
							IsCorrect = false,
							SequenceNumber = 3
						},
					}
				},
				new VerificationQuestionDocument {
					Id = "q2",
					IsFakeQuestion = false,
					IsLastChanceQuestion = false,
					QuestionType = "test question",
					SequenceNumber = 1,
					Question = "here's a question 2",
					Answers = new List<VerificationAnswerDocument> {
						new VerificationAnswerDocument {
							Answer = "wrong 1",
							Id = "1",
							IsCorrect = false,
							SequenceNumber = 0
						},
						new VerificationAnswerDocument {
							Answer = "wrong 2",
							Id = "2",
							IsCorrect = false,
							SequenceNumber = 1
						},
						new VerificationAnswerDocument {
							Answer = "wrong 3",
							Id = "3",
							IsCorrect = false,
							SequenceNumber = 2
						},
						new VerificationAnswerDocument {
							Answer = "correct",
							Id = "4",
							IsCorrect = true,
							SequenceNumber = 3
						},
					}
				},
				new VerificationQuestionDocument {
					Id = "q3",
					IsFakeQuestion = false,
					IsLastChanceQuestion = false,
					QuestionType = "test question",
					SequenceNumber = 1,
					Question = "here's a question 3",
					Answers = new List<VerificationAnswerDocument> {
						new VerificationAnswerDocument {
							Answer = "wrong 1",
							Id = "1",
							IsCorrect = false,
							SequenceNumber = 0
						},
						new VerificationAnswerDocument {
							Answer = "wrong 2",
							Id = "2",
							IsCorrect = false,
							SequenceNumber = 1
						},
						new VerificationAnswerDocument {
							Answer = "correct",
							Id = "3",
							IsCorrect = true,
							SequenceNumber = 2
						},
						new VerificationAnswerDocument {
							Answer = "wrong 3",
							Id = "4",
							IsCorrect = false,
							SequenceNumber = 3
						},
					}
				}
			};
			#endregion

			#region Answers

			List<QuestionaireAnswer> answers = new List<QuestionaireAnswer> { 
				new QuestionaireAnswer {
					Answers = new List<string> { "correct" },
					QuestionId = "q1"
				}
			};

			#endregion

			bool result = CreditVerificationHelper.VerifyCreditQuestions("", questions, answers);

			Assert.IsFalse(result);
		}
	}
}
