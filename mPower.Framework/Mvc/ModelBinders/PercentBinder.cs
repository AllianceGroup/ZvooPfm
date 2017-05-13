using System;
using System.Web.Mvc;

namespace mPower.Framework.Mvc.ModelBinders
{
    public class PercentBinder: IModelBinder
    {

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var input = ((string[])bindingContext.ValueProvider.GetValue(bindingContext.ModelName).RawValue)[0];
            var value = input.Replace("%", "");
            try
            {
                var result = double.Parse(value);
                return result;
            }
            catch (Exception)
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName,
                                                        String.Format("{0} is not valid format for {1}", input,
                                                                      bindingContext.ModelMetadata.GetDisplayName()));
                return default(double);
            }
        }
    }
}