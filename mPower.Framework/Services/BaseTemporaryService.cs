namespace mPower.Framework.Services
{
    public abstract class BaseTemporaryService<T, TFilter> : BaseMongoService<T, TFilter> 
        where T : class
        where TFilter : BaseFilter
    {
        protected MongoTemp _temp;

        protected BaseTemporaryService(MongoTemp temp)
        {
            _temp = temp;
        }
    }
}
