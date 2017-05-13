using System.Web.Mvc;
using Paralect.Domain;
using StructureMap.Attributes;
using mPower.Framework;
using mPower.Framework.Environment;
using mPower.Framework.Mvc.Ajax;
using mPower.TempDocuments.Server.DocumentServices;
using mPower.TempDocuments.Server.Documents;

namespace mPower.Web.Admin.Controllers
{
    public class BaseAdminController : Controller
    {
        [SetterProperty]
        public IIdGenerator IIdGenerator { get; set; }

        [SetterProperty]
        public CommandLogDocumentService CommandLogDocumentService { get; set; }

        [SetterProperty]
        public CommandService CommandService { get; set; }

        public void Send(params ICommand[] commands)
        {
            foreach (var command in commands)
            {
                command.Metadata.UserId = "DevAdmin";
            }

            CommandService.Send(commands);

            foreach (var command in commands)
            {
                CommandLogDocumentService.Save(CommandLogDocument.Create(command, Request.Headers, Request.Params, Request.Url.ToString()));
            }
        }


        /// <summary>
        /// Ajax Response
        /// </summary>
        protected AjaxResponse response = null;

        /// <summary>
        /// Ajax Response
        /// </summary>
        public virtual AjaxResponse AjaxResponse
        {
            get
            {
                if (response == null)
                    response = new AjaxResponse(this);
                return response;
            }
        }

        public virtual ActionResult Result()
        {
            if (!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        AjaxResponse.ValidationContext.AddError(error.ErrorMessage, key);
                    }
                }
            }

            return Json(AjaxResponse.ToJsonObject(), JsonRequestBehavior.AllowGet);
        }
    }
}
