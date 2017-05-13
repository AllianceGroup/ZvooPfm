using System.ComponentModel;
using System.Web.Mvc;

namespace mPower.Framework.Mvc.ModelBinders
{
    public interface IPropertyBinder
    {
        object BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext,
                            PropertyDescriptor propertyDescriptor);
    }
}