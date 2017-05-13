namespace mPower.Framework.Services
{
    public abstract class BaseDocumentService<T, TFilter> : BaseMongoService<T, TFilter> 
        where T : class
        where TFilter : BaseFilter
    {
        protected MongoRead _read;

        protected BaseDocumentService(MongoRead mongo)
        {
            _read = mongo;
        }
    }
}
