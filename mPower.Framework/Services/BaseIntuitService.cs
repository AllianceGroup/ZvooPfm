using mPower.Framework.Mongo;

namespace mPower.Framework.Services
{
    public abstract class BaseIntuitService<T, TFilter> : BaseMongoService<T, TFilter> 
        where T : class
        where TFilter : BaseFilter
    {
        protected MongoIntuit _intuit;

        protected BaseIntuitService(MongoIntuit intuit)
        {
            _intuit = intuit;
        }
    }
}
