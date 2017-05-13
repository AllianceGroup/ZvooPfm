using System;
using System.Collections.Generic;
using System.Web.Mvc;
using mPower.Framework.Utils;
using Newtonsoft.Json;

namespace mPower.Framework.Mvc.Ajax
{
    /// <summary>
    /// Store data which will be sent back to the client
    /// </summary>
    public class AjaxResponse
    {
        private readonly Controller controller;
        public String RedirectUrl { get; set; }
        public Boolean ClosePopup { get; set; }
        public Boolean ReloadPage { get; set; }

        /// <summary>
        /// Validation context
        /// </summary>
        public ValidationContext ValidationContext { get; set; }

        public AjaxResponseOptions Options { get; set; }

        /// <summary>
        /// List of refresh items
        /// </summary>
        public IList<UpdateItem> UpdateItems { get; set; }

        /// <summary>
        /// List of refresh items
        /// </summary>
        public IDictionary<String, String> Json { get; set; }

        /// <summary>
        /// Initialization
        /// </summary>
        public AjaxResponse(Controller controller)
        {
            this.controller = controller;
            UpdateItems = new List<UpdateItem>();
            Json = new Dictionary<string, string>();
            ValidationContext = new ValidationContext();
            Options = new AjaxResponseOptions();
        }

        public void Render(String containerSelector, String viewName, Object model, UpdateStyle updateStyle)
        {
            var html = MvcUtils.RenderPartialToStringRazor(controller.ControllerContext, viewName, model, controller.ViewData, controller.TempData);
            AddUpdateItem(containerSelector, html, updateStyle);
        }

        public void Render(String containerSelector, String viewName, Object model)
        {
            Render(containerSelector, viewName, model, UpdateStyle.Insert);
        }

        public void AddJsonItem(String key, Object jsonObject)
        {
            var json = JsonConvert.SerializeObject(jsonObject);
            Json[key] = json;
        }

        public void AddUpdateItem(String selector, String html)
        {
            AddUpdateItem(selector, html, UpdateStyle.Insert);
        }

        public void AddUpdateItem(String selector, String html, UpdateStyle updateStyle)
        {
            UpdateItems.Add(new UpdateItem(selector, html, updateStyle));
        }

        public Object ToJsonObject()
        {
            return new
            {
                Errors = ValidationContext.ToJsonObject(),
                Options,
                UpdateItems,
                RedirectUrl,
                ClosePopup,
                ReloadPage,
                Json
            };
        }
    }

    public class AjaxResponseOptions
    {
        /// <summary>
        /// Errors summary container should be ul id, i will append <li></li> for each error
        /// </summary>
        public string ErrorsSummaryContainer { get; set; }

        public bool ShowErrorNearElement { get; set; }

        /// <summary>
        /// Success box container should be ul id, i will append <li></li> for successMessage
        /// </summary>
        public string SuccessSummaryContainer { get; set; }

        public string SuccessMessage { get; set; }
    }
}
