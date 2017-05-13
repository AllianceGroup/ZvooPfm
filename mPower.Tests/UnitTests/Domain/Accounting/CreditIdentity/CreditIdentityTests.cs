using System;
using System.Collections.Generic;
using System.Linq;
using Default.Areas.Credit.Helpers;
using Default.ViewModel.Areas.Credit.Verification;
using mPower.Documents.Documents.Credit.CreditIdentity;
using mPower.Domain.Accounting.CreditIdentity;
using mPower.Domain.Accounting.CreditIdentity.Commands;
using mPower.Domain.Accounting.CreditIdentity.Data;
using mPower.Domain.Accounting.CreditIdentity.Events;
using NUnit.Framework;
using Paralect.Domain;
using mPower.Tests.Environment;

namespace mPower.Tests.UnitTests.Domain.Accounting.CreditIdentity
{
	public abstract class CreditIdentityTest : AggregateTest<CreditIdentityAR>
	{
		private readonly DateTime _currentDate = DateTime.Now;

		String _userId = "2";

		public DateTime CurrentDate
		{
			get { return _currentDate; }
		}

		#region Events
		public IEvent CreditIdentity_Created()
		{
			return new CreditIdentity_CreatedEvent
					   {

						   UserId = _userId,
						   Data = new CreditIdentityData()
									  {
										  ClientKey = _id

									  }
					   };
		}

		public IEvent CreditIdentity_Questions_Set()
		{
			return new CreditIdentity_Questions_SetEvent
					   {
						   ClientKey = _id,
						   Questions = new List<VerificationQuestionData>()
					   };
		}

		public IEvent CreditIdentity_Report_Added()
		{
			return new CreditIdentity_Report_AddedEvent()
			{
				ClientKey = _id,
				CreditReportId = "1",
				CreditScoreId = "1",
				Data = new CreditReportData(),
				UserId = _userId,
				UserFullName = String.Empty
			};
		}
		#endregion

		#region Commands

		public ICommand CreditIdentity_Report_Add()
		{
			return new CreditIdentity_Report_AddCommand()
							 {
								 ClientKey = _id,
								 CreditReportId = "1",
								 CreditScoreId = "1",
								 Data = new CreditReportData(),


							 };
		}

		#endregion

		[Test]
		public void correct_answers_for_questionaire_are_validated()
		{
			var questions = CreateVerificationQuestionList();

			var answers = new List<QuestionaireAnswer>()
			              	{
								new QuestionaireAnswer()
									{
										Answers = new List<string> { "Hello World" },
										QuestionId = "001"
									},
								new QuestionaireAnswer()
									{
										Answers = new List<string> { "rightAnswer" },
										QuestionId = "002"
									},
			              	};

			
			var isValid = CreditVerificationHelper.VerifyCreditQuestions("", questions, answers);

			Assert.IsTrue(isValid);
		}

		[Test]
		public void incorrect_answers_for_questionaire_are_invalidated()
		{
			var questions = CreateVerificationQuestionList();

			var answers = new List<QuestionaireAnswer>()
			              	{
								new QuestionaireAnswer()
									{
										Answers = new List<string> { "Hello World" },
										QuestionId = "001"
									},
								new QuestionaireAnswer()
									{
										Answers = new List<string> { "wrongAnswer" },
										QuestionId = "002"
									},
			              	};

			
			var isValid = CreditVerificationHelper.VerifyCreditQuestions("", questions, answers);

			Assert.IsFalse(isValid);

		}

		[Test]
		public void unanswered_answers_for_questionaire_are_invalidated()
		{
			var questions = CreateVerificationQuestionList();

			var answers = new List<QuestionaireAnswer>()
			              	{
								new QuestionaireAnswer()
									{
										Answers = new List<string> { "Hello World" },
										QuestionId = "001"
									},
			              	};

            bool isValid = CreditVerificationHelper.VerifyCreditQuestions("", questions, answers);

			Assert.IsFalse(isValid);

		}

		[Test]
		public void fake_answers_for_questionaire_are_invalidated()
		{
			var questions = CreateVerificationQuestionList();

			var answers = new List<QuestionaireAnswer>()
			              	{
								new QuestionaireAnswer()
									{
										Answers = new List<string> { "This isn't a choice" },
										QuestionId = "001"
									},
								new QuestionaireAnswer()
									{
										Answers = new List<string> { "rightAnswer" },
										QuestionId = "002"
									},
			              	};

            bool isValid = CreditVerificationHelper.VerifyCreditQuestions("", questions, answers);

			Assert.IsFalse(isValid);

		}

		[Test]
		public void answers_for_fake_questions_on_questionaire_are_invalidated()
		{
			var questions = CreateVerificationQuestionList();

			var answers = new List<QuestionaireAnswer>()
			              	{
								new QuestionaireAnswer()
									{
										Answers = new List<string> { "Hello World" },
										QuestionId = "001"
									},
								new QuestionaireAnswer()
									{
										Answers = new List<string> { "rightAnswer" },
										QuestionId = "002"
									},
								new QuestionaireAnswer()
									{
										Answers = new List<string> { "Imaginary Question Answer" },
										QuestionId = "003"
									},
			              	};

			bool isValid = CreditVerificationHelper.VerifyCreditQuestions("", questions, answers);

			Assert.IsFalse(isValid);

		}

		private static List<VerificationQuestionDocument> CreateVerificationQuestionList()
		{
			var questions = new List<VerificationQuestionDocument>
			                	{
			                		new VerificationQuestionDocument
			                			{
			                				Id = "001",
			                				Answers = new VerificationAnswerDocument[]
			                				          	{
			                				          		new VerificationAnswerDocument()
			                				          			{
			                				          				IsCorrect = true,
			                				          				Answer = "Hello World",
			                				          				Id = "001",
			                				          				SequenceNumber = 0
			                				          			},
			                				          		new VerificationAnswerDocument()
			                				          			{
			                				          				IsCorrect = false,
			                				          				Answer = "Hello Wirld",
			                				          				Id = "001",
			                				          				SequenceNumber = 1
			                				          			},
			                				          		new VerificationAnswerDocument()
			                				          			{
			                				          				IsCorrect = false,
			                				          				Answer = "Hello Werld",
			                				          				Id = "001",
			                				          				SequenceNumber = 2
			                				          			},
			                				          		new VerificationAnswerDocument()
			                				          			{
			                				          				IsCorrect = false,
			                				          				Answer = "Hello Wurld",
			                				          				Id = "001",
			                				          				SequenceNumber = 3
			                				          			},
			                				          	}.ToList(),
			                				IsFakeQuestion = false,
			                				IsLastChanceQuestion = false,
			                				QuestionType = "dumb",
			                				SequenceNumber = 0,
			                				Question = "Riddle me this:"
			                			},
			                		new VerificationQuestionDocument
			                			{
			                				Id = "002",
			                				Answers = new VerificationAnswerDocument[]
			                				          	{
			                				          		new VerificationAnswerDocument
			                				          			{
			                				          				Answer = "wrongAnswer",
			                				          				IsCorrect = false,
			                				          				Id = "002",
			                				          				SequenceNumber = 0
			                				          			}, new VerificationAnswerDocument
			                				          			   	{
			                				          			   		Answer = "rightAnswer",
			                				          			   		IsCorrect = true,
			                				          			   		Id = "002",
			                				          			   		SequenceNumber = 1
			                				          			   	}
			                				          	}.ToList(),
			                				IsFakeQuestion = false,
			                				IsLastChanceQuestion = false,
			                				QuestionType = "dumb",
			                				SequenceNumber = 0,
			                				Question = "Okay, well, riddle me THIS:"
			                			},
			                	};

			return questions;
		}
	}
}
