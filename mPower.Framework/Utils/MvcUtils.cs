using System;
using System.IO;
using System.Text;
using System.Web.Mvc;
using System.Web.UI;

namespace mPower.Framework.Utils
{
    public static class MvcUtils
    {
        public static string RenderPartialToStringRazor(ControllerContext context, string partialViewName, Object model, ViewDataDictionary viewData, TempDataDictionary tempData)
        {
            ViewEngineResult result = ViewEngines.Engines.FindPartialView(context, partialViewName);

            if (result.View != null)
            {
                StringBuilder sb = new StringBuilder();
                using (StringWriter sw = new StringWriter(sb))
                {
                    using (HtmlTextWriter output = new HtmlTextWriter(sw))
                    {
                        ViewContext viewContext = new ViewContext(context, result.View, viewData, tempData, output);

                        if (model != null)
                        {
							viewContext.ViewData.Model = model;
                        }
                        
                        result.View.Render(viewContext, output);
                       
                    }
                }

                return sb.ToString();
            }

            return String.Empty;
        }

        public static string RenderViewResultToString(ControllerContext context, ViewResult viewResult)
        {
            if (viewResult.View != null)
            {
                StringBuilder sb = new StringBuilder();
                using (StringWriter sw = new StringWriter(sb))
                {
                    using (HtmlTextWriter output = new HtmlTextWriter(sw))
                    {
                        ViewContext viewContext = new ViewContext(context, viewResult.View, viewResult.ViewData, viewResult.TempData, output);

                        viewResult.View.Render(viewContext, output);
                    }
                }

                return sb.ToString();
            }

            return String.Empty;
        }
    }
}
