using System.Xml.Linq;
using Idcs;
using mPower.Framework.Mvc;
using mPower.Domain.Accounting.CreditIdentity.Commands;

namespace Default.Factories.Commands.Credit
{
    public class CreditIdentity_EnrollCommandFactory : IObjectFactory<EnrollmentDto, CreditIdentity_EnrollCommand>
    {
        private readonly MyPublicInfoService _myPublicInfoService;

        public CreditIdentity_EnrollCommandFactory(MyPublicInfoService myPublicInfoService)
        {
            _myPublicInfoService = myPublicInfoService;
        }

        public CreditIdentity_EnrollCommand Load(EnrollmentDto dto)
        {
            string IDSEnrollResponse = _myPublicInfoService.Enroll(dto.CreditIdentity.ClientKey, dto.User.UserName, dto.User.Email, dto.User.FirstName, dto.User.LastName);
            var doc = XElement.Parse(IDSEnrollResponse);
            var node = doc.FirstNode;
            var status = ((XElement)(node)).Value;

            var enrollCommand = new CreditIdentity_EnrollCommand();
            if (status == "SUCCESS")
            {  
                enrollCommand.CreditIdentityId = dto.CreditIdentity.ClientKey;
                node = node.NextNode;
                enrollCommand.MemberId = ((XElement)node).Value;

                node = node.NextNode;
                if (node != null)
                {
                    enrollCommand.ActivationCode = ((XElement)node).Value;
                    node = node.NextNode;
                    if (node != null)
                    {
                        enrollCommand.SalesId = ((XElement)node).Value;
                    }
                }
            }
            else
            {
                enrollCommand = null;
            }

            return enrollCommand;
        }
    }
}