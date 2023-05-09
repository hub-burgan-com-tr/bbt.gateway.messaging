using bbt.gateway.common.Models;
using bbt.gateway.messaging.ui.Data;
using Refit;

namespace bbt.gateway.messaging.ui.Data
{
    public interface IMessagingGatewayService
    {
        [Get("/api/v2/Administration/transactions/phone/{phone.CountryCode}/{phone.Prefix}/{phone.Number}")]
        Task<TransactionsDto> GetTransactionsByPhone(Phone phone,QueryParams queryParams);
        [Get("/api/v2/Administration/transactions/mail/{mail}")]
        Task<TransactionsDto> GetTransactionsByMail(string mail, QueryParams queryParams);
        [Get("/api/v2/Administration/transactions/customer/{customerNo}/{messageType}")]
        Task<TransactionsDto> GetTransactionsByCustomerNo(ulong customerNo,int messageType, QueryParams queryParams);
        [Get("/api/v2/Administration/transactions/citizen/{citizenshipNo}/{messageType}")]
        Task<TransactionsDto> GetTransactionsByCitizenshipNo(string citizenshipNo, int messageType, QueryParams queryParams);

        [Get("/api/v2/Administration/blacklists/{countryCode}/{prefix}/{number}")]
        Task<IEnumerable<BlackListEntry>> GetBlackListByPhone(int countryCode, int prefix, int number, QueryParams queryParams);
        [Get("/api/v1/Administration/blacklists/customer/{customerNo}")]
        Task<BlackListEntriesDto> GetBlackListEntriesByCustomerNo(ulong customerNo, QueryParams queryParams);
        [Get("/api/v1/Administration/blacklists/phone/{countryCode}/{prefix}/{number}")]
        [Headers("Authorization","Bearer")]
        Task<BlackListEntriesDto> GetBlackListEntriesByPhone(int countryCode, int prefix, int number, QueryParams queryParams);
        [Get("/api/v1/Administration/user/control/{userName}")]
        Task<Dictionary<string, string>> GetUserControl(string userName);
        [Post("/api/v2/Administration/sms/check-message")]
        Task<SmsTrackingLog> CheckSmsStatus(common.Models.v2.CheckFastSmsRequest data);
        [Post("/api/v2/Administration/otp/check-message")]
        Task<OtpTrackingLog> CheckOtpSmsStatus(common.Models.v2.CheckSmsRequest data);
        [Get("/api/v2/Administration/transactions/createdName/phone/{createdName}/{phone.CountryCode}/{phone.Prefix}/{phone.Number}")]
        Task<TransactionsDto> GetTransactionsByPhoneCreatedName(Phone phone,string createdName, QueryParams queryParams);
        [Get("/api/v2/Administration/transactions/createdName/customer/{createdName}/{customerNo}/{messageType}")]
        Task<TransactionsDto> GetTransactionsByCustomerNoCreatedName(ulong customerNo, string createdName, int messageType, QueryParams queryParams);
        [Get("/api/v2/Administration/transactions/createdName/citizen/{createdName}/{citizenshipNo}/{messageType}")]
        Task<TransactionsDto> GetTransactionsByCitizenshipNoCreatedName(string citizenshipNo, string createdName, int messageType, QueryParams queryParams);
        [Get("/api/v2/Administration/report/phone/{phone.CountryCode}/{phone.Prefix}/{phone.Number}")]
        Task<string> GetTransactionsExcelReportWithPhone(Phone phone,  QueryParams queryParams);
        [Get("/api/v2/Administration/report/customer/{customerNo}/{messageType}")]
        Task<string> GetTransactionsExcelReportWithCustomer(ulong customerNo, int messageType, QueryParams queryParams);
        [Get("/api/v2/Administration/report/citizen/{citizenshipNo}/{messageType}")]
        Task<string> GetTransactionsExcelReportWithCitizenshipNo(string citizenshipNo, int messageType, QueryParams queryParams);
        [Get("/api/v2/Administration/report/mail/{mail}")]
        Task<string> GetTransactionsExcelReportWithMail(string mail, QueryParams queryParams);
        [Get("/api/v2/Administration/Report/Sms/{operator}")]
        Task<common.Models.v2.OperatorReport> SmsReportAsync(int @operator,QueryParams queryParams);



    }
}
