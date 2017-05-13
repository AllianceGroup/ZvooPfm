namespace Default.Areas.Api
{
    public class ApiResponseObject
    {
        public ApiResponseStatusEnum status { get; set; }
        public string data { get; set; }
        public string log { get; set; }
        public int error_code { get; set; }
    }
}