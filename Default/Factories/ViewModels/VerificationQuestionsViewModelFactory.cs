using System.Collections.Generic;
using System.Linq;
using Default.ViewModel.Areas.Credit.Verification;
using mPower.Documents.Documents.Credit.CreditIdentity;
using mPower.Documents.DocumentServices.Credit;
using mPower.Domain.Accounting.CreditIdentity.Commands;
using mPower.Framework;
using mPower.Framework.Mvc;
using TransUnionWrapper;
using TransUnionWrapper.Model;

namespace Default.Factories.ViewModels
{
    public class VerificationQuestionsViewModelFactory :
        IObjectFactory<string, VerificationQuestionsViewModel>
    {
        private readonly CreditIdentityDocumentService _creditIdentityDocumentService;
        private readonly IObjectRepository _objectRepository;
        private readonly ITransUnionService _transUnionService;
        private readonly ICommandService _commandService;

        public VerificationQuestionsViewModelFactory(CreditIdentityDocumentService creditIdentityDocumentService,
            IObjectRepository objectRepository,
            ITransUnionService transUnionService,
            ICommandService commandService)
        {
            _creditIdentityDocumentService = creditIdentityDocumentService;
            _objectRepository = objectRepository;
            _transUnionService = transUnionService;
            _commandService = commandService;
        }

        public VerificationQuestionsViewModel Load(string clientKey)
        {
            var viewModel = new VerificationQuestionsViewModel
            {
                ClientKey = clientKey
            };

            var questions = _creditIdentityDocumentService.GetValidationQuestions(clientKey);

            if (questions == null || questions.Count == 0)
            {
                var creditIdentityDocument = _creditIdentityDocumentService.GetById(clientKey);

                if (creditIdentityDocument == null)
                    return null;

                var creditIdentity =
                    _objectRepository.Load<CreditIdentityDocument, CreditIdentity>(creditIdentityDocument);

                var transunionQuestions =
                    _transUnionService.GetCreditIdentityVerificationQuestions(creditIdentity);

                var cmd =
                    _objectRepository
                        .Load<IEnumerable<CreditIdentityVerificationQuestion>,
                            CreditIdentity_Questions_SetCommand>(transunionQuestions);

                cmd.ClientKey = clientKey;

                _commandService.Send(cmd);

                viewModel.Questions =
                    this.Load(transunionQuestions);

                return viewModel;
            }

            viewModel.Questions = (from q in questions
                                   select new VerificationQuestion
                                   {
                                       Id = q.Id,
                                       IsFakeQuestion = q.IsFakeQuestion,
                                       IsLastChanceQuestion = q.IsLastChanceQuestion,
                                       QuestionType = q.QuestionType,
                                       SequenceNumber = q.SequenceNumber,
                                       Text = q.Question,
                                       Answers = (from a in q.Answers
                                                  select new VerificationAnswer
                                                  {
                                                      Answer = a.Answer,
                                                      IsCorrect = a.IsCorrect,
                                                      SequenceNumber = a.SequenceNumber,
                                                      QuestionId = q.Id,
                                                      ElementId =
                                                          string.Format("{0}_{1}",
                                                                        q.SequenceNumber,
                                                                        a.SequenceNumber),
                                                  }).OrderBy(x => x.SequenceNumber).
                                           ToList()
                                   }).OrderBy(x => x.SequenceNumber).ToList();


            return viewModel;
        }

        private List<VerificationQuestion> Load(IEnumerable<CreditIdentityVerificationQuestion> questions)
        {

            var quesitons = (from q in questions
                             select new VerificationQuestion
                             {
                                 Id = q.Id,
                                 IsFakeQuestion = q.IsFakeQuestion,
                                 IsLastChanceQuestion = q.IsLastChanceQuestion,
                                 QuestionType = q.QuestionType,
                                 SequenceNumber = q.SequenceNumber,
                                 Text = q.Question,
                                 Answers = (from a in q.Answers
                                            select new VerificationAnswer
                                            {
                                                Answer = a.Answer,
                                                IsCorrect = a.IsCorrect,
                                                SequenceNumber = a.SequenceNumber,
                                                QuestionId = q.Id,
                                                ElementId =
                                                    string.Format("{0}_{1}",
                                                                q.SequenceNumber,
                                                                a.SequenceNumber),
                                            }).OrderBy(x => x.SequenceNumber).
                                     ToList()
                             }).OrderBy(x => x.SequenceNumber).ToList();

            return quesitons;
        }
    }
}