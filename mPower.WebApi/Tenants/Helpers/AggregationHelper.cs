using System.Collections.Generic;
using mPower.Aggregation.Client;
using mPower.Aggregation.Contract.Data;
using mPower.Aggregation.Contract.Documents;

namespace mPower.WebApi.Tenants.Helpers
{
    public class AggregationHelper
    {
        private readonly IAggregationClient _aggClient;

        private readonly string _userId;
        private readonly string _ledgerId;

        public AggregationHelper(IAggregationClient aggClient, string userId, string ledgerId)
        {
            _aggClient = aggClient;
            this._userId = userId;
            this._ledgerId = ledgerId;
        }

        private Metadata GetMetadata()
        {
            return new Metadata
            {
                LogonId = _userId,
                //IsLoggingEnabled = _context.AggregationLoggingEnabled,
                IsLoggingEnabled = true,
            };
        }

        public void LaunchPullingTransactions(long accountId, string ledgerId, bool refresh = false)
        {
            _aggClient.LaunchPullingTransactions(GetMetadata(), accountId, ledgerId, _userId, refresh);
        }

        public void DeleteAccount(long accountId, string ledgerId)
        {
            _aggClient.DeleteAccount(GetMetadata(), accountId, ledgerId);
        }

        public void DeleteUser(string logonId)
        {
            _aggClient.DeleteUser(GetMetadata(), logonId);
        }

        public InstitutionDocument GetInstitution(long intuitInstitutionId)
        {
            return _aggClient.GetInstitution(intuitInstitutionId);
        }

        public IEnumerable<InstitutionDocument> GetInstitutions()
        {
            return _aggClient.GetInstitutions();
        }

        public IEnumerable<KeyDocument> GetInstitutionKeys(long institutionId)
        {
            return _aggClient.GetInstitutionKeys(GetMetadata(), institutionId);
        }

        public DiscoverAccountsResponse DiscoverAccounts(long institutionId, IList<KeyDocument> keys)
        {
            return _aggClient.DiscoverAccounts(GetMetadata(), institutionId, keys);
        }

        public DiscoverAccountsResponse DiscoverAccounts(long institutionId, MfaSession session)
        {
            return _aggClient.DiscoverAccounts(GetMetadata(), institutionId, session);
        }

        public IEnumerable<long> AddAccounts(IList<AccountDocument> accounts, IList<KeyDocument> keys, long institutionId)
        {
            return _aggClient.AddAccounts(GetMetadata(), accounts, keys, institutionId);
        }

        //public void UpdateAccount(decimal accountId, IList<KeyDocument> keys, bool resetStatus = false)
        //{
        //    _aggClient.UpdateAccount(GetMetadata(), accountId, keys, resetStatus);
        //}

        public IList<UserDocument> GetAllIntuitUsers()
        {
            return _aggClient.GetAllIntuitUsers();
        }

        public IEnumerable<AccountDocument> GetAccountsByLogonId(string logonId)
        {
            return _aggClient.GetAccountsByLogonId(GetMetadata(), logonId);
        }

        public RefreshAccountsResponse RefreshAccountsInteractively(long intuitAccountId)
        {
            return _aggClient.RefreshAccounts(GetMetadata(), intuitAccountId);
        }

        public RefreshAccountsResponse RefreshAccountsInteractively(long intuitAccountId, MfaSession session)
        {
            return _aggClient.RefreshAccountsMfa(GetMetadata(),intuitAccountId, session);
        }

        public AggregationExceptionDocument GetAggregationException(string exceptionId)
        {
            return _aggClient.GetAggregationException(exceptionId);
        }

        public GetLogsResponse GetLogs(string searchKey, string userId, string exceptionId, int itemsPerPage, int currentPage)
        {
            return _aggClient.GetLogs(searchKey, userId, exceptionId, itemsPerPage, currentPage);
        }

        public void DeleteUserLogs(string userId)
        {
            _aggClient.DeleteUserLogs(userId);
        }

        public BaseAggregationResponse UpdateAccountCredentials(long accountId, IList<KeyDocument> keys)
        {
            return _aggClient.UpdateAccountCredentials(GetMetadata(), accountId, keys);
        }
    }
}