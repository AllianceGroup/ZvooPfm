using mPower.Framework.Mongo;
using mPower.Framework.Services;

namespace mPower.Domain.Yodlee.Storage
{
    public abstract class BaseYodleeDocumentService<T, TFilter> : BaseMongoService<T, TFilter> 
        where T : class
        where TFilter : BaseFilter
    {
        protected MongoYodlee _db;

        protected BaseYodleeDocumentService(MongoYodlee data)
        {
            _db = data;
        }
    }
}
