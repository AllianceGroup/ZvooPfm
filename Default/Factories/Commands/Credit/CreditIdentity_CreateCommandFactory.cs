using System;
using System.Globalization;
using Default.ViewModel.Areas.Credit.Verification;
using mPower.Documents.Documents.Credit.CreditIdentity;
using mPower.Domain.Accounting.CreditIdentity.Commands;
using mPower.Domain.Accounting.CreditIdentity.Data;
using mPower.Framework.Environment;
using mPower.Framework.Mvc;
using mPower.Framework.Utils.Security;
using TransUnionWrapper.Model;

namespace Default.Factories.Commands.Credit
{
    public class CreditIdentityCreateCommandFactory :
        IObjectFactory<IdentityViewModel, CreditIdentity_CreateCommand>,
        IObjectFactory<CreditIdentityDocument, CreditIdentity>
    {
        private readonly IEncryptionService _encryptionService;
        private readonly IIdGenerator _idGenerator;

        public CreditIdentityCreateCommandFactory(IEncryptionService encryptionService, IIdGenerator idGenerator)
        {
            _encryptionService = encryptionService;
            _idGenerator = idGenerator;
        }

        public CreditIdentity_CreateCommand Load(IdentityViewModel creditIdentity)
        {
           return new CreditIdentity_CreateCommand
            {
                UserId = creditIdentity.UserId,
                Data = new CreditIdentityData
                {
                    Address = creditIdentity.Address,
                    Address2 = creditIdentity.Address2,
                    City = creditIdentity.City,
                    State = creditIdentity.State,
                    PostalCode = creditIdentity.PostalCode,
                    FirstName = creditIdentity.FirstName,
                    MiddleName = creditIdentity.MiddleName,
                    LastName = creditIdentity.LastName,
                    Suffix = creditIdentity.Suffix,
                    SocialSecurityNumber =
                    _encryptionService.Encode(
                            creditIdentity.SocialSecurityNumber),
                    DateOfBirth = creditIdentity.DateOfBirth,
                    ClientKey = !String.IsNullOrEmpty(creditIdentity.CreditIdentityId) ? creditIdentity.CreditIdentityId : _idGenerator.Generate(),
                },
            };
        }

        public CreditIdentity Load(CreditIdentityDocument creditIdentity)
        {
            return new CreditIdentity
            {
                Address = creditIdentity.Address,
                Address2 = creditIdentity.Address2,
                City = creditIdentity.City,
                State = creditIdentity.State,
                PostalCode = creditIdentity.PostalCode,
                FirstName = creditIdentity.FirstName,
                MiddleName = creditIdentity.MiddleName,
                LastName = creditIdentity.LastName,
                Suffix = creditIdentity.Suffix,
                SocialSecurityNumber =
                    _encryptionService.Decode(
                        creditIdentity.SocialSecurityNumber),
                DateOfBirth = creditIdentity.DateOfBirth,
                ClientKey = creditIdentity.ClientKey,
                BirthYear = creditIdentity.DateOfBirth.Year.ToString(CultureInfo.InvariantCulture)
               
            };
        }
    }
}