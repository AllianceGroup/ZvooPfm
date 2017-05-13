using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mPower.Framework.Services;
using mPower.TempDocuments.Server.Documents;
using MongoDB.Driver.Builders;
using mPower.Framework;
using MongoDB.Driver;

namespace mPower.TempDocuments.Server.DocumentServices
{
    public class CommandLogDocumentService : BaseTemporaryService<CommandLogDocument, BaseFilter>
    {
        public CommandLogDocumentService(MongoTemp temp)
            : base(temp)
        {
        }

        protected override MongoCollection Items
        {
            get { return _temp.CommandLogs; }
        }

        protected override IEnumerable<IMongoQuery> BuildFilterQuery(BaseFilter filter)
        {
            yield break;
        }
    }
}
