using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using CsvHelper;
using NLog;
using mPower.Framework;
using mPower.OfferingsSystem.Data;

namespace mPower.OfferingsSystem
{
    public class AccessDataRepsitory : IAccessDataRepsitory
    {
        private readonly IFileLoader _fileLoader;

        private readonly Logger _logger = MPowerLogManager.CurrentLogger;

        public AccessDataRepsitory(IFileLoader fileLoader)
        {
            _fileLoader = fileLoader;
        }

        private IEnumerable<T> ReadRecordsFor<T>() where T : class, new()
        {
            var files = _fileLoader.LoadFor<T>();
            foreach (var textReader in files)
            {
                using (var csvReader = new MPowerCsvReader(textReader))
                {
                    csvReader.Configuration.Delimiter = ',';
                    var records = csvReader.GetRecords<T>();
                    var enumerator = records.GetEnumerator();
                    var hasValue = true;
                    while (hasValue)
                    {
                        T value;
                        try
                        {
                            hasValue = enumerator.MoveNext();
                            value = enumerator.Current;
                        }
                        catch (Exception ex)
                        {
                            _logger.ErrorException("Can't read record form CSV file", ex);
                            break;
                        }
                        if (hasValue)
                        {
                            yield return value;
                        }
                    }
                    enumerator.Dispose();
                }
            }
        }

        public IEnumerable<Brand> GetBrands()
        {
            return ReadRecordsFor<Brand>();
        }

        public IEnumerable<Channel> GetChannels()
        {
            return ReadRecordsFor<Channel>();
        }

        public IEnumerable<Merchant> GetMerchants()
        {
            return ReadRecordsFor<Merchant>();
        }

        public IEnumerable<Subscription> GetSubscriptions()
        {
            return ReadRecordsFor<Subscription>();
        }

        public IEnumerable<Category> GetCategories()
        {
            return ReadRecordsFor<Category>();
        }

        public IEnumerable<Member> GetMembers()
        {
            return ReadRecordsFor<Member>();
        }

        public IEnumerable<Mid> GetMids()
        {
            return ReadRecordsFor<Mid>();
        }

        public IEnumerable<Offer> GetOffers()
        {
            return ReadRecordsFor<Offer>();
        }

        public IEnumerable<Product> GetProducts()
        {
            return ReadRecordsFor<Product>();
        }

        public IEnumerable<Redeem> GetRedeems()
        {
            return ReadRecordsFor<Redeem>();
        }

        public IEnumerable<Statement> GetStatements()
        {
            return ReadRecordsFor<Statement>();
        }

        public IEnumerable<Status> GetStatuses()
        {
            return ReadRecordsFor<Status>();
        }

        public IEnumerable<Card> GetCards()
        {
            return ReadRecordsFor<Card>();
        }

        public IEnumerable<Transaction> GetTransactions()
        {
            return ReadRecordsFor<Transaction>();
        }

        public IEnumerable<Usage> GetUsages()
        {
            return ReadRecordsFor<Usage>();
        }

        public IEnumerable<Settlement> GetSettlements()
        {
            return ReadRecordsFor<Settlement>();
        }
    }
}