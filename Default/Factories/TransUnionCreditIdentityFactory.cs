using System.Globalization;
using mPower.Documents.DocumentServices.Credit;
using mPower.Domain.Accounting.CreditIdentity.Commands;
using mPower.Framework.Mvc;
using mPower.Framework.Utils.Security;
using TransUnionWrapper.Model;

namespace Default.Factories
{
    public class TransUnionCreditIdentityFactory : 
        IObjectFactory<string, CreditIdentity>,
        IObjectFactory<CreditIdentity_CreateCommand, CreditIdentity>
    {
        private readonly CreditIdentityDocumentService _creditIdentityDocumentService;
        private readonly IEncryptionService _encrypter;

        public TransUnionCreditIdentityFactory(
            CreditIdentityDocumentService creditIdentityDocumentService,
            IEncryptionService encrypter)
        {
            _creditIdentityDocumentService = creditIdentityDocumentService;
            _encrypter = encrypter;
        }

        public CreditIdentity Load(string clientKey)
        {
            var creditIdentity = _creditIdentityDocumentService.GetById(clientKey);
            
            var transUnionCreditIdentity = new CreditIdentity
            {
                Address = creditIdentity.Address,
                Address2 = creditIdentity.Address2,
                City = creditIdentity.City,
                State = creditIdentity.State,
                PostalCode = creditIdentity.PostalCode,
                BirthYear = creditIdentity.BirthYear.ToString(CultureInfo.InvariantCulture),
                ClientKey = creditIdentity.ClientKey,
                DateOfBirth = creditIdentity.DateOfBirth,
                FirstName = creditIdentity.FirstName,
                MiddleName = creditIdentity.MiddleName,
                LastName = creditIdentity.LastName,
                SocialSecurityNumber =
                    _encrypter.Decode(creditIdentity.SocialSecurityNumber),
                Suffix = creditIdentity.Suffix,
            };

            return transUnionCreditIdentity;
        }

        public CreditIdentity Load(CreditIdentity_CreateCommand creditIdentity)
        {
            var transUnionCreditIdentity = new CreditIdentity
            {
                Address = creditIdentity.Data.Address,
                Address2 = creditIdentity.Data.Address2,
                City = creditIdentity.Data.City,
                State = creditIdentity.Data.State,
                PostalCode = creditIdentity.Data.PostalCode,
                BirthYear = creditIdentity.Data.DateOfBirth.Year.ToString(CultureInfo.InvariantCulture),
                ClientKey = creditIdentity.Data.ClientKey,
                DateOfBirth = creditIdentity.Data.DateOfBirth,
                FirstName = creditIdentity.Data.FirstName,
                MiddleName = creditIdentity.Data.MiddleName,
                LastName = creditIdentity.Data.LastName,
                SocialSecurityNumber =
                    _encrypter.Decode(creditIdentity.Data.SocialSecurityNumber),
                Suffix = creditIdentity.Data.Suffix,
            };

            return transUnionCreditIdentity;
        }

    }
}