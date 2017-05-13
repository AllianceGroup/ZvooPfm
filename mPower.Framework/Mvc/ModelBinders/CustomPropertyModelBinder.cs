using System;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;

namespace mPower.Framework.Mvc.ModelBinders
{
    public class CustomPropertyModelBinder : DefaultModelBinder
    {
        protected override object GetPropertyValue(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, IModelBinder propertyBinder)
        {
            // Check if the property has the PropertyBinderAttribute, meaning
            // it's specifying a different binder to use.
            var propertyBinderAttribute = TryFindPropertyBinderAttribute(propertyDescriptor);

            if (propertyBinderAttribute != null)
            {
                var binder = CreateBinder(propertyBinderAttribute);
                var value = binder.BindModel(controllerContext, bindingContext);
                if (bindingContext.ModelMetadata.ConvertEmptyStringToNull && Object.Equals(value, String.Empty))
                {
                    return null;
                }
                return value;
            }
            return base.GetPropertyValue(controllerContext, bindingContext, propertyDescriptor, propertyBinder);
            
        }

        

        private IModelBinder CreateBinder(PropertyBinderAttribute propertyBinderAttribute)
        {
            return (IModelBinder)DependencyResolver.Current.GetService(propertyBinderAttribute.BinderType);
        }

        private PropertyBinderAttribute TryFindPropertyBinderAttribute(PropertyDescriptor propertyDescriptor)
        {
            return propertyDescriptor.Attributes
                .OfType<PropertyBinderAttribute>()
                .FirstOrDefault();
        }

    }
}