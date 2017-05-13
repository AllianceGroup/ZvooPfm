using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;

namespace mPower.Documents.Documents.Membership
{
    public class UserStatisticDocument
    {
        [BsonId]
        public string Id { get; set; }

        public Dictionary<string, SpentStatiscticData> MerhchantsSpentInCents { get; set; }

        public Dictionary<string, AccountSpentStatiscticData> ExpenseAccountsSpentInCents { get; set; }

        public long TotalSpent { get; set; }

        public UserStatisticDocument()
        {
            MerhchantsSpentInCents = new Dictionary<string, SpentStatiscticData>();
            ExpenseAccountsSpentInCents = new Dictionary<string, AccountSpentStatiscticData>();
        }

        public IEnumerable<string> GetAccountNamesWithPositiveSpentValue()
        {
            return ExpenseAccountsSpentInCents.Where(x => x.Value.PerMonth.Any(s=> s.Value > 0)).Select(x => x.Value.Name);
        }

        public long GetMerchantSpentInPeriod(string merchantName, DateTime start, DateTime end)
        {
            if (!MerhchantsSpentInCents.ContainsKey(merchantName))
            {
                return 0;
            }
            return MerhchantsSpentInCents[merchantName].GetSpentInPeriod(start, end);
        }

        public long GetAccountSpentInMonth(string accountName, int month, int year)
        {
            if (!ExpenseAccountsSpentInCents.Any(x => x.Value.Name == accountName))
            {
                return 0;
            }
            return ExpenseAccountsSpentInCents.First(x => x.Value.Name == accountName).Value.GetSpentForMonths();
        }

        public long GetAccountSpentInPeriodByAccountName(string accountName, DateTime start, DateTime end)
        {
            if (!ExpenseAccountsSpentInCents.Any(x => x.Value.Name == accountName))
            {
                return 0;
            }
            return ExpenseAccountsSpentInCents.First(x=> x.Value.Name == accountName).Value.GetSpentInPeriod(start, end);
        }

        public IEnumerable<string> GetMerchantNames()
        {
            return MerhchantsSpentInCents.Keys;
        }

        public void AddAccountSpentAmount(string accountId, string accountName, DateTime date, long spent)
        {
            if (!ExpenseAccountsSpentInCents.ContainsKey(accountId))
            {
                ExpenseAccountsSpentInCents[accountId] = new AccountSpentStatiscticData
                                                             {
                                                                 Id = accountId,
                                                                 Name = accountName
                                                             };
            }
            ExpenseAccountsSpentInCents[accountId].AddStats(date,spent);
        }

        public void AddMerchantSpentAmount(string merchantName, DateTime date, long spent)
        {
            if (!MerhchantsSpentInCents.ContainsKey(merchantName))
            {
                MerhchantsSpentInCents[merchantName] = new SpentStatiscticData();
            }
            MerhchantsSpentInCents[merchantName].AddStats(date, spent);
        }

    }

    public class AccountSpentStatiscticData : SpentStatiscticData
    {
        [BsonId]
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class AccountData
    {
        public long SpentInCents { get; set; }
    }

    public class SpentStatiscticData
    {
        public Dictionary<int, long> PerDay { get; set; }

        public Dictionary<int, long> PerMonth { get; set; }

        public long Total { get; set; }

        public SpentStatiscticData()
        {
            PerDay = new Dictionary<int, long>();
            PerMonth = new Dictionary<int, long>();
        }

        public void AddStats(DateTime date, long spent)
        {
            IncOrSetValue(PerDay,date.Date,spent);
            IncOrSetValue(PerMonth, new MonthYear(date.Month,date.Year), spent);
            Total += spent;
        }

        private static void IncOrSetValue<TKey>(IDictionary<int, long> dictionary, TKey key, long value)
        {
            var hash = GetHash(key);
            if (dictionary.ContainsKey(hash))
            {
                dictionary[hash] += value;
            }
            else
            {
                dictionary[hash] = value;
            }
        }

        private static int GetHash(object key)
        {
            if (key is MonthYear)
            {
                return ((MonthYear) key).GetUniqHash();
            }
            if (key is DateTime)
            {
                return ConvertDateToInt((DateTime) key);
            }
            return key.GetHashCode();
        }

        public long GetSpentForMonths(params MonthYear[] months)
        {
            return PerMonth.Where(x => months.Select(m=> m.GetUniqHash()).Contains(x.Key)).Sum(x => x.Value);
        }

        public long GetSpentInPeriod(DateTime start, DateTime end)
        {
            var startHash = ConvertDateToInt(start);
            var endHash = ConvertDateToInt(end);
            return PerDay.Where(x => x.Key >= startHash&& x.Key <= endHash).Sum(x=> x.Value);
        }

        private static int ConvertDateToInt(DateTime date)
        {
            return date.Day + date.Month*100 + date.Year*10000;
        }

        public DateTime? GetMinDate(DateTime? start = null, DateTime? end = null)
        {
            var allDates = PerDay.Keys.ToList();
            if (start.HasValue)
            {
                var startHash = ConvertDateToInt(start.Value);
                allDates = allDates.Where(x => x >= startHash).ToList();
            }
            if (end.HasValue)
            {
                var endHash = ConvertDateToInt(end.Value);
                allDates = allDates.Where(x => x <= endHash).ToList();
            }

            return allDates.Any() ? DateTime.ParseExact(allDates.Min().ToString(CultureInfo.InvariantCulture), "yyyyMMdd", CultureInfo.InvariantCulture) : (DateTime?)null;
        }
    }

    public class MonthYear : IComparable<MonthYear>
    {
        public int Month;
        public int Year;

        public MonthYear(int month, int year)
        {
            Month = month;
            Year = year;
        }

        public int CompareTo(MonthYear other)
        {
            return GetUniqHash().CompareTo(other.GetUniqHash());
        }

        public override string ToString()
        {
            return GetUniqHash().ToString(CultureInfo.InvariantCulture);
        }

        public int GetUniqHash()
        {
            return (Year*100) + Month;
        }
    }

    public class MonthYearEqualityComparer : IEqualityComparer<MonthYear>
    {
        public bool Equals(MonthYear x, MonthYear y)
        {
            return x.GetUniqHash() == y.GetUniqHash();
        }

        public int GetHashCode(MonthYear obj)
        {
            return obj.GetUniqHash();
        }
    }
}