using System;
using System.Collections.Generic;
using System.Linq;
using Default.ViewModel;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Domain.Accounting;
using mPower.Framework.Mvc;
using mPower.Framework.Utils.Extensions;

namespace Default.Factories.ViewModels
{
    public class GroupedSelectListItemFactory : IObjectFactory<IEnumerable<AccountDocument>, IEnumerable<GroupedSelectListItem>>
    {
       
        public IEnumerable<GroupedSelectListItem> Load(IEnumerable<AccountDocument> accounts)
        {
            return (from a in accounts.OrderBy(x => x.Name)
                    select new GroupedSelectListItem()
                    {
                        Text = String.Format("{0}|{1}", a.Name, a.LabelEnum.GetDescription()),
                        Value = a.Id,
                        Group = AccountingFormatter.GenericCategoryGroup(a.LabelEnum)
                    });
        }
    }

    public class GroupedSeletcListItemFactoryAlternate : IObjectFactory<string, IEnumerable<GroupedSelectListItem>>
    {
        private readonly LedgerDocumentService _ledgerDocumentService;   

        public GroupedSeletcListItemFactoryAlternate(LedgerDocumentService ledgerDocumentService)
        {
            _ledgerDocumentService = ledgerDocumentService;
        }

        public IEnumerable<GroupedSelectListItem> Load(string ledgerId)
        {
            var accounts = _ledgerDocumentService.GetById(ledgerId).Accounts;
            
            return (from a in accounts.OrderBy(x => x.Name)
                    //where a.Id != BaseAccounts.UnknownCash
                    select new GroupedSelectListItem()
                    {
                        Text = String.Format("{0}|{1}", a.Name, a.LabelEnum.GetDescription()),
                        Value = a.Id,
                        Group = AccountingFormatter.GenericCategoryGroup(a.LabelEnum)
                    });
        }
    }
}
