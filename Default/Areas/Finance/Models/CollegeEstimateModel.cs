using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Framework.Mvc.ModelBinders;

namespace Default.Areas.Finance.Models
{
    [Serializable]
    [ModelBinder(typeof(CustomPropertyModelBinder))]
    public class CollegeEstimateModel: IEstimateModel
    {
        [Range(1, 50000, ErrorMessage = "Please enter a valid cost")]
        [PropertyBinder(typeof(MoneyBinder))]
        public decimal CostPerYear { get; set; }

        [Range(1, 100, ErrorMessage = "Please enter a valid age")]
        public int StudentAge { get; set; }

        [Range(1, 100, ErrorMessage = "Please enter a college age")]
        public int CollegeYears { get; set; }

        [Range(12, 100, ErrorMessage = "Please enter a valid begin age")]
        public int BeginAtAge { get; set; }

        [Range(0, 100, ErrorMessage = "Please enter a valid annual cost increase")]
        [PropertyBinder(typeof(PercentBinder))]
        public double AnnualCostIncreaseInPercents { get; set; }

        [Required(ErrorMessage = "Please choose a college type")]
        public CollegeTypeEnum? CollegeType { get; set; }

        [NonSerialized]
        public IEnumerable<SelectListItem> _collegeTypes;

        public IEnumerable<SelectListItem> CollegeTypes { get { return _collegeTypes = _collegeTypes ?? GenerateCollegeTypesList(); }}


        [NonSerialized]
        private IEnumerable<CollegeEstimateModel> _initialModels;

        public IEnumerable<CollegeEstimateModel> InitialModels
        {
            get { return _initialModels = _initialModels ?? CreateInitialModelsList(); }
        }

        protected IEnumerable<CollegeEstimateModel> CreateInitialModelsList()
        {
            yield return new CollegeEstimateModel
            {
                CostPerYear = 15000,
                CollegeYears = 4,
                BeginAtAge = 18,
                AnnualCostIncreaseInPercents = 3,
                CollegeType = CollegeTypeEnum.PublicInState,
                _initialModels = new List<CollegeEstimateModel>(),
                StudentAge = 12
            };
            yield return new CollegeEstimateModel
            {
                CostPerYear = 25000,
                CollegeYears = 4,
                BeginAtAge = 18,
                AnnualCostIncreaseInPercents = 3,
                CollegeType = CollegeTypeEnum.PublicOutOfState,
                _initialModels = new List<CollegeEstimateModel>(),
                StudentAge = 12
            };
            yield return new CollegeEstimateModel
            {
                CostPerYear = 40000,
                CollegeYears = 4,
                BeginAtAge = 18,
                AnnualCostIncreaseInPercents = 3,
                CollegeType = CollegeTypeEnum.Private,
                _initialModels = new List<CollegeEstimateModel>(),
                StudentAge = 12
            };
            yield return new CollegeEstimateModel
            {
                CostPerYear = 5000,
                CollegeYears = 4,
                BeginAtAge = 18,
                AnnualCostIncreaseInPercents = 3,
                CollegeType = CollegeTypeEnum.Community,
                _initialModels = new List<CollegeEstimateModel>(),
                StudentAge = 12
            };
        }

        public decimal EstimatedValue
        {
            get
            {
                return StudentAge == 0 ? 0 : (decimal)LoanCalculator.TotalCostOfCollecge(CollegeYears, (double)CostPerYear, AnnualCostIncreaseInPercents / 100, StudentAge, BeginAtAge);
            }
        }

        public string Title
        {
            get { return "Save for College"; }
        }

        public GoalTypeEnum GoalType
        {
            get { return GoalTypeEnum.College; }
        }

        public CollegeEstimateModel()
        {
            CollegeType = CollegeTypeEnum.PublicInState;
            BeginAtAge = 18;
            AnnualCostIncreaseInPercents = 3;
            CostPerYear = 15000;
            CollegeYears = 4;
        }

        private static IEnumerable<SelectListItem> GenerateCollegeTypesList()
        {
            return new List<SelectListItem>
            {
                CreateSelectListItem(CollegeTypeEnum.PublicInState,"Public (in state)" ),
                CreateSelectListItem(CollegeTypeEnum.PublicOutOfState,"Public (out of state)"),
                CreateSelectListItem(CollegeTypeEnum.Private),
                CreateSelectListItem(CollegeTypeEnum.Community)
            };
        }

        private static SelectListItem CreateSelectListItem(CollegeTypeEnum value)
        {
            return CreateSelectListItem(value, value.ToString());
        }

        private static SelectListItem CreateSelectListItem(CollegeTypeEnum value, String label)
        {
            return new SelectListItem
            {
                Text = label,
                Value = ((int)value).ToString()
            };
        }
    }

    [Serializable]
    public enum CollegeTypeEnum
    {
        [Display(Name = "Public (in state)")] 
        PublicInState,

        [Display(Name = "Public (out of state)")] 
        PublicOutOfState,

        Private,

        Community
    }
}
