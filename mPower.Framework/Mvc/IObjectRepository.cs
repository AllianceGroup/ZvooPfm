namespace mPower.Framework.Mvc
{
    public interface IObjectRepository
    {
        TOutput Load<TInput, TOutput>(TInput input);
        
    }
}