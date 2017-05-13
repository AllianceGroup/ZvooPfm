using System;
using System.Globalization;
using System.Linq;
using mPower.Domain.Accounting.Enums;

namespace mPower.Domain.Accounting
{
    public static class AccountingFormatter
    {
        public static string ConvertToDollarsThenFormat(long debitAmountInCents, long creditAmountInCents, AccountTypeEnum accountType)
        {
            decimal amount = Convert.ToDecimal(FormatDebitCreditToPositiveOrNegativeNumberByAccountType(debitAmountInCents, creditAmountInCents, accountType)) / 100;
            
            return String.Format("{0:c}", amount);
        }

        public static string ConvertToDollarsThenFormat(long amountInCents, bool showMinusSign = false)
        {
            var amount = CentsToDollars(amountInCents);

            var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            if (showMinusSign)
            {
                culture.NumberFormat.CurrencyNegativePattern = 1;
            }

            return String.Format(culture, "{0:c}", amount);
        }

        public static string CentsToDollarString(long cents, bool withoutThousandDelimeter = false)
        {
            return String.Format(withoutThousandDelimeter ? "{0:0.00}" : "{0:N}", CentsToDollars(cents));
        }

        public static string CentsToDollarString(long debitAmountInCents, long creditAmountInCents, AccountTypeEnum accountType, bool withoutThousandDelimeter = false)
        {
            decimal amount = Convert.ToDecimal(FormatDebitCreditToPositiveOrNegativeNumberByAccountType(debitAmountInCents, creditAmountInCents, accountType)) / 100;

            return String.Format(withoutThousandDelimeter ? "{0:0.00}" : "{0:N}", amount);
        }

        public static decimal CentsToDollars(long cents)
        {
            return Convert.ToDecimal(cents) / 100;
        }

        public static long DollarsToCents(decimal dollars)
        {
            return Convert.ToInt64(dollars * 100);
        }

        public static long DollarsToCents(string dollars)
        {
            return DollarsToCents(Convert.ToDouble(dollars));
        }

        public static long DollarsToCents(double dollars)
        {
            return Convert.ToInt64(dollars * 100);
        }

        public static long FormatDebitCreditToPositiveOrNegativeNumberByAccountType(long debitAmount, long creditAmount, AccountTypeEnum accountType)
        {
            Int64 amount = 0;

            switch (accountType)
            {
                case AccountTypeEnum.Asset:
                    amount += debitAmount;
                    amount -= creditAmount;
                    break;

                case AccountTypeEnum.Liability:
                    amount -= debitAmount;
                    amount += creditAmount;
                    break;

                case AccountTypeEnum.Equity:
                    amount -= debitAmount;
                    amount += creditAmount;
                    break;

                case AccountTypeEnum.Income:
                    amount -= debitAmount;
                    amount += creditAmount;
                    break;

                case AccountTypeEnum.Expense:
                    amount += debitAmount;
                    amount -= creditAmount;
                    break;
            }


            return amount;
        }
        
        public static AmountTypeEnum DebitOrCredit(long amount, AccountTypeEnum accountType)
        {
            switch (accountType)
            {
                case AccountTypeEnum.Asset:

                    if (amount >= 0)
                        return AmountTypeEnum.Debit;
                    if (amount < 0)
                        return AmountTypeEnum.Credit;

                    break;

                case AccountTypeEnum.Expense:

                    if (amount >= 0)
                        return AmountTypeEnum.Debit;
                    if (amount < 0)
                        return AmountTypeEnum.Credit;

                    break;

                case AccountTypeEnum.Liability:

                    if (amount >= 0)
                        return AmountTypeEnum.Credit;
                    if (amount < 0)
                        return AmountTypeEnum.Debit;

                    break;

                case AccountTypeEnum.Equity:

                    if (amount >= 0)
                        return AmountTypeEnum.Credit;
                    if (amount < 0)
                        return AmountTypeEnum.Debit;

                    break;

                case AccountTypeEnum.Income:

                    if (amount >= 0)
                        return AmountTypeEnum.Credit;
                    if (amount < 0)
                        return AmountTypeEnum.Debit;

                    break;
            }

            throw new Exception("Debit Or Credit Conversion Error");

        }

        public static long CreditAmount(long amount, AccountTypeEnum accountType)
        {
            var amountTypeEnum = DebitOrCredit(amount, accountType);

            if (amountTypeEnum == AmountTypeEnum.Credit)
                return Math.Abs(amount);

            if (amountTypeEnum == AmountTypeEnum.Debit)
                return 0;

            return 0;

        }

        public static long DebitAmount(long amount, AccountTypeEnum accountType)
        {
            var amountTypeEnum = DebitOrCredit(amount, accountType);

            if (amountTypeEnum == AmountTypeEnum.Credit)
                return 0;

            if (amountTypeEnum == AmountTypeEnum.Debit)
                return Math.Abs(amount);

            return 0;

        }

        public static long CreditAmountByTransactionType(long amount, TransactionType type)
        {
            switch (type)
            {
                case TransactionType.Deposit:
                    return 0;

                default :
                    return Math.Abs(amount);

            }
        }

        public static long DebitAmountByTransactionType(long amount, TransactionType type)
        {
            
            switch (type)
            {
                case TransactionType.Deposit:
                    return Math.Abs(amount);

                default:
                    return 0;
            }

        }

        public static AccountTypeEnum AccountLabelToType(AccountLabelEnum label)
        {
            switch (label)
            {
                case AccountLabelEnum.Bank:
                    return AccountTypeEnum.Asset;

                case AccountLabelEnum.AccountsReceivable:
                    return AccountTypeEnum.Asset;

                case AccountLabelEnum.FixedAsset:
                    return AccountTypeEnum.Asset;

                case AccountLabelEnum.OtherAsset:
                    return AccountTypeEnum.Asset;

                case AccountLabelEnum.OtherCurrentAsset:
                    return AccountTypeEnum.Asset;

                case AccountLabelEnum.AccountsPayable:
                    return AccountTypeEnum.Liability;

                case AccountLabelEnum.CreditCard:
                    return AccountTypeEnum.Liability;

                case AccountLabelEnum.LongTermLiability:
                    return AccountTypeEnum.Liability;

                case AccountLabelEnum.OtherCurrentLiability:
                    return AccountTypeEnum.Liability;

                case AccountLabelEnum.Equity:
                    return AccountTypeEnum.Equity;

                case AccountLabelEnum.Income:
                    return AccountTypeEnum.Income;

                case AccountLabelEnum.OtherIncome:
                    return AccountTypeEnum.Income;

                case AccountLabelEnum.CostOfGoodsSold:
                    return AccountTypeEnum.Expense;

                case AccountLabelEnum.Expense:
                    return AccountTypeEnum.Expense;

                case AccountLabelEnum.OtherExpense:
                    return AccountTypeEnum.Expense;
                
                case AccountLabelEnum.Loan:
                    return AccountTypeEnum.Liability;

                case AccountLabelEnum.Investment:
                    return AccountTypeEnum.Asset;

            }


            throw new Exception("Account Label not recognized");
        }


        public static String DollarToWords(double num = 0)
        {
            var numb = num.ToString();
            string val = "", wholeNo = numb, points = "", andStr = "", pointStr = "";

            var endStr = ("and 00/100");

            try
            {
                var decimalPlace = numb.IndexOf(".");
                if (decimalPlace > 0)
                {
                    wholeNo = numb.Substring(0, decimalPlace);
                    points = numb.Substring(decimalPlace + 1);
                    if (Convert.ToInt32(points) > 0)
                    {
                        andStr = ("and");
                        endStr = (points + "/100");
                    }
                }
                val = String.Format("{0} {1}{2} {3}", TranslateWholeNumber(wholeNo).Trim(), andStr, pointStr, endStr);
            }
            catch
            {

            }
            return val;
        }

        private static String TranslateWholeNumber(String number)
        {
            var word = "";
            try
            {
                var beginsZero = false; //tests for 0XX
                var isDone = false; //test if already translated
                var dblAmt = (Convert.ToDouble(number));
                //if ((dblAmt > 0) && number.StartsWith("0"))
                if (dblAmt > 0)
                {
					string newNumber = number;
                    //test for zero or digit zero in a nuemric

                    beginsZero = newNumber.StartsWith("0");

					
					if(beginsZero) {
						int numberOfLeadingZeros = number.ToCharArray().TakeWhile(x => x == '0').Count();
						bool hasMultipleLeadingZeros = numberOfLeadingZeros > 0;

						if(hasMultipleLeadingZeros) {
							newNumber = new string(newNumber.Skip(numberOfLeadingZeros).ToArray());
						}
					}

					var numDigits = newNumber.Length;

                    var pos = 0; //store digit grouping

                    var place = ""; //digit grouping name:hundres,thousand,etc...


                    switch (numDigits)
                    {
                        case 1: //ones' range
                            word = Ones(newNumber);
                            isDone = true;
                            break;
                        case 2: //tens' range
							word = Tens(newNumber);
                            isDone = true;
                            break;
                        case 3: //hundreds' range
                            pos = (numDigits % 3) + 1;
                            place = " Hundred ";
                            break;
                        case 4: //thousands' range
                        case 5:
                        case 6:
                            pos = (numDigits % 4) + 1;
                            place = " Thousand ";
                            break;
                        case 7: //millions' range
                        case 8:
                        case 9:
                            pos = (numDigits % 7) + 1;
                            place = " Million ";
                            break;
                        case 10: //Billions's range
                            pos = (numDigits % 10) + 1;
                            place = " Billion ";
                            break;
                        //add extra case options for anything above Billion...
                        default:
                            isDone = true;
                            break;
                    }

                    if (!isDone)
                    {
                        //if transalation is not done, continue...(Recursion comes in now!!)
						word = TranslateWholeNumber(newNumber.Substring(0, pos)) + place +
							   TranslateWholeNumber(newNumber.Substring(pos));
                        //check for trailing zeros
                        if (beginsZero)
                            word = " and " + word.Trim();
                    }
                    if (word.Trim().Equals(place.Trim()))
                        word = "";
                }
            }
            catch
            {
                ;
            }
            return word.Trim();
        }

        private static String Tens(String digit)
        {
            var digt = Convert.ToInt32(digit);
            String name = null;
            switch (digt)
            {
                case 10:
                    name = "Ten";
                    break;
                case 11:
                    name = "Eleven";
                    break;
                case 12:
                    name = "Twelve";
                    break;
                case 13:
                    name = "Thirteen";
                    break;
                case 14:
                    name = "Fourteen";
                    break;
                case 15:
                    name = "Fifteen";
                    break;
                case 16:
                    name = "Sixteen";
                    break;
                case 17:
                    name = "Seventeen";
                    break;
                case 18:
                    name = "Eighteen";
                    break;
                case 19:
                    name = "Nineteen";
                    break;
                case 20:
                    name = "Twenty";
                    break;
                case 30:
                    name = "Thirty";
                    break;
                case 40:
                    name = "Forty";
                    break;
                case 50:
                    name = "Fifty";
                    break;
                case 60:
                    name = "Sixty";
                    break;
                case 70:
                    name = "Seventy";
                    break;
                case 80:
                    name = "Eighty";
                    break;
                case 90:
                    name = "Ninety";
                    break;
                default:
                    if (digt > 0)
                    {
                        name = Tens(digit.Substring(0, 1) + "0") + " " + Ones(digit.Substring(1));
                    }
                    break;
            }
            return name;
        }

        private static String Ones(String digit)
        {
            var digt = Convert.ToInt32(digit);
            var name = "";
            switch (digt)
            {
                case 1:
                    name = "One";
                    break;
                case 2:
                    name = "Two";
                    break;
                case 3:
                    name = "Three";
                    break;
                case 4:
                    name = "Four";
                    break;
                case 5:
                    name = "Five";
                    break;
                case 6:
                    name = "Six";
                    break;
                case 7:
                    name = "Seven";
                    break;
                case 8:
                    name = "Eight";
                    break;
                case 9:
                    name = "Nine";
                    break;
            }
            return name;
        }

        public static long FormatDebitCreditToPositiveOrNegativeNumberByAccountLabel(long debitAmountInCents, long creditAmountInCents, AccountLabelEnum accountLabel)
        {
            var accountType = AccountLabelToType(accountLabel);

            return FormatDebitCreditToPositiveOrNegativeNumberByAccountType(debitAmountInCents, creditAmountInCents,
                                                                     accountType);
        }


        public static string GenericCategoryGroup(AccountLabelEnum label)
        {

            const string transfer = "Transfer/Payment";

            switch (label)
            {
                case AccountLabelEnum.Income:
                    return label.ToString();

                case AccountLabelEnum.Expense:
                    return label.ToString();

                case AccountLabelEnum.OtherIncome:
                    return AccountLabelEnum.Income.ToString();

                case AccountLabelEnum.OtherExpense:
                    return AccountLabelEnum.Expense.ToString();

                case AccountLabelEnum.CreditCard:
                    return transfer;

                case AccountLabelEnum.Loan:
                    return transfer;

                case AccountLabelEnum.Bank:
                    return transfer;

                case AccountLabelEnum.Investment:
                    return transfer;

                default:
                    return "Other Accounts";
            }

        }

        
        public static long IntuitBalanceToAggregegatedBalanceInCents(decimal? balance, AccountLabelEnum assignedAccountType)
        {
            var lBalance = balance.HasValue ? DollarsToCents(balance.Value) : 0;

            switch (assignedAccountType)
            {
                    case AccountLabelEnum.Bank:
                        return lBalance;
                    case AccountLabelEnum.Investment:
                        return lBalance;
                    case AccountLabelEnum.CreditCard:
                        return -lBalance;
                    case AccountLabelEnum.Loan:
                        return -lBalance;
            }

            throw new Exception("Type not accepted");
        }
    }
}
