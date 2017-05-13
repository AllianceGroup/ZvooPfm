namespace Default.Areas.Finance.Controllers
{
    public class ZipCodeModel
    {
        public string Zip { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}