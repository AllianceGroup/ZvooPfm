using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using mPower.Documents.IifHelpers.Documents;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Framework.Utils.Extensions;

namespace mPower.Documents.IifHelpers
{
    public class IifParser
    {
        #region Private Types

        private enum RecordType
        {
            Account = 0,
            Transaction = 1,
            Entry = 2,
            TransactionEnd = 3,
        }

        private enum ReadingStatus
        {
            Free = 0,
            HeaderReading = 1,
            RecordReading = 2,
        }

        #endregion

        public static IifParsingResult ParseIifString(Stream iifInput)
        {
            var result = new IifParsingResult();

            if (iifInput.Length > 0)
            {
                // prevRecordType will be changed during reading of headers/records
                RecordType? prevRecordType = null;

                // readingStatus will be changed during reading of multi-line headers/records (transactions)
                var readingStatus = ReadingStatus.Free;

                // recordColumns contains current format for records. Will be changed during reading of headers
                var recordColumns = new Dictionary<RecordType, List<string>>();

                using (var reader = new StreamReader(iifInput))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();

                        if (!string.IsNullOrEmpty(line))
                        {
                            ParseIifLine(line, reader.BaseStream.Position, ref readingStatus, ref prevRecordType, recordColumns, result);
                        }
                    }
                    reader.Close();
                }
            }

            return result;
        }

        #region Helper Methods

        private static void ParseIifLine(string line, long position, ref ReadingStatus readingStatus, ref RecordType? prevRecordType, Dictionary<RecordType, List<string>> recordColumns, IifParsingResult result)
        {
            if (line.StartsWith("!"))
            {
                if (readingStatus == ReadingStatus.RecordReading)
                {
                    throw new FormatException(string.Format("Unexpected end of '{0}' record (position: {1})", prevRecordType, position));
                }
                ParseHeader(line, position, ref prevRecordType, ref readingStatus, recordColumns);
            }
            else
            {
                if (readingStatus == ReadingStatus.HeaderReading)
                {
                    throw new FormatException(string.Format("Unexpected end of '{0}' header (position: {1})", prevRecordType, position));
                }
                ParseRecord(line, position, ref prevRecordType, ref readingStatus, recordColumns, result);
            }
        }

        #region Header

        private static void ParseHeader(string line, long position, ref RecordType? prevRecordType, ref ReadingStatus readingStatus, Dictionary<RecordType, List<string>> recordColumns)
        {
            var headerItems = new List<string>(line.Split(IifConstants.Separator).Except(new[] { string.Empty }));

            // perform data validation and calculation
            if (headerItems.Count < 2 && !headerItems.Contains(IifConstants.HeaderSign + IifConstants.Tranaction.EndKey))
            {
                throw new FormatException(string.Format("Values in line '{0}' must be separated by tabs (position: {1})", line, position));
            }
            var headersRepeats = headerItems.Count - headerItems.Distinct().Count();
            if (headersRepeats > 0)
            {
                throw new FormatException(string.Format("Dublication of columns is unacceptable (number of repeats: {0}; position: {1})", headersRepeats, position));
            }

            var currRecordType = GetRecordType(headerItems[0].TrimStart(IifConstants.HeaderSign));
            if (recordColumns.ContainsKey(currRecordType))
            {
                throw new FormatException(string.Format("Dublication of headers is unacceptable (position: {0})", position));
            }
            var newReadingStatus = CheckHeadersSequence(currRecordType, prevRecordType, readingStatus);
            var columns = headerItems.Skip(1).ToList();
            CheckRequiredColumns(currRecordType, columns);

            // formulate returning data
            if (currRecordType == RecordType.Account || currRecordType == RecordType.Transaction)
            {
                recordColumns.Clear();
            }
            recordColumns.Add(currRecordType, columns);
            readingStatus = newReadingStatus;
            prevRecordType = currRecordType;
        }

        private static ReadingStatus CheckHeadersSequence(RecordType currRecordType, RecordType? prevRecordType, ReadingStatus readingStatus)
        {
            switch (currRecordType)
            {
                case RecordType.Account:
                    if (readingStatus == ReadingStatus.HeaderReading)
                        throw new FormatException("Unexpected end of header");
                    return ReadingStatus.Free;
                case RecordType.Transaction:
                    if (readingStatus == ReadingStatus.HeaderReading)
                        throw new FormatException("Unexpected end of header");
                    return ReadingStatus.HeaderReading;
                case RecordType.Entry:
                    if (readingStatus != ReadingStatus.HeaderReading || (prevRecordType.HasValue && prevRecordType.Value != RecordType.Transaction))
                        throw new FormatException(string.Format("Illegal usage of header key '{0}'", IifConstants.Spl.Key));
                    return ReadingStatus.HeaderReading;
                case RecordType.TransactionEnd:
                    if (readingStatus != ReadingStatus.HeaderReading || (prevRecordType.HasValue && prevRecordType.Value != RecordType.Entry))
                        throw new FormatException(string.Format("Illegal usage of header key '{0}'", IifConstants.Tranaction.EndKey));
                    return ReadingStatus.Free;
                default:
                    throw new NotSupportedException(string.Format("'{0}' value of enumeration {1} is not supported", currRecordType, typeof(RecordType)));
            }
        }

        #endregion

        #region Records

        private static void ParseRecord(string line, long position, ref RecordType? prevRecordType, ref ReadingStatus readingStatus, Dictionary<RecordType, List<string>> recordColumns, IifParsingResult result)
        {
            // perform data validation and calculation
            var recordItems = line.Split(IifConstants.Separator);
            if (recordItems.Length < 2 && !recordItems.Contains(IifConstants.Tranaction.EndKey))
            {
                throw new FormatException(string.Format("Values in line '{0}' must be separated by tabs (position: {1})", line, position));
            }
            var currRecordType = GetRecordType(recordItems[0].TrimStart(IifConstants.HeaderSign));
            if (!recordColumns.ContainsKey(currRecordType))
            {
                throw new FormatException(string.Format("Corellated header is requiered before placing any data (position: {0})", position));
            }
            var newReadingStatus = CheckRecordsSequence(currRecordType, prevRecordType, readingStatus);
            var columnValues = GetValueForEachColumn(recordColumns[currRecordType], recordItems.Skip(1).ToList());
            CheckRequiredColumns(currRecordType, columnValues.Keys);
            // formulate returning data
            GenerateIifDocument(currRecordType, columnValues, result);
            readingStatus = newReadingStatus;
            prevRecordType = currRecordType;
        }

        private static ReadingStatus CheckRecordsSequence(RecordType currRecordType, RecordType? prevRecordType, ReadingStatus readingStatus)
        {
            switch (currRecordType)
            {
                case RecordType.Account:
                    if (readingStatus == ReadingStatus.RecordReading)
                        throw new FormatException("Unexpected end of record");
                    return ReadingStatus.Free;
                case RecordType.Transaction:
                    if (readingStatus == ReadingStatus.RecordReading)
                        throw new FormatException("Unexpected end of record");
                    return ReadingStatus.RecordReading;
                case RecordType.Entry:
                    if (readingStatus != ReadingStatus.RecordReading || (prevRecordType.HasValue && prevRecordType.Value != RecordType.Transaction && prevRecordType.Value != RecordType.Entry))
                        throw new FormatException(string.Format("Illegal usage of record key '{0}'", IifConstants.Spl.Key));
                    return ReadingStatus.RecordReading;
                case RecordType.TransactionEnd:
                    if (readingStatus != ReadingStatus.RecordReading || (prevRecordType.HasValue && prevRecordType.Value != RecordType.Entry))
                        throw new FormatException(string.Format("Illegal usage of record key '{0}'", IifConstants.Tranaction.EndKey));
                    return ReadingStatus.Free;
                default:
                    throw new NotSupportedException(string.Format("'{0}' value of enumeration {1} is not supported", currRecordType, typeof(RecordType)));
            }
        }

        private static Dictionary<string, string> GetValueForEachColumn(List<string> columns, List<string> values)
        {
            var result = new Dictionary<string, string>();
            var length = Math.Min(columns.Count, values.Count);
            for (var i = 0; i < length; i++)
            {
                result.Add(columns[i], values[i]);
            }
            return result;
        }

        private static void GenerateIifDocument(RecordType currRecordType, Dictionary<string, string> columnValues, IifParsingResult result)
        {
            switch (currRecordType)
            {
                case RecordType.Account:
                    result.Accounts.Add(GenerateIifAccount(columnValues));
                    break;
                case RecordType.Transaction:
                    result.Transactions.Add(GenerateIifTransaction(columnValues));
                    break;
                case RecordType.Entry:
                    result.Transactions.Last().Entries.Add(GenerateIifEntry(columnValues));
                    break;
                case RecordType.TransactionEnd:
                    var transaction = result.Transactions.Last();
                    if (transaction.Type == TransactionType.Unknown)
                    {
                        // miss transactions with unknown type
                        result.Transactions.Remove(transaction);
                    }
                    else if (transaction.Entries.Sum(e => e.Debit) != transaction.Entries.Sum(e => e.Credit))
                    {
                        throw new FormatException("Not balanced transaction found");
                    }
                    break;
            }
        }

        private static IifAccount GenerateIifAccount(Dictionary<string, string> columnValues)
        {
            var account = new IifAccount();
            foreach (var pair in columnValues)
            {
                switch (pair.Key)
                {
                    case IifConstants.Account.Name:
                        account.Name = pair.Value;
                        break;
                    case IifConstants.Account.Description:
                        account.Description = pair.Value;
                        break;
                    case IifConstants.Account.Type:
                        account.LabelEnum = GetLabelEnum(pair.Value);
                        break;
                }
            }
            account.TypeEnum = GetAccountTypeByLabel(account.LabelEnum);
            return account;
        }

        private static IifTransaction GenerateIifTransaction(Dictionary<string, string> columnValues)
        {
            var transaction = new IifTransaction();
            var entry = new IifEntry();

            foreach (var pair in columnValues)
            {
                switch (pair.Key)
                {
                    case IifConstants.Tranaction.Id:
                        // not required
                        break;
                    case IifConstants.Tranaction.Type:
                        transaction.Type = GetTransactionType(pair.Value);
                        break;
                    case IifConstants.Tranaction.Date:
                        transaction.BookedDate = DateTime.ParseExact(pair.Value, IifConstants.DateFormat, CultureInfo.InvariantCulture);
                        break;
                    case IifConstants.Tranaction.Amount:
                        decimal amount;
                        if (decimal.TryParse(pair.Value, out amount))
                            entry.Amount = AccountingFormatter.DollarsToCents(amount);
                        else
                            throw new FormatException(string.Format("Can't parse amount '{0}'", pair.Value));
                        break;
                    case IifConstants.Tranaction.AccountName:
                        entry.AccountName = pair.Value;
                        break;
                    case IifConstants.Tranaction.Name:
                        entry.Payee = pair.Value;
                        break;
                    case IifConstants.Tranaction.Memo:
                        entry.Memo = pair.Value;
                        break;
                }
            }

            GetDebitCredit(entry);
            transaction.Entries.Add(entry);
            return transaction;
        }

        private static IifEntry GenerateIifEntry(Dictionary<string, string> columnValues)
        {
            var entry = new IifEntry();

            foreach (var pair in columnValues)
            {
                switch (pair.Key)
                {
                    case IifConstants.Spl.Id:
                        // not required
                        break;
                    case IifConstants.Spl.Type:
                        // already saved in transaction
                        break;
                    case IifConstants.Spl.Date:
                        // already saved in transaction
                        break;
                    case IifConstants.Spl.Amount:
                        decimal amount;
                        if (decimal.TryParse(pair.Value, out amount))
                            entry.Amount = AccountingFormatter.DollarsToCents(amount);
                        else
                            throw new FormatException(string.Format("Can't parse amount '{0}'", pair.Value));
                        break;
                    case IifConstants.Spl.AccountName:
                        entry.AccountName = pair.Value;
                        break;
                    case IifConstants.Spl.Name:
                        entry.Payee = pair.Value;
                        break;
                    case IifConstants.Spl.Memo:
                        entry.Memo = pair.Value;
                        break;
                }
            }

            GetDebitCredit(entry);
            return entry;
        }

        private static AccountLabelEnum GetLabelEnum(string iifName)
        {
            if (!string.IsNullOrEmpty(iifName))
            {
                var values = (AccountLabelEnum[])Enum.GetValues(typeof(AccountLabelEnum));
                foreach (var enumValue in values)
                {
                    if (enumValue.GetIifName().Equals(iifName))
                    {
                        return enumValue;
                    }
                }
            }
            throw new FormatException(string.Format("Can't find AccountLabelEnum value for IIF value '{0}'", iifName));
        }

        private static TransactionType GetTransactionType(string iifName)
        {
            if (!string.IsNullOrEmpty(iifName))
            {
                var values = (TransactionType[])Enum.GetValues(typeof(TransactionType));
                foreach (var enumValue in values)
                {
                    if (enumValue.GetIifName().Equals(iifName))
                    {
                        return enumValue;
                    }
                }
            }
            return TransactionType.Unknown;
        }

        private static void GetDebitCredit(IifEntry entry)
        {
            if (entry.Amount > 0)
            {
                entry.Debit = entry.Amount;
                entry.Credit = 0;
            }
            else
            {
                entry.Debit = 0;
                entry.Credit = -entry.Amount;
            }
        }

        private static AccountTypeEnum GetAccountTypeByLabel(AccountLabelEnum accountLabel)
        {
            switch (accountLabel)
            {
                case AccountLabelEnum.Bank:
                case AccountLabelEnum.AccountsReceivable:
                case AccountLabelEnum.OtherCurrentAsset:
                case AccountLabelEnum.FixedAsset:
                case AccountLabelEnum.OtherAsset:
                    return AccountTypeEnum.Asset;

                case AccountLabelEnum.AccountsPayable:
                case AccountLabelEnum.CreditCard:
                case AccountLabelEnum.OtherCurrentLiability:
                case AccountLabelEnum.LongTermLiability:
                    return AccountTypeEnum.Liability;

                case AccountLabelEnum.Equity:
                    return AccountTypeEnum.Equity;

                case AccountLabelEnum.Income:
                case AccountLabelEnum.OtherIncome:
                    return AccountTypeEnum.Income;

                case AccountLabelEnum.CostOfGoodsSold:
                case AccountLabelEnum.Expense:
                case AccountLabelEnum.OtherExpense:
                    return AccountTypeEnum.Expense;

                default:
                    throw new FormatException(string.Format("AccountLabelEnum value '{0}' can't be resolved to any value of AccountTypeEnum", accountLabel));
            }
        }

        #endregion

        #region Common

        private static RecordType GetRecordType(string typeKey)
        {
            switch (typeKey)
            {
                case IifConstants.Tranaction.Key:
                    return RecordType.Transaction;
                case IifConstants.Spl.Key:
                    return RecordType.Entry;
                case IifConstants.Tranaction.EndKey:
                    return RecordType.TransactionEnd;
                case IifConstants.Account.Key:
                    return RecordType.Account;
                default:
                    throw new FormatException(string.Format("Records' type for key '{0}' is undefined", typeKey));
            }
        }

        private static void CheckRequiredColumns(RecordType currRecordType, ICollection<string> columns)
        {
            List<string> requiredColumns;
            switch (currRecordType)
            {
                case RecordType.Account:
                    requiredColumns = new List<string> { IifConstants.Account.Name, IifConstants.Account.Type };
                    break;
                case RecordType.Transaction:
                    requiredColumns = new List<string> { IifConstants.Tranaction.Type, IifConstants.Tranaction.AccountName, IifConstants.Tranaction.Amount };
                    break;
                case RecordType.Entry:
                    requiredColumns = new List<string> { IifConstants.Spl.Id, IifConstants.Spl.AccountName, IifConstants.Spl.Amount };
                    break;
                default:
                    return;
            }
            var excludedColumns = requiredColumns.Where(rc => !columns.Contains(rc)).ToArray();
            if (excludedColumns.Length > 0)
            {
                throw new FormatException(string.Format("Can't find one or more required columns: '{0}'", string.Join(", ", excludedColumns)));
            }
        }

        #endregion

        #endregion
    }
}
