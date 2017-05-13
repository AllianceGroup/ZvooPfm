using System;
using System.Linq;
using Default;
using Default.ViewModel.RealestateController;
using mPower.Documents.DocumentServices.Membership;
using mPower.Domain.Accounting;
using mPower.Domain.Membership.User.Commands;
using mPower.Domain.Membership.User.Data;
using mPower.Framework;
using mPower.Framework.Environment;
using mPower.Framework.Environment.MultiTenancy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace mPower.WebApi.Tenants.Controllers
{
    [Authorize("Pfm")]
    [Route("api/[controller]")]
    public class RealestateController : BaseController
    {
        private readonly ZillowHelpers _zillowService;
        private readonly IIdGenerator _idGenerator;
        private readonly UserDocumentService _userService;

        public RealestateController(ZillowHelpers zillowService,
            IIdGenerator idGenerator, UserDocumentService userService,
            ICommandService command, IApplicationTenant tenant) : base(command, tenant)
        {
            _zillowService = zillowService;
            _idGenerator = idGenerator;
            _userService = userService;
        }

        [HttpGet]
        public RealestatesListModel GetAll()
        {
            return GetRealestatesListModel();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var command = new User_Realestate_DeleteCommand
                {
                    Id = id,
                    UserId = GetUserId(),
                };

                Send(command);
            }
            else
            {
                ModelState.AddModelError("Delete", "Property ID is required.");
                return new BadRequestObjectResult(ModelState);
            }
            return new OkResult();
        }

        [HttpGet("search")]
        public IActionResult Search(string address, string zip)
        {
            try
            {
                var model = _zillowService.Search(address, zip);
                return new OkObjectResult(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return new BadRequestObjectResult(ModelState);
            }
        }

        [HttpGet("get/zillow/{zillowId}")]
        public IActionResult GetZillowProperty(uint zillowId)
        {
            try
            {
                var model = _zillowService.GetZestimate(zillowId);
                return new OkObjectResult(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("NewProperty", ex.Message);
                return new BadRequestObjectResult(ModelState);
            }
        }

        [HttpPost("incudeInWorthRealEster")]
        public ActionResult IncudeInWorthRealEster([FromBody] PropertyModel realEster)
        {
            var command = new User_Realestate_IncludeInWorthCommand()
            {
                Id = realEster.Id,
                UserId = GetUserId(),
                IsIncludedInWorth = realEster.IsIncludedInWorth
            };
            Send(command);
            return new OkResult();
        }

        [HttpPost("save/zillow")]
        public OkObjectResult SaveNewProperty([FromBody] PropertyModel model)
        {
            RealestateRawData rawData = null;
            if (model.ZillowId.HasValue)
            {
                try
                {
                    rawData = _zillowService.GetRealestateRawData(model.ZillowId.Value);
                }
                catch (Exception){}
            }

            var command = new User_Realestate_AddCommand
            {
                Id = _idGenerator.Generate(),
                UserId = GetUserId(),
                Name = model.Name,
                AmountInCents = AccountingFormatter.DollarsToCents(model.Value),
                RawData = rawData,
            };

            Send(command);

            return new OkObjectResult(new PropertyModel
            {
                Id = command.Id,
                Name = command.Name,
                Value = AccountingFormatter.CentsToDollars(command.AmountInCents),
            });
        }

        private RealestatesListModel GetRealestatesListModel()
        {
            var model = new RealestatesListModel();
            var user = _userService.GetById(GetUserId());
            if (user != null)
            {
                model.Items = user.Realestates.Select(r =>
                    new PropertyModel
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Value = AccountingFormatter.CentsToDollars(r.AmountInCents),
                        IsIncludedInWorth = r.IsIncludedInWorth
                    }).ToList();
            }
            return model;
        }
    }
}