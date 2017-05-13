using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mPower.Membership.Domain.User;
using mPower.Membership.Domain.User.Events;
using Paralect.Domain;
using mPower.Membership.Domain.User.Commands;
using mPower.Documents.DocumentServices.Membership;
using mPower.Yodlee.Domain.ContentServiceItem;
using mPower.Yodlee.Domain.ContentServiceItem.Commands;
using mPower.Yodlee.Domain.ContentServiceItem.Data;
using mPower.Yodlee.Domain.ContentServiceItem.Events;
using mPower.Yodlee.Enums;
using mPower.Yodlee.Storage.Documents;
using ContentServiceItemAccount = mPower.Yodlee.Domain.ContentServiceItem.Data.ContentServiceItemAccount;
using ContentServiceItemAccountTransaction = mPower.Yodlee.Domain.ContentServiceItem.Data.ContentServiceItemAccountTransaction;

namespace mPower.Accounting.Tests.UnitTests.User
{
    public abstract class ContentServiceItemTest : AggregateTest<ContentServiceItemAR>
    {
        protected ContentServiceItemTest()
        {
            
        }

        protected ItemAccessStatusEnum _accessStatus = ItemAccessStatusEnum.ACCESS_NOT_VERIFIED;
        protected string _contentServiceId = "5";
        protected string _displayName = "Wells Fargo";
        protected string _userId = "1";
        protected RefreshStatus _refreshStatus = RefreshStatus.SUCCESS_NEXT_REFRESH_SCHEDULED_CODE;
        protected DateTime _lastSuccessfulUpdate = DateTime.Now;
        protected DateTime _lastUpdateAttempt = DateTime.Now;

        public IEvent ContentServiceItem_Added()
        {
            return new ContentServiceItem_AddedEvent()
            {
                Data = new ContentServiceItemData()
                           {
                               AccessStatus = _accessStatus,
                               ContentServiceId = _contentServiceId,
                               DisplayName = _displayName,
                               ItemId = _id,
                               UserId = _userId,
                               RefreshStatus = _refreshStatus,
                               LastSuccessfulUpdate = _lastSuccessfulUpdate,
                               LastUpdateAttempt = _lastUpdateAttempt,
                           }
            };
        }


        public ICommand ContentServiceItem_Add()
        {
            return new ContentServiceItem_AddCommand()
            {
                Data = new ContentServiceItemData()
                {
                    AccessStatus = _accessStatus,
                    ContentServiceId = _contentServiceId,
                    DisplayName = _displayName,
                    ItemId = _id,
                    UserId = _userId,
                    RefreshStatus = _refreshStatus,
                    LastSuccessfulUpdate = _lastSuccessfulUpdate,
                    LastUpdateAttempt = _lastUpdateAttempt,
                    Accounts = Account().ToList()
                }
            };
        }



        protected IEnumerable<ContentServiceItemAccount> Account()
        {
            yield return new ContentServiceItemAccount()
            {
                AccountId = "1",
                AccountName = "Test1",
                Transactions = Transaction().ToList()

            };

            yield return new ContentServiceItemAccount()
            {
                AccountId = "3",
                AccountName = "Test",
                Transactions = Transaction().ToList()

            };
        }

        protected IEnumerable<ContentServiceItemAccount> Accounts()
        {
            yield return new ContentServiceItemAccount()
            {
                AccountId = "1",
                AccountName = "Test1",
                Transactions = Transactions().ToList()

            };

            yield return new ContentServiceItemAccount()
            {
                AccountId = "2",
                AccountName = "Test2",
                Transactions = Transactions().ToList()

            };

        }

        protected IEnumerable<ContentServiceItemAccountTransaction> Transaction()
        {
            yield return new ContentServiceItemAccountTransaction()
            {
                Amount = 5.00,
                BankTransactionId = "1",
                Date = new DateTime(2011, 12, 05),
                Description = "Test",

            };

            

        }

        protected IEnumerable<ContentServiceItemAccountTransaction> Transactions()
        {
            yield return new ContentServiceItemAccountTransaction()
            {
                Amount = 5.00,
                BankTransactionId = "2",
                Date = new DateTime(2011, 12, 05),
                Description = "Test",

            };

            yield return new ContentServiceItemAccountTransaction()
            {
                Amount = 15.00,
                BankTransactionId = "3",
                Date = new DateTime(2011, 12, 05),
                Description = "Test",

            };

        }


        protected ContentServiceItemDocumentService  _contentDocumentService
        {
            get
            {
                return GetInstance<ContentServiceItemDocumentService>();
            }
        }
    }
}
