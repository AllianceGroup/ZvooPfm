namespace mPower.Domain.Accounting.Enums
{
    public enum AccountTypeEnum
    {
        /// <summary>
        ///  Debit will Increase the Amount/Balance,Credit will Decrease the Amount/Balance
        /// </summary>
        Asset = 1,
        /// <summary>
        ///  Debit will Decrease Amount, Credit will Increase the Amount
        /// </summary>
        Liability = 2,
        /// <summary>
        ///  Debit will Decrease Amount,Credit will Increase Amount
        /// </summary>
        Equity = 3,
        /// <summary>
        ///  Debit will Decrease Amount,Credit will Increase Amount
        /// </summary>
        Income = 4,
        /// <summary>
        ///  Debit will Increase Amount, Credit will Decrease Amount
        /// </summary>
        Expense = 5
    }
}