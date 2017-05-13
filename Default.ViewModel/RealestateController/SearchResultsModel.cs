using System.Collections.Generic;

namespace Default.ViewModel.RealestateController
{
    public class SearchResultsModel
    {
        public List<SearchResultsItem> Items { get; set; }
    }

    public class SearchResultsItem
    {
        public uint Id { get; set; }

        public string Address { get; set; }

        public string Description { get; set; }
    }
}