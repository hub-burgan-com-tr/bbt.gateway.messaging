using bbt.gateway.common.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace bbt.gateway.common.Repositories
{
    public class TransactionRepository : Repository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(DatabaseContext context) : base(context)
        {

        }

        public async Task<Transaction> GetWithIdAsync(Guid TxnId)
        {
            return await Context.Transactions
                .Where(t => t.Id == TxnId)
                .Include(t => t.OtpRequestLog).ThenInclude(o => o.ResponseLogs).ThenInclude(o => o.TrackingLogs)
                .Include(t => t.SmsRequestLog).ThenInclude(s => s.ResponseLogs)
                .SingleOrDefaultAsync();
        }

        public async Task<Transaction> GetWithIdAsNoTrackingAsync(Guid TxnId)
        {
            return await Context.Transactions.AsNoTracking()
                .Where(t => t.Id == TxnId)
                .Include(t => t.OtpRequestLog).ThenInclude(o => o.ResponseLogs).ThenInclude(o => o.TrackingLogs)
                .Include(t => t.SmsRequestLog).ThenInclude(s => s.ResponseLogs)
                .Include(t => t.MailRequestLog).ThenInclude(m => m.ResponseLogs)
                .Include(t => t.PushNotificationRequestLog).ThenInclude(p => p.ResponseLogs)
                .SingleOrDefaultAsync();
        }

        public async Task<(IEnumerable<Transaction>,int)> GetOtpMessagesWithPhoneAsync(int countryCode, int prefix, int number, DateTime startDate, DateTime endDate, int page, int pageSize)
        {
            IEnumerable<Transaction> list = await Context.Transactions
               .Where(t => t.Phone.CountryCode == countryCode && t.Phone.Prefix == prefix && t.Phone.Number == number &&
                t.TransactionType == TransactionType.Otp && t.CreatedAt >= startDate && t.CreatedAt <= endDate)
               .Include(t => t.OtpRequestLog).ThenInclude(o => o.ResponseLogs).ThenInclude(o => o.TrackingLogs)
               .Include(t => t.OtpRequestLog.PhoneConfiguration)
               .OrderByDescending(t => t.CreatedAt)
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToListAsync();

            int count = await Context.Transactions
               .CountAsync(t => t.Phone.CountryCode == countryCode && t.Phone.Prefix == prefix && t.Phone.Number == number &&
                t.TransactionType == TransactionType.Otp && t.CreatedAt >= startDate && t.CreatedAt <= endDate);

            return (list, count);
                
        }
        public async Task<(IQueryable<Transaction>, int)> GetOtpMessagesWithPhoneByCreatedNameAsync(string createdbyName, int countryCode, int prefix, int number, DateTime startDate, DateTime endDate, int page, int pageSize)
        {
            IQueryable<Transaction> list =  Context.Transactions.AsNoTracking()
               .Where(t => t.Phone.CountryCode == countryCode && t.Phone.Prefix == prefix && t.Phone.Number == number &&
                t.TransactionType == TransactionType.Otp && t.CreatedAt >= startDate && t.CreatedAt <= endDate
                 && t.CreatedBy.Name.Contains(createdbyName))
               .Include(t => t.OtpRequestLog).ThenInclude(o => o.ResponseLogs).ThenInclude(o => o.TrackingLogs)
               .Include(t => t.OtpRequestLog.PhoneConfiguration)
               .OrderByDescending(t => t.CreatedAt).Select(s => new Transaction
               {
                   CustomerNo = s.CustomerNo,
                   CitizenshipNo = s.CitizenshipNo,
                   CreatedAt = s.CreatedAt,
                   CreatedBy = s.CreatedBy,
                   Phone = s.Phone,
                   Mail = s.Mail,
                   TransactionType = s.TransactionType,
                   OtpRequestLog = s.OtpRequestLog
               })
                .Skip(page * pageSize)
                .Take(pageSize)
                ;

            int count = await Context.Transactions
               .CountAsync(t => t.Phone.CountryCode == countryCode && t.Phone.Prefix == prefix && t.Phone.Number == number &&
                t.TransactionType == TransactionType.Otp && t.CreatedAt >= startDate && t.CreatedAt <= endDate
                 && t.CreatedBy.Name.Contains(createdbyName));

            return (list, count);

        }
        public async Task<(IEnumerable<Transaction>,int)> GetOtpMessagesWithCustomerNoAsync(ulong customerNo, DateTime startDate, DateTime endDate, int page, int pageSize)
        {
            IEnumerable<Transaction> list = await Context.Transactions
                .Where(t => t.CustomerNo == customerNo && t.TransactionType == TransactionType.Otp
                 && t.CreatedAt >= startDate && t.CreatedAt <= endDate)
                .Include(t => t.OtpRequestLog).ThenInclude(o => o.ResponseLogs).ThenInclude(o => o.TrackingLogs)
                .Include(t => t.OtpRequestLog.PhoneConfiguration)
                .OrderByDescending(t => t.CreatedAt)
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToListAsync();

            int count = await Context.Transactions
                .CountAsync(t => t.CustomerNo == customerNo && t.TransactionType == TransactionType.Otp
                 && t.CreatedAt >= startDate && t.CreatedAt <= endDate);

            return (list, count);

        }
        public async Task<(IQueryable<Transaction>, int)> GetOtpMessagesWithCustomerNoByCreatedNameAsync(string createdbyName, ulong customerNo, DateTime startDate, DateTime endDate, int page, int pageSize)
        {
            IQueryable<Transaction> list =  Context.Transactions.AsNoTracking()
                .Where(t => t.CustomerNo == customerNo && t.TransactionType == TransactionType.Otp
                 && t.CreatedAt >= startDate && t.CreatedAt <= endDate
                  && t.CreatedBy.Name.Contains(createdbyName))
                .Include(t => t.OtpRequestLog).ThenInclude(o => o.ResponseLogs).ThenInclude(o => o.TrackingLogs)
                .Include(t => t.OtpRequestLog.PhoneConfiguration)
                .OrderByDescending(t => t.CreatedAt)
                .Select(s => new Transaction
                {
                    CustomerNo = s.CustomerNo,
                    CitizenshipNo = s.CitizenshipNo,
                    CreatedAt = s.CreatedAt,
                    CreatedBy = s.CreatedBy,
                    Phone = s.Phone,
                    Mail = s.Mail,
                    TransactionType = s.TransactionType,
                    OtpRequestLog = s.OtpRequestLog
                })
                .Skip(page * pageSize)
                .Take(pageSize)
                ;

            int count = await Context.Transactions
                .CountAsync(t => t.CustomerNo == customerNo && t.TransactionType == TransactionType.Otp
                 && t.CreatedAt >= startDate && t.CreatedAt <= endDate
                  && t.CreatedBy.Name.Contains(createdbyName));

            return (list, count);

        }

        public async Task<(IEnumerable<Transaction>,int)> GetOtpMessagesWithCitizenshipNoAsync(string citizenshipNo, DateTime startDate, DateTime endDate, int page, int pageSize)
        {
            IEnumerable<Transaction> list = await Context.Transactions
                .Where(t => t.CitizenshipNo == citizenshipNo && t.TransactionType == TransactionType.Otp
                 && t.CreatedAt >= startDate && t.CreatedAt <= endDate)
                .Include(t => t.OtpRequestLog).ThenInclude(o => o.ResponseLogs).ThenInclude(o => o.TrackingLogs)
                .Include(t => t.OtpRequestLog.PhoneConfiguration)
                .OrderByDescending(t => t.CreatedAt)
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToListAsync();

            int count = await Context.Transactions
                .CountAsync(t => t.CitizenshipNo == citizenshipNo && t.TransactionType == TransactionType.Otp
                 && t.CreatedAt >= startDate && t.CreatedAt <= endDate);

            return (list, count);
        }

        public async Task<(IQueryable<Transaction>, int)> GetOtpMessagesWithCitizenshipNoByCreatedNameAsync(string createdbyName, string citizenshipNo, DateTime startDate, DateTime endDate, int page, int pageSize)
        {
            IQueryable<Transaction> list =  Context.Transactions.AsNoTracking()
                .Where(t => t.CitizenshipNo == citizenshipNo && t.TransactionType == TransactionType.Otp
                 && t.CreatedAt >= startDate && t.CreatedAt <= endDate
                 && t.CreatedBy.Name.Contains(createdbyName))
                .Include(t => t.OtpRequestLog).ThenInclude(o => o.ResponseLogs).ThenInclude(o => o.TrackingLogs)
                .Include(t => t.OtpRequestLog.PhoneConfiguration)
                .OrderByDescending(t => t.CreatedAt)
                .Select(s => new Transaction
                {
                    CustomerNo = s.CustomerNo,
                    CitizenshipNo = s.CitizenshipNo,
                    CreatedAt = s.CreatedAt,
                    CreatedBy = s.CreatedBy,
                    Phone = s.Phone,
                    Mail = s.Mail,
                    TransactionType = s.TransactionType,
                    OtpRequestLog = s.OtpRequestLog
                })
                .Skip(page * pageSize)
                .Take(pageSize)
                ;

            int count = await Context.Transactions
                .CountAsync(t => t.CitizenshipNo == citizenshipNo && t.TransactionType == TransactionType.Otp
                 && t.CreatedAt >= startDate && t.CreatedAt <= endDate
                 && t.CreatedBy.Name.Contains(createdbyName));

            return (list, count);
        }

        public async Task<(IEnumerable<Transaction>,int)> GetTransactionalSmsMessagesWithPhoneAsync(int countryCode, int prefix, int number, DateTime startDate, DateTime endDate, int page, int pageSize)
        {
            IEnumerable<Transaction> list = await Context.Transactions
                .Where(t => t.Phone.CountryCode == countryCode && t.Phone.Prefix == prefix && t.Phone.Number == number && (t.TransactionType == TransactionType.TransactionalSms
                || t.TransactionType == TransactionType.TransactionalTemplatedSms)
                && t.CreatedAt >= startDate && t.CreatedAt < endDate)
                .Include(t => t.SmsRequestLog).ThenInclude(s => s.ResponseLogs)
                .OrderByDescending(t => t.CreatedAt)
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToListAsync();

            int count = await Context.Transactions
                .CountAsync(t => t.Phone.CountryCode == countryCode && t.Phone.Prefix == prefix && t.Phone.Number == number && (t.TransactionType == TransactionType.TransactionalSms
                || t.TransactionType == TransactionType.TransactionalTemplatedSms)
                && t.CreatedAt >= startDate && t.CreatedAt < endDate);

            return (list, count);
        }
        public async Task<(IQueryable<Transaction>, int)> GetTransactionalSmsMessagesWithPhoneByCreatedNameAsync(string createdbyName,int countryCode, int prefix, int number, DateTime startDate, DateTime endDate, int page, int pageSize)
        {
            IQueryable<Transaction> list =  Context.Transactions.AsNoTracking()
                .Where(t => t.Phone.CountryCode == countryCode && t.Phone.Prefix == prefix && t.Phone.Number == number && (t.TransactionType == TransactionType.TransactionalSms
                || t.TransactionType == TransactionType.TransactionalTemplatedSms)
                && t.CreatedAt >= startDate && t.CreatedAt < endDate
                && t.CreatedBy.Name.Contains(createdbyName))
                .Include(t => t.SmsRequestLog).ThenInclude(s => s.ResponseLogs)
                .OrderByDescending(t => t.CreatedAt)
                 .Select(s => new Transaction
                 {
                     CustomerNo = s.CustomerNo,
                     CitizenshipNo = s.CitizenshipNo,
                     CreatedAt = s.CreatedAt,
                     CreatedBy = s.CreatedBy,
                     Phone = s.Phone,
                     Mail = s.Mail,
                     TransactionType = s.TransactionType,
                     SmsRequestLog = s.SmsRequestLog
                 })
                .Skip(page * pageSize)
                .Take(pageSize)
                ;

            int count = await Context.Transactions
                .CountAsync(t => t.Phone.CountryCode == countryCode && t.Phone.Prefix == prefix && t.Phone.Number == number && (t.TransactionType == TransactionType.TransactionalSms
                || t.TransactionType == TransactionType.TransactionalTemplatedSms)
                && t.CreatedAt >= startDate && t.CreatedAt < endDate
                && t.CreatedBy.Name.Contains(createdbyName));

            return (list, count);
        }
        public async Task<(IEnumerable<Transaction>,int)> GetTransactionalSmsMessagesWithCustomerNoAsync(ulong customerNo, DateTime startDate, DateTime endDate, int page, int pageSize)
        {
            IEnumerable<Transaction> list = await Context.Transactions
                .Where(t => t.CustomerNo == customerNo &&
                (t.TransactionType == TransactionType.TransactionalSms
                || t.TransactionType == TransactionType.TransactionalTemplatedSms)
                && t.CreatedAt >= startDate && t.CreatedAt < endDate)
                .Include(t => t.SmsRequestLog).ThenInclude(s => s.ResponseLogs)
                .OrderByDescending(t => t.CreatedAt)
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToListAsync();

            int count = await Context.Transactions
                .CountAsync(t => t.CustomerNo == customerNo &&
                (t.TransactionType == TransactionType.TransactionalSms
                || t.TransactionType == TransactionType.TransactionalTemplatedSms)
                && t.CreatedAt >= startDate && t.CreatedAt < endDate);

            return (list, count);
        }
        public async Task<(IQueryable<Transaction>, int)> GetTransactionalSmsMessagesWithCustomerNoByCreatedNameAsync(string createdbyName, ulong customerNo, DateTime startDate, DateTime endDate, int page, int pageSize)
        {
            IQueryable<Transaction> list =  Context.Transactions.AsNoTracking()
                .Where(t => t.CustomerNo == customerNo &&
                (t.TransactionType == TransactionType.TransactionalSms
                || t.TransactionType == TransactionType.TransactionalTemplatedSms)
                && t.CreatedAt >= startDate && t.CreatedAt < endDate
                  && t.CreatedBy.Name.Contains(createdbyName))
                .Include(t => t.SmsRequestLog).ThenInclude(s => s.ResponseLogs)
                .OrderByDescending(t => t.CreatedAt)
                .Select(s => new Transaction
                  {
                      CustomerNo = s.CustomerNo,
                      CitizenshipNo = s.CitizenshipNo,
                      CreatedAt = s.CreatedAt,
                      CreatedBy = s.CreatedBy,
                      Phone = s.Phone,
                      Mail = s.Mail,
                      TransactionType = s.TransactionType,
                      SmsRequestLog = s.SmsRequestLog
                 })
                .Skip(page * pageSize)
                .Take(pageSize)
                ;

            int count = await Context.Transactions
                .CountAsync(t => t.CustomerNo == customerNo &&
                (t.TransactionType == TransactionType.TransactionalSms
                || t.TransactionType == TransactionType.TransactionalTemplatedSms)
                && t.CreatedAt >= startDate && t.CreatedAt < endDate
                  && t.CreatedBy.Name.Contains(createdbyName));

            return (list, count);
        }
        public async Task<(IEnumerable<Transaction>,int)> GetTransactionalSmsMessagesWithCitizenshipNoAsync(string citizenshipNo, DateTime startDate, DateTime endDate, int page, int pageSize)
        {
            IEnumerable<Transaction> list = await Context.Transactions
                .Where(t => t.CitizenshipNo == citizenshipNo &&
                (t.TransactionType == TransactionType.TransactionalSms
                || t.TransactionType == TransactionType.TransactionalTemplatedSms)
                && t.CreatedAt >= startDate && t.CreatedAt < endDate)
                .Include(t => t.SmsRequestLog).ThenInclude(s => s.ResponseLogs)
                .OrderByDescending(t => t.CreatedAt)
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToListAsync();


            int count = await Context.Transactions
                .CountAsync(t => t.CitizenshipNo == citizenshipNo &&
                (t.TransactionType == TransactionType.TransactionalSms
                || t.TransactionType == TransactionType.TransactionalTemplatedSms)
                && t.CreatedAt >= startDate && t.CreatedAt < endDate);

            return (list, count);
        }
        public async Task<(IQueryable<Transaction>, int)> GetTransactionalSmsMessagesWithCitizenshipNoByCreatedNameAsync(string createdbyName, string citizenshipNo, DateTime startDate, DateTime endDate, int page, int pageSize)
        {
            IQueryable<Transaction> list =  Context.Transactions.AsNoTracking()
                .Where(t => t.CitizenshipNo == citizenshipNo &&
                (t.TransactionType == TransactionType.TransactionalSms
                || t.TransactionType == TransactionType.TransactionalTemplatedSms)
                && t.CreatedAt >= startDate && t.CreatedAt < endDate
                 && t.CreatedBy.Name.Contains(createdbyName))
                .Include(t => t.SmsRequestLog).ThenInclude(s => s.ResponseLogs)
                .OrderByDescending(t => t.CreatedAt)
                .Select(s => new Transaction
                {
                    CustomerNo = s.CustomerNo,
                    CitizenshipNo = s.CitizenshipNo,
                    CreatedAt = s.CreatedAt,
                    CreatedBy = s.CreatedBy,
                    Phone = s.Phone,
                    Mail = s.Mail,
                    TransactionType = s.TransactionType,
                    SmsRequestLog = s.SmsRequestLog
                })
                .Skip(page * pageSize)
                .Take(pageSize)
                ;


            int count = await Context.Transactions
                .CountAsync(t => t.CitizenshipNo == citizenshipNo &&
                (t.TransactionType == TransactionType.TransactionalSms
                || t.TransactionType == TransactionType.TransactionalTemplatedSms)
                && t.CreatedAt >= startDate && t.CreatedAt < endDate
                 && t.CreatedBy.Name.Contains(createdbyName));

            return (list, count);
        }
        public async Task<(IQueryable<Transaction>, int)> GetMailMessagesWithMailAsync(string createdName, string email, DateTime startDate, DateTime endDate, int page, int pageSize)
        {
            IQueryable<Transaction> list =  Context.Transactions.AsNoTracking()
            .Where(t => t.Mail == email &&
                (t.TransactionType == TransactionType.TransactionalMail
                || t.TransactionType == TransactionType.TransactionalTemplatedMail) &&
                t.CreatedAt >= startDate && t.CreatedAt <= endDate
                  && t.CreatedBy.Name.Contains(createdName))
            .Include(t => t.MailRequestLog).ThenInclude(s => s.ResponseLogs)
            .OrderByDescending(t => t.CreatedAt)
                .Select(s => new Transaction
                {
                    CustomerNo = s.CustomerNo,
                    CitizenshipNo = s.CitizenshipNo,
                    CreatedAt = s.CreatedAt,
                    CreatedBy = s.CreatedBy,
                    Phone = s.Phone,
                    Mail = s.Mail,
                    TransactionType = s.TransactionType,
                    MailRequestLog = s.MailRequestLog
                })
                .Skip(page * pageSize)
                .Take(pageSize)
                ;

            var count = await Context.Transactions
            .CountAsync(t => t.Mail == email &&
                (t.TransactionType == TransactionType.TransactionalMail
                || t.TransactionType == TransactionType.TransactionalTemplatedMail) &&
                t.CreatedAt >= startDate && t.CreatedAt <= endDate
                  && t.CreatedBy.Name.Contains(createdName));

            return (list, count);
        }

        public async Task<(IQueryable<Transaction>, int)> GetMailMessagesWithCustomerNoAsync(string createdName, ulong customerNo, DateTime startDate, DateTime endDate, int page, int pageSize)
        {
            IQueryable<Transaction> list =  Context.Transactions.AsNoTracking()
            .Where(t => t.CustomerNo == customerNo &&
            (t.TransactionType == TransactionType.TransactionalMail
                || t.TransactionType == TransactionType.TransactionalTemplatedMail) &&
            t.CreatedAt >= startDate && t.CreatedAt <= endDate
              && t.CreatedBy.Name.Contains(createdName))
            .Include(t => t.MailRequestLog).ThenInclude(s => s.ResponseLogs)
            .OrderByDescending(t => t.CreatedAt)
            .Select(s => new Transaction
            {
                CustomerNo = s.CustomerNo,
                CitizenshipNo = s.CitizenshipNo,
                CreatedAt = s.CreatedAt,
                CreatedBy = s.CreatedBy,
                Phone = s.Phone,
                Mail = s.Mail,
                TransactionType = s.TransactionType,
                MailRequestLog = s.MailRequestLog
            })
                .Skip(page * pageSize)
                .Take(pageSize)
                ;

            int count = await Context.Transactions
            .CountAsync(t => t.CustomerNo == customerNo &&
            (t.TransactionType == TransactionType.TransactionalMail
                || t.TransactionType == TransactionType.TransactionalTemplatedMail) &&
            t.CreatedAt >= startDate && t.CreatedAt <= endDate
              && t.CreatedBy.Name.Contains(createdName));

            return (list, count);
        }
        public async Task<(IQueryable<Transaction>, int)> GetMailMessagesWithCitizenshipNoAsync(string createdName, string citizenshipNo, DateTime startDate, DateTime endDate, int page, int pageSize)
        {
            IQueryable<Transaction> list =  Context.Transactions.AsNoTracking()
            .Where(t => t.CitizenshipNo == citizenshipNo &&
            (t.TransactionType == TransactionType.TransactionalMail
                || t.TransactionType == TransactionType.TransactionalTemplatedMail) &&
            t.CreatedAt >= startDate && t.CreatedAt <= endDate
            && t.CreatedBy.Name.Contains(createdName))
            .Include(t => t.MailRequestLog).ThenInclude(s => s.ResponseLogs)
            .OrderByDescending(t => t.CreatedAt)
            .Select(s => new Transaction
            {
                CustomerNo = s.CustomerNo,
                CitizenshipNo = s.CitizenshipNo,
                CreatedAt = s.CreatedAt,
                CreatedBy = s.CreatedBy,
                Phone = s.Phone,
                Mail = s.Mail,
                TransactionType = s.TransactionType,
                MailRequestLog = s.MailRequestLog
            })
                .Skip(page * pageSize)
                .Take(pageSize)
                ;

            int count = await Context.Transactions
            .CountAsync(t => t.CitizenshipNo == citizenshipNo &&
            (t.TransactionType == TransactionType.TransactionalMail
                || t.TransactionType == TransactionType.TransactionalTemplatedMail) &&
            t.CreatedAt >= startDate && t.CreatedAt <= endDate
            && t.CreatedBy.Name.Contains(createdName));

            return (list, count);
        }
        public  IQueryable<Transaction> GetAllMailMessagesWithCitizenshipNoAsync(string createdName, string citizenshipNo, DateTime startDate, DateTime endDate, int page, int pageSize)
        {
            IQueryable<Transaction> list =  Context.Transactions.AsNoTracking()
            .Where(t => t.CitizenshipNo == citizenshipNo &&
            (t.TransactionType == TransactionType.TransactionalMail
                || t.TransactionType == TransactionType.TransactionalTemplatedMail) &&
            t.CreatedAt >= startDate && t.CreatedAt <= endDate
            && t.CreatedBy.Name.Contains(createdName))
            .Include(t => t.MailRequestLog).ThenInclude(s => s.ResponseLogs)
            .Select(s => new Transaction
            {
                CustomerNo = s.CustomerNo,
                CitizenshipNo = s.CitizenshipNo,
                CreatedAt =s.CreatedAt,
                CreatedBy=s.CreatedBy,
                Phone=s.Phone,
                Mail=s.Mail,
                TransactionType=s.TransactionType,
                MailRequestLog=s.MailRequestLog
            })
            .OrderByDescending(t => t.CreatedAt)
                .Skip(page * pageSize)
                .Take(pageSize);

            return list;
        }

        public async Task<(IQueryable<Transaction>, int)> GetPushMessagesWithCustomerNoAsync(string createdName, ulong customerNo, DateTime startDate, DateTime endDate, int page, int pageSize)
        {
            IQueryable<Transaction> list =  Context.Transactions.AsNoTracking()
                .Where(t => (t.CustomerNo == customerNo &&
                 (t.TransactionType == TransactionType.TransactionalPush || t.TransactionType == TransactionType.TransactionalTemplatedPush) &&
                 t.CreatedAt >= startDate && t.CreatedAt <= endDate
                  && t.CreatedBy.Name.Contains(createdName)
                 ))
                .Include(t => t.PushNotificationRequestLog).ThenInclude(s => s.ResponseLogs)
                .OrderByDescending(t => t.CreatedAt)
                 .Select(s => new Transaction
                 {
                     CustomerNo = s.CustomerNo,
                     CitizenshipNo = s.CitizenshipNo,
                     CreatedAt = s.CreatedAt,
                     CreatedBy = s.CreatedBy,
                     Phone = s.Phone,
                     Mail = s.Mail,
                     TransactionType = s.TransactionType,
                     PushNotificationRequestLog = s.PushNotificationRequestLog
                 })
                .Skip(page * pageSize)
                .Take(pageSize)
                ;

            int count = await Context.Transactions
                .CountAsync(t => (t.CustomerNo == customerNo &&
                 (t.TransactionType == TransactionType.TransactionalPush || t.TransactionType == TransactionType.TransactionalTemplatedPush) &&
                 t.CreatedAt >= startDate && t.CreatedAt <= endDate
                 &&t.CreatedBy.Name.Contains(createdName)
                 ));

            return (list, count);
        }

        public async Task<(IQueryable<Transaction>, int)> GetPushMessagesWithCitizenshipNoAsync(string createdName, string citizenshipNo, DateTime startDate, DateTime endDate, int page, int pageSize)
        {
            IQueryable<Transaction> list =  Context.Transactions.AsNoTracking()
                .Where(t => (t.CitizenshipNo == citizenshipNo &&
                (t.TransactionType == TransactionType.TransactionalPush || t.TransactionType == TransactionType.TransactionalTemplatedPush) &&
                t.CreatedAt >= startDate && t.CreatedAt <= endDate
                  && t.CreatedBy.Name.Contains(createdName)
                ))
                .Include(t => t.PushNotificationRequestLog).ThenInclude(s => s.ResponseLogs)
                .OrderByDescending(t => t.CreatedAt)
                .Select(s => new Transaction
                {
                    CustomerNo = s.CustomerNo,
                    CitizenshipNo = s.CitizenshipNo,
                    CreatedAt = s.CreatedAt,
                    CreatedBy = s.CreatedBy,
                    Phone = s.Phone,
                    Mail = s.Mail,
                    TransactionType = s.TransactionType,
                    PushNotificationRequestLog = s.PushNotificationRequestLog
                })
                .Skip(page * pageSize)
                .Take(pageSize)
                ;

            int count = await Context.Transactions
                .CountAsync(t => (t.CitizenshipNo == citizenshipNo &&
                (t.TransactionType == TransactionType.TransactionalPush || t.TransactionType == TransactionType.TransactionalTemplatedPush) &&
                t.CreatedAt >= startDate && t.CreatedAt <= endDate
                  && t.CreatedBy.Name.Contains(createdName)
                ));

            return (list, count);

        }

        public async Task<IEnumerable<Transaction>> GetReportTransaction(int phoneNumber, string date, string message)
        {
            var transactions = await Context.Transactions.AsNoTracking().Where(t =>
                t.Phone.Number == phoneNumber &&
                    t.CreatedAt >= DateTime.ParseExact(date + " 00:00:00.000000", "dd/MM/yyyy HH:mm:ss.ffffff", null) &&
                    t.CreatedAt <= DateTime.ParseExact(date + " 23:59:59.999999", "dd/MM/yyyy HH:mm:ss.ffffff", null) &&
                    t.Request.Contains(message)
                )
                .Include(t => t.SmsRequestLog).ThenInclude(s => s.ResponseLogs)
                .OrderBy(t => t.CreatedAt).ToListAsync();

            return transactions;
        }

        public async Task<int> GetForeignSmsRequestCount(DateTime startDate, DateTime endDate, OperatorType @operator, bool isSuccess)
        {
            if (isSuccess)
            {
                return await Context.Transactions.CountAsync(t =>
                    t.CreatedAt.Date >= startDate.Date &&
                        t.CreatedAt.Date <= endDate.Date &&
                        t.Phone.CountryCode != 0 &&
                            t.Phone.CountryCode != 90 &&
                        t.SmsRequestLog.ResponseLogs.Any(r => r.Operator == @operator && r.OperatorResponseCode == 0)
                );
            }
            else
            {
                return await Context.Transactions.CountAsync(t =>
                    t.CreatedAt.Date >= startDate.Date &&
                        t.CreatedAt.Date <= endDate.Date &&
                        t.Phone.CountryCode != 0 &&
                            t.Phone.CountryCode != 90 &&
                        t.SmsRequestLog.ResponseLogs.Any(r => r.Operator == @operator && r.OperatorResponseCode != 0)
                );
            }

        }

        public async Task<int> GetSmsRequestCount(DateTime startDate, DateTime endDate, OperatorType @operator,bool isSuccess)
        {
            if (isSuccess)
            {
                return await Context.Transactions.CountAsync(t =>
                    t.CreatedAt.Date >= startDate.Date &&
                        t.CreatedAt.Date <= endDate.Date &&
                        t.SmsRequestLog.ResponseLogs.Any(r => r.Operator == @operator && r.OperatorResponseCode == 0)
                );
            }
            else
            {
                return await Context.Transactions.CountAsync(t =>
                    t.CreatedAt.Date >= startDate.Date &&
                        t.CreatedAt.Date <= endDate.Date &&
                        t.SmsRequestLog.ResponseLogs.Any(r => r.Operator == @operator && r.OperatorResponseCode != 0)
                );
            }

        }

        public async Task<int> GetForeignOtpRequestCount(DateTime startDate, DateTime endDate, OperatorType @operator, bool isSuccess)
        {
            if (isSuccess)
            {
                
                if (@operator == OperatorType.Turkcell)
                {
                    var foreignCount = await Context.Transactions.CountAsync(t =>
                        t.CreatedAt.Date >= startDate.Date &&
                            t.CreatedAt.Date <= endDate.Date &&
                            t.Phone.CountryCode != 0 &&
                            t.OtpRequestLog.ResponseLogs.Any(r => r.Operator == OperatorType.Foreign && (r.ResponseCode == SendSmsResponseStatus.Success
                            || r.ResponseCode != SendSmsResponseStatus.SimChange
                            || r.ResponseCode != SendSmsResponseStatus.OperatorChange
                            || r.ResponseCode != SendSmsResponseStatus.HasBlacklistRecord)
                            )
                    );

                    var turkcellCount = await Context.Transactions.CountAsync(t =>
                        t.CreatedAt.Date >= startDate.Date &&
                            t.CreatedAt.Date <= endDate.Date &&
                            t.Phone.CountryCode != 0 &&
                            t.Phone.CountryCode != 90 &&
                            t.OtpRequestLog.ResponseLogs.Any(r => r.Operator == OperatorType.Turkcell && (r.ResponseCode == SendSmsResponseStatus.Success
                            || r.ResponseCode != SendSmsResponseStatus.SimChange
                            || r.ResponseCode != SendSmsResponseStatus.OperatorChange
                            || r.ResponseCode != SendSmsResponseStatus.HasBlacklistRecord)
                            )
                    );

                    return foreignCount + turkcellCount;
                }

                return await Context.Transactions.CountAsync(t =>
                           t.CreatedAt.Date >= startDate.Date &&
                            t.CreatedAt.Date <= endDate.Date &&
                            t.Phone.CountryCode != 0 &&
                            t.Phone.CountryCode != 90 &&
                                t.OtpRequestLog.ResponseLogs.Any(r => r.Operator == @operator && (r.ResponseCode == SendSmsResponseStatus.Success
                            || r.ResponseCode != SendSmsResponseStatus.SimChange
                            || r.ResponseCode != SendSmsResponseStatus.OperatorChange
                            || r.ResponseCode != SendSmsResponseStatus.HasBlacklistRecord)
                            )
                        );
            }
            else
            {
                if (@operator == OperatorType.Turkcell)
                {
                    var foreignCount = await Context.Transactions.CountAsync(t =>
                        t.CreatedAt.Date >= startDate.Date &&
                            t.CreatedAt.Date <= endDate.Date &&
                            t.Phone.CountryCode != 0 &&
                            t.OtpRequestLog.ResponseLogs.Any(r => r.Operator == OperatorType.Foreign && r.ResponseCode != SendSmsResponseStatus.Success
                            && r.ResponseCode != SendSmsResponseStatus.SimChange
                            && r.ResponseCode != SendSmsResponseStatus.OperatorChange
                            && r.ResponseCode != SendSmsResponseStatus.HasBlacklistRecord)
                    );

                    var turkcellCount = await Context.Transactions.CountAsync(t =>
                        t.CreatedAt.Date >= startDate.Date &&
                            t.CreatedAt.Date <= endDate.Date &&
                            t.Phone.CountryCode != 0 &&
                            t.Phone.CountryCode != 90 &&
                            t.OtpRequestLog.ResponseLogs.Any(r => r.Operator == OperatorType.Turkcell && r.ResponseCode != SendSmsResponseStatus.Success
                            && r.ResponseCode != SendSmsResponseStatus.SimChange
                            && r.ResponseCode != SendSmsResponseStatus.OperatorChange
                            && r.ResponseCode != SendSmsResponseStatus.HasBlacklistRecord)
                    );

                    return foreignCount + turkcellCount;
                }
                

                return await Context.Transactions.CountAsync(t =>
                           t.CreatedAt.Date >= startDate.Date &&
                            t.CreatedAt.Date <= endDate.Date &&
                            t.Phone.CountryCode != 0 &&
                            t.Phone.CountryCode != 90 &&
                                t.OtpRequestLog.ResponseLogs.Any(r => r.Operator == @operator && r.ResponseCode != SendSmsResponseStatus.Success
                                && r.ResponseCode != SendSmsResponseStatus.SimChange
                                && r.ResponseCode != SendSmsResponseStatus.OperatorChange
                                && r.ResponseCode != SendSmsResponseStatus.HasBlacklistRecord)
                        );
            }

        }

        public async Task<int> GetOtpRequestCount(DateTime startDate, DateTime endDate, OperatorType @operator, bool isSuccess)
        {
            if (isSuccess)
            {
                if (@operator == OperatorType.Foreign)
                {
                    var foreignCount = await Context.Transactions.CountAsync(t =>
                        t.CreatedAt.Date >= startDate.Date &&
                            t.CreatedAt.Date <= endDate.Date &&
                            t.Phone.CountryCode != 0 &&
                            t.OtpRequestLog.ResponseLogs.Any(r => r.Operator == @operator && (r.ResponseCode == SendSmsResponseStatus.Success
                            || r.ResponseCode != SendSmsResponseStatus.SimChange
                            || r.ResponseCode != SendSmsResponseStatus.OperatorChange
                            || r.ResponseCode != SendSmsResponseStatus.HasBlacklistRecord)
                            )
                    );

                    var turkcellCount = await Context.Transactions.CountAsync(t =>
                        t.CreatedAt.Date >= startDate.Date &&
                            t.CreatedAt.Date <= endDate.Date &&
                            t.Phone.CountryCode != 0 &&
                            t.Phone.CountryCode != 90 &&
                            t.OtpRequestLog.ResponseLogs.Any(r => r.Operator == OperatorType.Turkcell && (r.ResponseCode == SendSmsResponseStatus.Success
                            || r.ResponseCode != SendSmsResponseStatus.SimChange
                            || r.ResponseCode != SendSmsResponseStatus.OperatorChange
                            || r.ResponseCode != SendSmsResponseStatus.HasBlacklistRecord)
                            )
                    );

                    return foreignCount + turkcellCount;
                }
                if (@operator == OperatorType.Turkcell)
                {
                    return await Context.Transactions.CountAsync(t =>
                           t.CreatedAt.Date >= startDate.Date &&
                            t.CreatedAt.Date <= endDate.Date &&
                                t.Phone.CountryCode == 90 &&
                                t.OtpRequestLog.ResponseLogs.Any(r => r.Operator == @operator && (r.ResponseCode == SendSmsResponseStatus.Success
                            || r.ResponseCode != SendSmsResponseStatus.SimChange
                            || r.ResponseCode != SendSmsResponseStatus.OperatorChange
                            || r.ResponseCode != SendSmsResponseStatus.HasBlacklistRecord)
                            )
                        );
                }

                return await Context.Transactions.CountAsync(t =>
                           t.CreatedAt.Date >= startDate.Date &&
                            t.CreatedAt.Date <= endDate.Date &&
                                t.OtpRequestLog.ResponseLogs.Any(r => r.Operator == @operator && (r.ResponseCode == SendSmsResponseStatus.Success
                            || r.ResponseCode != SendSmsResponseStatus.SimChange
                            || r.ResponseCode != SendSmsResponseStatus.OperatorChange
                            || r.ResponseCode != SendSmsResponseStatus.HasBlacklistRecord)
                            )
                        );
            }
            else
            {
                if (@operator == OperatorType.Foreign)
                {
                    var foreignCount = await Context.Transactions.CountAsync(t =>
                        t.CreatedAt.Date >= startDate.Date &&
                            t.CreatedAt.Date <= endDate.Date &&
                            t.Phone.CountryCode != 0 &&
                            t.OtpRequestLog.ResponseLogs.Any(r => r.Operator == @operator && r.ResponseCode != SendSmsResponseStatus.Success
                            && r.ResponseCode != SendSmsResponseStatus.SimChange
                            && r.ResponseCode != SendSmsResponseStatus.OperatorChange
                            && r.ResponseCode != SendSmsResponseStatus.HasBlacklistRecord)
                    );

                    var turkcellCount = await Context.Transactions.CountAsync(t =>
                        t.CreatedAt.Date >= startDate.Date &&
                            t.CreatedAt.Date <= endDate.Date &&
                            t.Phone.CountryCode != 0 &&
                            t.Phone.CountryCode != 90 &&
                            t.OtpRequestLog.ResponseLogs.Any(r => r.Operator == OperatorType.Turkcell && r.ResponseCode != SendSmsResponseStatus.Success
                            && r.ResponseCode != SendSmsResponseStatus.SimChange
                            && r.ResponseCode != SendSmsResponseStatus.OperatorChange
                            && r.ResponseCode != SendSmsResponseStatus.HasBlacklistRecord)
                    );

                    return foreignCount + turkcellCount;
                }
                if (@operator == OperatorType.Turkcell)
                {
                    return await Context.Transactions.CountAsync(t =>
                           t.CreatedAt.Date >= startDate.Date &&
                            t.CreatedAt.Date <= endDate.Date &&
                                t.Phone.CountryCode == 90 &&
                                t.OtpRequestLog.ResponseLogs.Any(r => r.Operator == @operator && r.ResponseCode != SendSmsResponseStatus.Success
                                && r.ResponseCode != SendSmsResponseStatus.SimChange
                                && r.ResponseCode != SendSmsResponseStatus.OperatorChange
                                && r.ResponseCode != SendSmsResponseStatus.HasBlacklistRecord)
                        );
                }

                return await Context.Transactions.CountAsync(t =>
                           t.CreatedAt.Date >= startDate.Date &&
                            t.CreatedAt.Date <= endDate.Date &&
                                t.OtpRequestLog.ResponseLogs.Any(r => r.Operator == @operator && r.ResponseCode != SendSmsResponseStatus.Success
                                && r.ResponseCode != SendSmsResponseStatus.SimChange
                                && r.ResponseCode != SendSmsResponseStatus.OperatorChange
                                && r.ResponseCode != SendSmsResponseStatus.HasBlacklistRecord)
                        );
            }

        }

        public async Task<int> GetSuccessfullForeignSmsCount(DateTime startDate, DateTime endDate, OperatorType @operator)
        {

            return await Context.Transactions.CountAsync(t =>
                t.CreatedAt.Date >= startDate.Date &&
                        t.CreatedAt.Date <= endDate.Date &&
                        t.Phone.Number != 90 && t.Phone.Number != 0 &&
                    t.SmsRequestLog.ResponseLogs.Any(r => r.Operator == @operator && r.OperatorResponseCode == 0)
            );

        }

        public async Task<int> GetSuccessfullSmsCount(DateTime startDate, DateTime endDate, OperatorType @operator)
        {
            
                return await Context.Transactions.CountAsync(t =>
                    t.CreatedAt.Date >= startDate.Date &&
                            t.CreatedAt.Date <= endDate.Date &&
                        t.SmsRequestLog.ResponseLogs.Any(r => r.Operator == @operator && r.OperatorResponseCode == 0)
                );
            
        }

        public async Task<int> GetSuccessfullForeignOtpCount(DateTime startDate, DateTime endDate, OperatorType @operator)
        {
            
            if (@operator == OperatorType.Turkcell)
            {
                var foreignCount = await Context.Transactions.CountAsync(t =>
                     t.CreatedAt.Date >= startDate.Date &&
                             t.CreatedAt.Date <= endDate.Date &&
                         t.Phone.CountryCode != 0 &&
                         t.OtpRequestLog.ResponseLogs.Any(r => r.Operator == OperatorType.Foreign && r.ResponseCode == SendSmsResponseStatus.Success)
                 );

                var turkcellCount = await Context.Transactions.CountAsync(t =>
                    t.CreatedAt.Date >= startDate.Date &&
                            t.CreatedAt.Date <= endDate.Date &&
                        t.Phone.CountryCode != 0 &&
                        t.Phone.CountryCode != 90 &&
                        t.OtpRequestLog.ResponseLogs.Any(r => r.Operator == OperatorType.Turkcell && r.ResponseCode == SendSmsResponseStatus.Success)
                );

                return foreignCount + turkcellCount;
            }

            return await Context.Transactions.CountAsync(t =>
                       t.CreatedAt.Date >= startDate.Date &&
                            t.CreatedAt.Date <= endDate.Date &&
                            t.Phone.CountryCode != 0 &&
                            t.Phone.CountryCode != 90 &&
                            t.OtpRequestLog.ResponseLogs.Any(r => r.Operator == @operator && r.ResponseCode == SendSmsResponseStatus.Success)
                    );
        }

        public async Task<int> GetSuccessfullOtpCount(DateTime startDate, DateTime endDate, OperatorType @operator)
        {
            if (@operator == OperatorType.Foreign)
            {
                var foreignCount = await Context.Transactions.CountAsync(t =>
                    t.CreatedAt.Date >= startDate.Date &&
                            t.CreatedAt.Date <= endDate.Date &&
                        t.Phone.CountryCode != 0 &&
                        t.OtpRequestLog.ResponseLogs.Any(r => r.Operator == @operator && r.ResponseCode == SendSmsResponseStatus.Success)
                );

                var turkcellCount = await Context.Transactions.CountAsync(t =>
                    t.CreatedAt.Date >= startDate.Date &&
                            t.CreatedAt.Date <= endDate.Date &&
                        t.Phone.CountryCode != 0 &&
                        t.Phone.CountryCode != 90 &&
                        t.OtpRequestLog.ResponseLogs.Any(r => r.Operator == OperatorType.Turkcell && r.ResponseCode == SendSmsResponseStatus.Success)
                );

                return foreignCount + turkcellCount;
            }
            if (@operator == OperatorType.Turkcell)
            {
                return await Context.Transactions.CountAsync(t =>
                       t.CreatedAt.Date >= startDate.Date &&
                            t.CreatedAt.Date <= endDate.Date &&
                            t.Phone.CountryCode == 90 &&
                            t.OtpRequestLog.ResponseLogs.Any(r => r.Operator == @operator && r.ResponseCode == SendSmsResponseStatus.Success)
                    );
            }

            return await Context.Transactions.CountAsync(t =>
                       t.CreatedAt.Date >= startDate.Date &&
                            t.CreatedAt.Date <= endDate.Date &&
                            t.OtpRequestLog.ResponseLogs.Any(r => r.Operator == @operator && r.ResponseCode == SendSmsResponseStatus.Success)
                    );
        }
    }
}
