namespace mPower.WebApi.Tenants.ViewModels.Filters
{
    public class UsersViewModelFilter
    {
        public int PageNumber { get; set; }

        public string SearchKey { get; set; }

        public string Affiliate { get; set; }

        public UsersViewModelFilter()
        {
            PageNumber = 1;
            SearchKey = null;
            Affiliate = null;
        }
    }
}
