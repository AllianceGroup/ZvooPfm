using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using mPower.Yodlee.Domain.ContentServiceItem.Commands;
using mPower.Yodlee.Domain.ContentServiceItem.Data;
using mPower.Yodlee.Domain.ContentServiceItem.Events;
using mPower.Yodlee.Enums;

namespace mPower.Accounting.Tests.UnitTests.User
{
    public class contentserviceitem_update_test : ContentServiceItemTest
    {
        protected ItemAccessStatusEnum _accessStatus = ItemAccessStatusEnum.ACCESS_NOT_VERIFIED;
        protected string _contentServiceId = "5";
        protected string _displayName = "Wells Fargo";
        protected string _itemId = "1";
        protected string _userId = "1";
        protected RefreshStatus _refreshStatus = RefreshStatus.SUCCESS_NEXT_REFRESH_SCHEDULED_CODE;
        protected DateTime _lastSuccessfulUpdate = DateTime.Now;
        protected DateTime _lastUpdateAttempt = DateTime.Now;
        
        
        public override IEnumerable<Paralect.Domain.IEvent> Given()
        {
            yield return ContentServiceItem_Added();
        }

        public override IEnumerable<Paralect.Domain.ICommand> When()
        {
           yield return new ContentServiceItem_UpdateCommand()
            {

                ItemId = _id,
                AccessStatus = _accessStatus,
                RefreshStatus = _refreshStatus,
                LastSuccessfulUpdate = _lastSuccessfulUpdate,
                LastUpdateAttempt = _lastUpdateAttempt,
                Accounts = Accounts().ToList()

            };
        }

        public override IEnumerable<Paralect.Domain.IEvent> Expected()
        {
            yield return new ContentServiceItem_UpdatedEvent()
            {

                ItemId = _id,
                AccessStatus = _accessStatus,
                RefreshStatus = _refreshStatus,
                LastSuccessfulUpdate = _lastSuccessfulUpdate,
                LastUpdateAttempt = _lastUpdateAttempt,
                Accounts = Accounts().ToList()

            };

        }

        [Test]
        
        public void Test()
        {
            Validate();

            DispatchEvents(() =>
            {
                var item = _contentDocumentService.GetById(_id);
                Assert.IsNotNull(item);
                
            });
        }
    }
}
