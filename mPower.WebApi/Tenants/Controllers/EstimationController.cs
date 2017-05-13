using System;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using Default.Areas.Finance.Models;
using mPower.Domain.Accounting.Enums;
using mPower.Framework;
using mPower.Framework.Environment.MultiTenancy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;

namespace mPower.WebApi.Tenants.Controllers
{
    [Authorize("Pfm")]
    [Route("api/[controller]")]
    public class EstimationController : BaseController
    {
        public EstimationController(ICommandService command, IApplicationTenant tenant) : base(command, tenant){}

        #region Emergency
        [HttpGet("emergency")]
        public EmergencyEstimateModel GetEmergencyModel()
        {
            return new EmergencyEstimateModel();
        }

        [HttpPost("estimate/emergency")]
        public IActionResult EstimateEmergency([FromBody]EmergencyEstimateModel model)
        {
            var estimatedValue = BuildEstimate(model);

            if (!ModelStateIsValid()) return new BadRequestObjectResult(ModelState);
            return new OkObjectResult(new
            {
                EstimatedValue = estimatedValue
            });
        }

        [HttpPost("emergency")]
        public IActionResult EmergencyGoal([FromBody]EmergencyEstimateModel model)
        {
            return BuildSubmiteResponse(model);
        }
        #endregion

        #region Retirement
        [HttpGet("retirement")]
        public RetirementEstimateModel GetRetirementModel()
        {
            return new RetirementEstimateModel();
        }

        [HttpPost("estimate/retirement")]
        public IActionResult EstimateRetirement([FromBody]RetirementEstimateModel model)
        {
            var retireAge = 0;
            var contribution = decimal.Zero;
            var growth = decimal.Zero;

            bool readyForValidation;
            BuildEstimate(model, out readyForValidation);
            if (readyForValidation)
            {
                retireAge = model.RetirementAge;
                contribution = model.TotalContribution;
                growth = model.Growth;
            }

            if(!ModelStateIsValid()) return new BadRequestObjectResult(ModelState);
            return new OkObjectResult(new
            {
                EstimatedValue = model.TotalMoney,
                RetirementAge = model.RetirementAge,
                Growth = growth,
                TotalContribution = contribution
            });
        }

        [HttpPost("retirement")]
        public IActionResult RetirementGoal([FromBody]RetirementEstimateModel model)
        {
            return BuildSubmiteResponse(model);
        }
        #endregion

        #region Home
        [HttpGet("home")]
        public BuyHomeEstimateModel GetHomeModel()
        {
            return new BuyHomeEstimateModel();
        }

        [HttpPost("estimate/home")]
        public IActionResult EstimateHome([FromBody]BuyHomeEstimateModel model)
        {
            var estimatedValue = BuildEstimate(model);

            if (!ModelStateIsValid()) return new BadRequestObjectResult(ModelState);
            return new OkObjectResult(new
            {
                EstimatedValue = estimatedValue,
                model.HomeCost,
                model.MonthlyPayment
            });
        }

        [HttpPost("home")]
        public IActionResult HomeGoal([FromBody]BuyHomeEstimateModel model)
        {
            return BuildSubmiteResponse(model);
        }
        #endregion

        #region Car
        [HttpGet("car")]
        public CarEstimateModel GetCarModel()
        {
            return new CarEstimateModel();
        }

        [HttpPost("estimate/car")]
        public IActionResult EstimateCar([FromBody]CarEstimateModel model)
        {
            ValidateCar(model);
            var estimatedValue = BuildEstimate(model);

            if (!ModelStateIsValid()) return new BadRequestObjectResult(ModelState);
            return new OkObjectResult(new
            {
                EstimatedValue = estimatedValue,
                model.CostWithTaxs,
                model.MonthlyPayment,
                model.Loan,
                model.TradingVehicleCost
            });
        }


        [HttpPost("car")]
        public IActionResult CarGoal([FromBody]CarEstimateModel model)
        {
            ValidateCar(model);
            return BuildSubmiteResponse(model);
        }

        private void ValidateCar(CarEstimateModel model)
        {
            if (!model.IsTradingVehicleCostInputVisible)
            {
                ClearErrorFor(model, x => x.TradingVehicleCost);
            }
            if (!model.IsFinancing)
            {
                ClearErrorFor(model, x => x.MonthlyPayment);
                ClearErrorFor(model, x => x.CreditDuration);
                ClearErrorFor(model, x => x.LoanAnnualPercRate);
            }
        }
        #endregion

        #region College
        [HttpGet("college")]
        public CollegeEstimateModel GetCollegeModel()
        {
            return new CollegeEstimateModel();
        }

        [HttpPost("estimate/college")]
        public IActionResult EstimateCollege([FromBody]CollegeEstimateModel model)
        {
            var estimatedValue = BuildEstimate(model);

            if (!ModelStateIsValid()) return new BadRequestObjectResult(ModelState);
            return new OkObjectResult(new
            {
                EstimatedValue = estimatedValue
            });
        }

        [HttpPost("college")]
        public IActionResult CollegeGoal([FromBody]CollegeEstimateModel model)
        {
            return BuildSubmiteResponse(model);
        }
        #endregion

        #region Trip
        [HttpGet("trip")]
        public TripEstimateModel GetTripModel()
        {
            return new TripEstimateModel();
        }

        [HttpPost("estimate/trip")]
        public IActionResult EstimateTrip([FromBody]TripEstimateModel model)
        {
            var estimatedValue = BuildEstimate(model);

            if (!ModelStateIsValid()) return new BadRequestObjectResult(ModelState);
            return new OkObjectResult(new
            {
                EstimatedValue = estimatedValue
            });
        }

        [HttpPost("trip")]
        public IActionResult TripGoal([FromBody]TripEstimateModel model)
        {
            return BuildSubmiteResponse(model);
        }
        #endregion

        #region Improve home
        [HttpGet("improvehome")]
        public ImproveHomeEstimateModel GetImproveHomeModel()
        {
            return new ImproveHomeEstimateModel();
        }

        [HttpPost("estimate/improvehome")]
        public IActionResult EstimateImprovehome([FromBody]ImproveHomeEstimateModel model)
        {
            ValidateImproveHome(model);
            var estimatedValue = BuildEstimate(model);

            if (!ModelStateIsValid()) return new BadRequestObjectResult(ModelState);
            return new OkObjectResult(new
            {
                EstimatedValue = estimatedValue
            });
        }

        [HttpPost("improvehome")]
        public IActionResult ImprovehomeGoal([FromBody]ImproveHomeEstimateModel model)
        {
            ValidateImproveHome(model);
            return BuildSubmiteResponse(model);
        }

        private void ValidateImproveHome(ImproveHomeEstimateModel model)
        {
            if (!model.IsMultipleSources)
            {
                ClearErrorFor(model, x => x.Amount);
            }
        }
        #endregion

        #region Custom
        [HttpGet("custom")]
        public CustomEstimateModel GetCustomModel()
        {
            return new CustomEstimateModel();
        }

        [HttpPost("estimate/custom")]
        public IActionResult EstimateCustom([FromBody]CustomEstimateModel model)
        {
            var estimatedValue = BuildEstimate(model);

            if (!ModelStateIsValid()) return new BadRequestObjectResult(ModelState);
            return new OkObjectResult(new
            {
                EstimatedValue = estimatedValue
            });
        }

        [HttpPost("custom")]
        public IActionResult CustomGoal([FromBody]CustomEstimateModel model)
        {
            return BuildSubmiteResponse(model);
        }
        #endregion

        [HttpPost("estimatebydateamount")]
        public IActionResult EstimateByDateAmount([FromBody]GoalViewModel model)
        {
            dynamic result = new ExpandoObject();
            if (model.PeriodInMonths > 0)
            {
                model.MonthlyContribution = (model.Amount - model.StartingBalance) / model.PeriodInMonths;
                result.DateAway = model.DateAway;
                result.MonthlyContribution = model.MonthlyContribution;
                if (!string.IsNullOrEmpty(model.Summary))
                {
                    result.Summary = model.Summary;
                }
            }
            return new OkObjectResult(result);
        }

        [HttpPost("estimatebypayment")]
        public IActionResult EstimateByPayment([FromBody]GoalViewModel model)
        {
            if (ModelState.IsValid && (model.PeriodInMonths > 0 && !string.IsNullOrEmpty(model.Summary)))
            {
                return new OkObjectResult(new {model.Summary});
            }
            return new BadRequestObjectResult(ModelState);
        }

        #region

        private IActionResult BuildSubmiteResponse<T>(T model)
            where T: IEstimateModel
        {
            if (!ModelStateIsValid()) return new BadRequestObjectResult(ModelState);
            return new OkObjectResult(CreateGoal(model));
        }

        protected decimal BuildEstimate(IEstimateModel model, out bool readyForValidation)
        {
            readyForValidation = !IsModelContainsDefaultValues(model);
            if (readyForValidation) return model.EstimatedValue;

            foreach (var state in ModelState.Values)
                state.Errors.Clear();

            return 0;
        }

        protected decimal BuildEstimate(IEstimateModel model)
        {
            var readyForValidation = !IsModelContainsDefaultValues(model);
            if (readyForValidation) return model.EstimatedValue;

            foreach (var state in ModelState.Values)
            {
                state.Errors.Clear();
                state.ValidationState = ModelValidationState.Valid;
            }

            return 0;
        }

        private bool IsModelContainsDefaultValues(IEstimateModel model)
        {
            var properties = model.GetType().GetProperties();
            return
                properties.Any(
                    info =>
                    info.PropertyType.IsValueType && info.PropertyType != typeof(bool) &&
                    Equals(info.GetValue(model, null), Activator.CreateInstance(info.PropertyType)) &&
                    ModelState.ContainsKey(info.Name) &&
                    ModelState[info.Name].Errors.Any());
        }

        private void ClearErrorFor<T, TOut>(T model, Expression<Func<T, TOut>> expression)
        {
            var name = ExpressionHelper.GetExpressionText(expression);
            ClearError(name);
        }

        private void ClearError(string name)
        {
            if (ModelState.ContainsKey(name))
                ModelState[name].Errors.Clear();
        }

        private bool ModelStateIsValid()
        {
            return !ModelState.Values.Any(x => x.Errors.Any());
        }

        private static GoalViewModel CreateGoal(IEstimateModel model, DateTime? plannedDate = null)
        {
            var result = new GoalViewModel
            {
                Amount = Math.Round(model.EstimatedValue),
                Title = GetTitle(model),
                Type = model.GoalType
            };

            if (plannedDate.HasValue)
                result.PlannedDate = plannedDate.Value;

            result.MonthlyContribution = result.Amount / result.PeriodInMonths;
            result.BackLinkAction = result.EstimateAction;

            return result;
        }

        private static string GetTitle(IEstimateModel model)
        {
            switch (model.GoalType)
            {
                case GoalTypeEnum.Custom:
                    return ((CustomEstimateModel)model).CustomTitle;
                case GoalTypeEnum.Trip:
                    return ((TripEstimateModel)model).Destination;
                default:
                    return model.Title;
            }
        }
        #endregion
    }
}