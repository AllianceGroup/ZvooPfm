using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace mPower.Framework.Mvc.Validation
{
    /// <summary>
    /// <see cref="Validator"/> class for validating a property against another property in the model.
    /// </summary>
    public class EqualToPropertyValidator : CrossFieldValidator<EqualToPropertyAttribute>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EqualToPropertyValidator"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="attribute">The attribute.</param>
        public EqualToPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext,
                                        EqualToPropertyAttribute attribute) :
            base(metadata, controllerContext, attribute)
        {
        }

        /// <summary>
        /// Gets metadata for client validation.
        /// </summary>
        /// <returns>The metadata for client validation.</returns>
        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            var rule = new ModelClientValidationRule
            {
                ValidationType = "equaltoproperty",
                ErrorMessage = Attribute.FormatErrorMessage(Metadata.PropertyName),
            };

            rule.ValidationParameters.Add("otherProperty", Attribute.OtherProperty);

            return new[] { rule };
        }
    }
}