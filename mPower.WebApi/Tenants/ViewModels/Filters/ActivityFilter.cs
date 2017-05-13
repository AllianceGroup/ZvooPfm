namespace mPower.WebApi.Tenants.ViewModels.Filters
{
    public class ActivityFilter
    {
        public string Id { get; set; }

        public int PageNumber { get; set; }

        public ActivityFilter()
        {
            PageNumber = 1;
        }
    }
}
