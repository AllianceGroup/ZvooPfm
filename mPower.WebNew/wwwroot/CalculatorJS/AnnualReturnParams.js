


    KJE.parameters.set("CALC_ROR",true);
    KJE.parameters.set("COMPOUND_INTEREST",KJE.DatePeriods.PERIOD_YEAR);
    KJE.parameters.set("DEPOSIT_FREQUENCY",KJE.DatePeriods.PERIOD_MONTHLY);
    KJE.parameters.set("ERROR_MSG2","Time elapsed must be at least one period");
    KJE.parameters.set("FUTURE_DATE","NEXT_MONTH");
    KJE.parameters.set("FUTURE_VALUE",0);
    KJE.parameters.set("MSG_ENTER_INFO","Enter all fields below to calculate rate of return");
    KJE.parameters.set("MSG_RESULT","Calculated Annual Rate of Return is RATE_OF_RETURN");
    KJE.parameters.set("PAYMENTS_AT_START",false);
    KJE.parameters.set("PERIODIC_DEPOSIT",0);
    KJE.parameters.set("PRESENT_DATE","TODAY");
    KJE.parameters.set("PRESENT_VALUE",0);
    KJE.parameters.set("RATE_OF_RETURN",KJE.Default.RORMarket);

    

/**V3_CUSTOM_CODE**/
/* <!--
  Financial Calculators, ©1998-2017 KJE Computer Solutions, Inc.
  For more information please see:
  <A HREF="http://www.dinkytown.net">http://www.dinkytown.net</A>
 -->
 */
if (KJE.IE7and8) KJE.init();

