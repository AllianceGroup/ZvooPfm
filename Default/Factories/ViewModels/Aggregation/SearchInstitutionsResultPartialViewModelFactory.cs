using System.Linq;
using Default.ViewModel.GettingStartedController;
using mPower.Documents.ExternalServices.FullTextSearch;
using mPower.Framework.Mvc;

namespace Default.Factories.ViewModels.Aggregation
{
    public class SearchInstitutionsResultPartialViewModelFactory : IObjectFactory<string ,SearchInstitutionsResultPartialViewModel>
    {
        private readonly IntutitInstitutionLuceneService _institutionLuceneService;

        public SearchInstitutionsResultPartialViewModelFactory(IntutitInstitutionLuceneService institutionLuceneService)
        {
            _institutionLuceneService = institutionLuceneService;
        }

        public SearchInstitutionsResultPartialViewModel Load(string searchText)
        {
            var contentServices = _institutionLuceneService.SearchByQuery(searchText)
                .Select(x =>
                            new InstitutionModel()
                            {
                                Url = x.HomeUrl,
                                Id = x.IntuitId,
                                DisplayName = x. Name
                            }).AsEnumerable();

            var model = new SearchInstitutionsResultPartialViewModel
            {
                ContentServices = contentServices,
            };

            return model;
        }
    }
}