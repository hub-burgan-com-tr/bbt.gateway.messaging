
using bbt.gateway.common.Models;

namespace bbt.gateway.messaging.ui.Data
{
    public class Constants
    {
        public static List<SmsType> SmsTypes = new()
        {
            { new() { Name = "Otp", Type = SmsTypeEnum.Otp } },
            { new() { Name = "Transactional", Type = SmsTypeEnum.Transactional } },
        };

        public static List<MessageType> MessageTypes = new()
        {
            { new() { Name = "Sms", Type = MessageTypeEnum.Sms } },
            { new() { Name = "E-Mail", Type = MessageTypeEnum.Mail } },
            { new() { Name = "Push Notification", Type = MessageTypeEnum.PushNotification } }
        };

        public static Dictionary<int, FilterInput> Filters = new()
        {
            { 1, new FilterInput { Name = "Müşteri No", Helpline = "Müşteri numarasını giriniz.", MessageTypes = MessageTypes, SmsTypes = SmsTypes } },
            { 2, new FilterInput { Name = "Kimlik No", Helpline = "Müşteri kimlik numarasını giriniz.", MessageTypes = MessageTypes, SmsTypes = SmsTypes } },
            { 3, new FilterInput { Name = "Telefon No", Helpline = "Müşteri telefon numarasını boşluksuz giriniz. {ÜlkeKodu}{AlanKodu}{Numara}", MessageTypes = null, SmsTypes = SmsTypes } },
            { 4, new FilterInput { Name = "E-Mail", Helpline = "Müşteri E-Mail adresini giriniz.", MessageTypes = null, SmsTypes = null } },
        };
        public static Dictionary<int, FilterInput> FiltersBlackList = new()
        {
            { 1, new FilterInput { Name = "Telefon No", Helpline = "Müşteri telefon numarasını boşluksuz giriniz. {ÜlkeKodu}{AlanKodu}{Numara}", MessageTypes = null, SmsTypes = SmsTypes } },
            { 2, new FilterInput { Name = "Müşteri No", Helpline = "Müşteri numarasını giriniz.", MessageTypes = MessageTypes, SmsTypes = SmsTypes } },
           
        };

        public static Dictionary<TransactionType, string> TransactionTypeMap = new()
        {
            { TransactionType.Otp, "Otp" },
            { TransactionType.TransactionalPush, "Push Notification" },
            { TransactionType.TransactionalTemplatedPush, "Push Notification" },
            { TransactionType.TransactionalMail, "E-Mail" },
            { TransactionType.TransactionalTemplatedMail, "E-Mail" },
            { TransactionType.TransactionalSms, "Sms" },
            { TransactionType.TransactionalTemplatedSms, "Sms" }
        };
        public static Dictionary<BlacklistStatus, string> BlacklistStatusMap = new()
        {
            { BlacklistStatus.Resolved, "Çözüldü" },
            { BlacklistStatus.NotResolved, "Çözülmedi" },
        };
        public static Dictionary<OperatorType, string> OperatorTypeMap = new()
        {
            { OperatorType.Codec, "Codec" },
            { OperatorType.Vodafone, "Vodafone" },
            { OperatorType.Turkcell, "Turkcell" },
            { OperatorType.TurkTelekom, "Türk Telekom" },
            { OperatorType.dEngageBurgan, "Dengage" },
            { OperatorType.dEngageOn, "Dengage" },

        };

        public static Dictionary<SmsTypeEnum, int> SmsTypeMap = new()
        {
            {
                SmsTypeEnum.Unselected,
                0
            },
            {
                SmsTypeEnum.Otp,
                1
            },
            {
                SmsTypeEnum.Transactional,
                2
            }
        };

        public static Dictionary<MessageTypeEnum, int> MessageTypeMap = new()
        {
            { 
                MessageTypeEnum.Unselected,
                0
            },
            {
                MessageTypeEnum.Sms,
                1
            },
            {
                MessageTypeEnum.Mail,
                2
            },
            {
                MessageTypeEnum.PushNotification,
                3
            }
        };
    }
}
