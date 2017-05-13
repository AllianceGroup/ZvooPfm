using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using DataAnnotationsExtensions;
using Default.ViewModel.UploadController;
using mPower.Domain.Accounting.Enums;
using mPower.Framework.Mvc.ModelBinders;

namespace Default.Areas.Finance.Models
{
    [Serializable]
    [ModelBinder(typeof(CustomPropertyModelBinder))]
    public class CustomEstimateModel : IEstimateModel
    {
        [Required(ErrorMessage = "Please enter a valid title")]
        public string CustomTitle { get; set; }

        [Min(1, ErrorMessage = "Please enter a valid cost")]
        [PropertyBinder(typeof(MoneyBinder))]
        public decimal EstimatedValue { get; set; }

        public string Title
        {
            get { return "Create a Custom Goal"; }
        }

        public GoalTypeEnum GoalType
        {
            get { return GoalTypeEnum.Custom; }
        }


        public CustomEstimateModel()
        {
            CustomTitle = "Custom Savings Goal";
        }
    }
}