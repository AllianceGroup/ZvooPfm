namespace mPower.Framework.Mvc.Validation
{
    public interface IValidator<in T>
    {
        bool IsValid(T command);
    }
}