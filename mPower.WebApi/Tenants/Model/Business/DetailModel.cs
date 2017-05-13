using System;

namespace mPower.WebApi.Tenants.Model.Business
{
    public class DetailModel
    {
        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        public string Id { get; set; }

        public int Dates { get; set; }

        public int P { get; set; }

        public DetailModel()
        {
            Dates = 4;
        }
    }
}
