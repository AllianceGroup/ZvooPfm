using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Accounting.CreditIdentity.Commands
{
    public class CreditIdentity_Report_AddCommandHandler : IMessageHandler<CreditIdentity_Report_AddCommand>
    {
        private readonly IRepository _repository;

        public CreditIdentity_Report_AddCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(CreditIdentity_Report_AddCommand message)
        {
            var creditIdentity = _repository.GetById<CreditIdentityAR>(message.ClientKey);

            creditIdentity.SetCommandMetadata(message.Metadata);
            creditIdentity.AddReport(message.CreditReportId, message.CreditScoreId, message.Data);

            _repository.Save(creditIdentity);
        }
    }
}