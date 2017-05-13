using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using mPower.Domain.Patches;
using mPower.Framework;
using mPower.Web.Admin.Models;
using StructureMap;

namespace mPower.Web.Admin.Controllers
{
    public class ReadModelController : BaseAdminController
    {
        private const string ReadMode = "http://localhost:8080/api/tasks/SwitchReadOnlyMode/?isReadOnlyMode={0}";

        private readonly MongoRead _read;
        private readonly IContainer _container;
        private readonly MPowerSettings _settings;
        private readonly DeploymentHelper _deploymentHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Web.Mvc.Controller"/> class.
        /// </summary>
        public ReadModelController(MongoRead read, IContainer container, MPowerSettings settings, DeploymentHelper deploymentHelper)
        {
            _read = read;
            _container = container;
            _settings = settings;
            _deploymentHelper = deploymentHelper;
        }

        public ActionResult Index()
        {
            var model = new ReadModelGenerationModel
            {
                ReadConnectionString = _settings.MongoReadDatabaseConnectionString,
                WriteConnectionString = _settings.MongoWriteDatabaseConnectionString,
                SetReadModeUrl = String.Format(ReadMode, true),
                DisableReadModeUrl = String.Format(ReadMode, false),
                CopyFromDatabase = _read.Database.Name,
                CopyToDatabase = _read.Database.Name
            };

            var patches = _container.GetAllInstances<IPatch>();
            model.Patches = patches.Select(x => new SelectListItem {Text = x.Name, Value = x.Id.ToString(CultureInfo.InvariantCulture)}).ToList();

            return View(model);
        }

        public ActionResult Regenerate(ReadModelGenerationModel model)
        {
            var sw = new Stopwatch();
            sw.Start();
            _deploymentHelper.RegenerateReadModel(model.ReadConnectionString, model.WriteConnectionString,model.UseInMemoryRegeneration);
            sw.Stop();
            return Content("Done without errors. Elapsed time: " + sw.Elapsed.ToString());
        }

        public ActionResult RunPatch(ReadModelGenerationModel model)
        {
            var patch = _container.GetAllInstances<IPatch>().Single(x => x.Id == model.PatchId);

            _deploymentHelper.WorkWithTransitions(model.ReadConnectionString, model.WriteConnectionString, patch.Apply, patch.UseIncomeTransitions);

            return RedirectToAction("Index");
        }

        public ActionResult GoToReadOnlyMode(string setReadModeUrl)
        {
            _deploymentHelper.SwitchReadMode(setReadModeUrl);

            return RedirectToAction("Index");
        }

        public ActionResult GoToWriteMode(string disableReadModeUrl)
        {
            _deploymentHelper.SwitchReadMode(disableReadModeUrl);

            return RedirectToAction("Index");
        }
    }
}