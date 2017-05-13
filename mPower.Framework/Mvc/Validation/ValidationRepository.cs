using System;
using StructureMap;

namespace mPower.Framework.Mvc.Validation
{
    public class ValidationRepository : IValidationRepository
    {
        private readonly IContainer _container;

        public ValidationRepository(IContainer container)
        {
            _container = container;
        }

        public bool Validates<T>(T input)
        {
            var validator = _container.TryGetInstance<IValidator<T>>();
            
            if (validator == null) 
                throw new Exception("Command Validator Not Found");
            
            return validator.IsValid(input);
        }
    }
}