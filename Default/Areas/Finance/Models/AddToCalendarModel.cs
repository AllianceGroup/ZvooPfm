using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Default.Areas.Finance.Models
{
	public class AddToCalendarModel
	{
		public string FunctionString
		{
			get { return _functionMap[Function]; }
		}
		public string ProgramName { get; set; }
		public bool IsAddedToCalendar { get; set; }
		public FunctionEnum Function { get; set; }

		public enum FunctionEnum
		{
			DebtElimination,
			MortgageAcceleration
		}

		private readonly Dictionary<FunctionEnum, string> _functionMap = new Dictionary<FunctionEnum, string>()
		                                                                 	{
																				{FunctionEnum.DebtElimination, "addEliminationToCalendar();"},
																				{FunctionEnum.MortgageAcceleration, "addMortgageToCalendar();"},
		                                                                 	};
	}
}