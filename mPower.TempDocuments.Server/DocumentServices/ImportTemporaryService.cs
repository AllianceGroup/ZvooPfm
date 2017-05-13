using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using mPower.Documents.IifHelpers;
using mPower.Framework;
using mPower.Framework.Services;
using mPower.TempDocuments.Server.DocumentServices.Filters;
using mPower.TempDocuments.Server.Documents;

namespace mPower.TempDocuments.Server.DocumentServices
{
    public class ImportTemporaryService : BaseTemporaryService<ImportTemporaryDocument, ImportFilter>
    {
        public ImportTemporaryService(MongoTemp mongo)
            : base(mongo)
        {
        }

        protected override MongoCollection Items
        {
            get { return _temp.Imports; }
        }

        protected override IEnumerable<IMongoQuery> BuildFilterQuery(ImportFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.Id))
            {
                yield return Query.EQ("_id", filter.Id);
            }
            if (!string.IsNullOrEmpty(filter.LedgerId))
            {
                yield return Query.EQ("LedgerImports._id", filter.LedgerId);
            }
        }

        public void SaveTempResult(string userId, string ledgerId, IifParsingResult parsingResult)
        {
            var userImports = GetById(userId);

            if (userImports == null)
            {
                // create new
                var newUserImport = new ImportTemporaryDocument {Id = userId};
                newUserImport.LedgerImports.Add(new LedgerImportDocument{Id = ledgerId, ParsingResult = parsingResult});
                Items.Save(newUserImport);
            }
            else
            {
                IMongoQuery query;
                UpdateBuilder update;
                if (userImports.LedgerImports.Any(li => li.Id == ledgerId))
                {
                    // update
                    query = Query.And(BuildFilterQuery(new ImportFilter { Id = userId, LedgerId = ledgerId }).ToArray());
                    update = MongoDB.Driver.Builders.Update.SetWrapped("LedgerImports.$.ParsingResult", parsingResult);
                }
                else
                {
                    // insert
                    query = Query.EQ("_id", userId);

                    var newLedgerImport = new LedgerImportDocument { Id = ledgerId, ParsingResult = parsingResult };
                    update = MongoDB.Driver.Builders.Update.PushWrapped("LedgerImports", newLedgerImport);
                }
                Items.Update(query, update);
            }
        }
    }
}
