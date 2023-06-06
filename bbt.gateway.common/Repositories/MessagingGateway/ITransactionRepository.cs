using bbt.gateway.common.Models;

namespace bbt.gateway.common.Repositories
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        public Task<(IEnumerable<Transaction>,int)> GetOtpMessagesWithPhoneAsync(int countryCode, int prefix, int number, DateTime startDate, DateTime endDate, int page, int pageSize);
        public Task<(IQueryable<Transaction>, int)> GetOtpMessagesWithPhoneByCreatedNameAsync(string createdbyName, int countryCode, int prefix, int number, DateTime startDate, DateTime endDate, int page, int pageSize);
        public Task<(IEnumerable<Transaction>, int)> GetTransactionalSmsMessagesWithPhoneAsync(int countryCode, int prefix, int number, DateTime startDate, DateTime endDate, int page, int pageSize);
        public Task<(IQueryable<Transaction>, int)> GetTransactionalSmsMessagesWithPhoneByCreatedNameAsync(string createdbyName, int countryCode, int prefix, int number, DateTime startDate, DateTime endDate, int page, int pageSize);
        public Task<(IEnumerable<Transaction>, int)> GetOtpMessagesWithCustomerNoAsync(ulong customerNo, DateTime startDate, DateTime endDate, int page, int pageSize);
        public Task<(IQueryable<Transaction>, int)> GetOtpMessagesWithCustomerNoByCreatedNameAsync(string createdbyName, ulong customerNo, DateTime startDate, DateTime endDate, int page, int pageSize);
        public Task<(IEnumerable<Transaction>, int)> GetTransactionalSmsMessagesWithCustomerNoAsync(ulong customerNo, DateTime startDate, DateTime endDate, int page, int pageSize);
        public Task<(IQueryable<Transaction>, int)> GetTransactionalSmsMessagesWithCustomerNoByCreatedNameAsync(string createdbyName, ulong customerNo, DateTime startDate, DateTime endDate, int page, int pageSize);
        public Task<(IEnumerable<Transaction>, int)> GetOtpMessagesWithCitizenshipNoAsync(string citizensipNo, DateTime startDate, DateTime endDate, int page, int pageSize);
        public Task<(IQueryable<Transaction>, int)> GetOtpMessagesWithCitizenshipNoByCreatedNameAsync(string createdbyName, string citizensipNo, DateTime startDate, DateTime endDate, int page, int pageSize);
        public Task<(IEnumerable<Transaction>, int)> GetTransactionalSmsMessagesWithCitizenshipNoAsync(string citizensipNo, DateTime startDate, DateTime endDate, int page, int pageSize);
        public Task<(IQueryable<Transaction>, int)> GetTransactionalSmsMessagesWithCitizenshipNoByCreatedNameAsync(string createdbyName, string citizensipNo, DateTime startDate, DateTime endDate, int page, int pageSize);
        public Task<(IQueryable<Transaction>, int)> GetMailMessagesWithMailAsync(string createdName, string mail, DateTime startDate, DateTime endDate, int page, int pageSize);
        public Task<(IQueryable<Transaction>, int)> GetMailMessagesWithCustomerNoAsync(string createdName, ulong customerNo, DateTime startDate, DateTime endDate, int page, int pageSize);
        public Task<(IQueryable<Transaction>, int)> GetMailMessagesWithCitizenshipNoAsync(string createdName, string citizenshipNo, DateTime startDate, DateTime endDate, int page, int pageSize);
        public IQueryable<Transaction> GetAllMailMessagesWithCitizenshipNoAsync(string createdName, string citizenshipNo, DateTime startDate, DateTime endDate, int page, int pageSize);
        public Task<(IQueryable<Transaction>, int)> GetPushMessagesWithCustomerNoAsync(string createdName, ulong customerNo, DateTime startDate, DateTime endDate, int page, int pageSize);
        public Task<(IQueryable<Transaction>, int)> GetPushMessagesWithCitizenshipNoAsync(string createdName, string citizenshipNo, DateTime startDate, DateTime endDate, int page, int pageSize);
        public Task<IEnumerable<Transaction>> GetReportTransaction(int phoneNumber,string date,string message);
        public Task<int> GetSuccessfullSmsCount(DateTime startDate, DateTime endDate, OperatorType @operator);
        public Task<int> GetSuccessfullForeignSmsCount(DateTime startDate, DateTime endDate, OperatorType @operator);
        public Task<int> GetSuccessfullOtpCount(DateTime startDate, DateTime endDate, OperatorType @operator);
        public Task<int> GetSuccessfullForeignOtpCount(DateTime startDate, DateTime endDate, OperatorType @operator);
        public Task<int> GetOtpRequestCount(DateTime startDate, DateTime endDate, OperatorType @operator, bool isSuccess);
        public Task<int> GetForeignOtpRequestCount(DateTime startDate, DateTime endDate, OperatorType @operator, bool isSuccess);
        public Task<int> GetSmsRequestCount(DateTime startDate, DateTime endDate, OperatorType @operator, bool isSuccess);
        public Task<int> GetForeignSmsRequestCount(DateTime startDate, DateTime endDate, OperatorType @operator, bool isSuccess);
        public Task<Transaction> GetWithIdAsync(Guid TxnId);
        public Task<Transaction> GetWithIdAsNoTrackingAsync(Guid TxnId);
    }
}
