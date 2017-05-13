namespace mPower.Framework.Mvc
{
    public interface IObjectFactory<TInput, TOutput>
    {
        TOutput Load(TInput input);
    }

    public interface IObjectFactory<TOutput>
    {
        TOutput Load();
    }
}