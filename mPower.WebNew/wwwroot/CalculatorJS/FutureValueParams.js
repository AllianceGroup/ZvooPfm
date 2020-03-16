


  KJE.parameters.set("CALC_FV",true);
KJE.parameters.set("COMPOUND_INTEREST",KJE.DatePeriods.PERIOD_YEAR);
KJE.parameters.set("DEPOSIT_FREQUENCY",KJE.DatePeriods.PERIOD_MONTHLY);
KJE.parameters.set("ERROR_MSG2","Time elapsed must be at least one period");
  KJE.parameters.set("FUTURE_DATE","NEXT_MONTH");
  KJE.parameters.set("MSG_CAT_LABEL1","Future value of initial amount");
  KJE.parameters.set("MSG_CAT_LABEL2","Future value of periodic deposits");
  KJE.parameters.set("MSG_CAT_LABEL3","Total future value");
  KJE.parameters.set("MSG_ENTER_INFO","Enter all fields below to calculate Future Value");
  KJE.parameters.set("MSG_RESULT","Calculated Future Value is FUTURE_VALUE");
  KJE.parameters.set("PAYMENTS_AT_START",false);
  KJE.parameters.set("PERIODIC_DEPOSIT",0);
  KJE.parameters.set("PRESENT_DATE","TODAY");
  KJE.parameters.set("PRESENT_VALUE",0);
  KJE.parameters.set("RATE_OF_RETURN",KJE.Default.RORMarket);
KJE.parameters.set("MSG_DROPPER_TITLE", "Future Value Inputs:");



/**V3_CUSTOM_CODE**/
/* <!--
  Financial Calculators, ©1998-2017 KJE Computer Solutions, Inc.
  For more information please see:
  <A HREF="http://www.dinkytown.net">http://www.dinkytown.net</A>
 -->
 */
if (KJE.IE7and8) KJE.init();

