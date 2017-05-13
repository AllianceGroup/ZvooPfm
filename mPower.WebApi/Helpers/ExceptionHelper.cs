using mPower.Aggregation.Contract.Exceptions;

namespace mPower.WebApi.Helpers
{
    public static class ExceptionHelper
    {
        public static string GetErrorMessage(this AggregationException exception)
        {
            switch (exception.Reason)
            {
                case AggregationExceptionReason.IncorrectMfaAnswer:
                    return "You incorrectly answered the question. Please repeat again.";
                case AggregationExceptionReason.MfaSessionExpired:
                    return "Session expired. Please, enter username and password again.";
                case AggregationExceptionReason.InvalidCredentials:
                    return "Incorrect username or password. Please, check the correctness of the entered data.";
                default:
                    return "Something happend. Please, try again later.";
            }
        }
    }
}