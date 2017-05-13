using System;
using System.Web.Mvc;

namespace mPower.Framework.Mvc.ModelBinders
{
    public class MoneyBinder: IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var input = ((string[])bindingContext.ValueProvider.GetValue(bindingContext.ModelName).RawValue)[0];
            var value = input.Replace("$", "").Replace(",", "");
            try
            {
                var result = decimal.Parse(value);
                return result;
            }
            catch (Exception)
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName,
                                                        String.Format("{0} is not valid format for {1}", input,
                                                                      bindingContext.ModelMetadata.GetDisplayName()));  
                return default(decimal);
            }
        }
    }
}