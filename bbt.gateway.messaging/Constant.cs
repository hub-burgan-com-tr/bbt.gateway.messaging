using bbt.gateway.common.Models;
using System.Collections.Generic;

namespace bbt.gateway.messaging
{
    public class Constant
    {
        public static readonly Dictionary<string, dEngageResponseCodes> dEngageStatusCodes = new()
        {
            { "0", dEngageResponseCodes.Success },
            { "400", dEngageResponseCodes.BadRequest },
            { "401", dEngageResponseCodes.Unauthorized },
            { "403", dEngageResponseCodes.NotAllowed },
            { "404", dEngageResponseCodes.NotFound },
            { "429", dEngageResponseCodes.TooManyRequest },
        };

        public static readonly Dictionary<int, CodecResponseCodes> CodecStatusCodes = new()
        {
            { 0, CodecResponseCodes.Success },
            { -441, CodecResponseCodes.UndefinedSender },
            { -103, CodecResponseCodes.NotAuthorized},
        };

        public static readonly Dictionary<SmsTypes, string> smsTypes = new()
        {
            { SmsTypes.Bulk, "codec" },
            { SmsTypes.Fast, "codec-fast" }
        };

        public static readonly Dictionary<OperatorType, string> OperatorBurganSenderNames =
        new()
        {
            { OperatorType.Turkcell, "BURGANBANK" },
            { OperatorType.Vodafone, "BURGANBANK" },
            { OperatorType.TurkTelekom, "BURGAN BANK" }
        };

        public static readonly Dictionary<OperatorType, string> OperatorOnSenderNames =
        new()
        {
            { OperatorType.Turkcell, "ON." },
            { OperatorType.Vodafone, "ON." },
            { OperatorType.TurkTelekom, "ON." }
        };

        public static readonly Dictionary<SenderType, Dictionary<OperatorType, string>> OperatorSenders =
        new()
        {
            {SenderType.Burgan ,OperatorBurganSenderNames },
            {SenderType.On,OperatorOnSenderNames}
        };

        public static readonly Dictionary<string, SmsApiResponse> TurkTelekomErrorCodes =
        new()
        {
            { "0", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.Success} },
            { "1", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "2", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "3", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "4", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "5", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "6", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "7", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "8", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.NotSubscriber } },
            { "9", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.SimChange} },
            { "10", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "11", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "12", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "13", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "14", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "15", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.OperatorChange } },
            { "16", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "17", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "19", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "20", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "21", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "29", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-1", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "50", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "51", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "52", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "53", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "54", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "55", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "34", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "35", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "36", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "37", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "38", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "30", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-99999", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.ClientError } }
        };

        public static readonly Dictionary<string, SmsApiResponse> VodafoneErrorCodes =
        new()
        {
            { "100", new SmsApiResponse { ReturnMessage = "OTP request accepted", SmsResponseStatus = SendSmsResponseStatus.Success } },
            { "101", new SmsApiResponse { ReturnMessage = "Sim Popup has successfully been delivered to subscriber.", SmsResponseStatus = SendSmsResponseStatus.Success } },
            { "102", new SmsApiResponse { ReturnMessage = "OTP SMS was sent instead of Sim Popup has been failed", SmsResponseStatus = SendSmsResponseStatus.ServerError } },
            { "300", new SmsApiResponse { ReturnMessage = "OTP SMS has successfully been delivered to subscriber.", SmsResponseStatus = SendSmsResponseStatus.Success } },
            { "400", new SmsApiResponse { ReturnMessage = "OTP SMS has been expired.", SmsResponseStatus = SendSmsResponseStatus.ServerError } },
            { "600", new SmsApiResponse { ReturnMessage = "OTP SMS has been failed", SmsResponseStatus = SendSmsResponseStatus.ServerError } },
            { "601", new SmsApiResponse { ReturnMessage = "MSISDN is not a Vodafone subscriber number", SmsResponseStatus = SendSmsResponseStatus.NotSubscriber } },
            { "602", new SmsApiResponse { ReturnMessage = "Subscriber is in MNP-PORT-IN state at delivery time", SmsResponseStatus = SendSmsResponseStatus.OperatorChange } },
            { "603", new SmsApiResponse { ReturnMessage = "Subscriber has changed SIMCARD within the specified period of time", SmsResponseStatus = SendSmsResponseStatus.SimChange } },
            { "604", new SmsApiResponse { ReturnMessage = "Subscriber is in barring status. SMS could not be delivered.", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "605", new SmsApiResponse { ReturnMessage = "Encryption parameter is wrong", SmsResponseStatus = SendSmsResponseStatus.ServerError } },
            { "606", new SmsApiResponse { ReturnMessage = "Message encryption failure", SmsResponseStatus = SendSmsResponseStatus.ServerError } },
            { "607", new SmsApiResponse { ReturnMessage = "Message format failure, missing parameter", SmsResponseStatus = SendSmsResponseStatus.ServerError } },
            { "608", new SmsApiResponse { ReturnMessage = "OTP request size limit exceeded", SmsResponseStatus = SendSmsResponseStatus.ServerError } },
            { "609", new SmsApiResponse { ReturnMessage = "Message size limit exceeded", SmsResponseStatus = SendSmsResponseStatus.ServerError } },
            { "610", new SmsApiResponse { ReturnMessage = "International foreign MSISDN not supported", SmsResponseStatus = SendSmsResponseStatus.ServerError } },
            { "611", new SmsApiResponse { ReturnMessage = "System error.", SmsResponseStatus = SendSmsResponseStatus.ServerError } },
            { "612", new SmsApiResponse { ReturnMessage = "Delivery cancelled due to MNP-PORT-IN check. OTP SMS has not been delivered. Notification SMS sent to subscriber.", SmsResponseStatus = SendSmsResponseStatus.Success } },
            { "613", new SmsApiResponse { ReturnMessage = "Delivery cancelled due to SIMCARD CHANGE check. OTP SMS has not been delivered. Notification SMS sent to subscriber.", SmsResponseStatus = SendSmsResponseStatus.Success } },
            { "615", new SmsApiResponse { ReturnMessage = "Sim Popup parameter error", SmsResponseStatus = SendSmsResponseStatus.ServerError } },
            { "616", new SmsApiResponse { ReturnMessage = "No Sim Popup support", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-99999", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.ClientError } }
        };

        public static readonly Dictionary<string, SmsApiResponse> TurkcellErrorCodes =
        new()
        {
            { "0", new SmsApiResponse { ReturnMessage = "Sms talebi gönderildi", SmsResponseStatus = SendSmsResponseStatus.Success } },
            { "-1", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-2", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-101", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-102", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-103", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-104", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-125", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-126", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-127", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-128", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-129", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-132", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.NotSubscriber } },
            { "-200", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-201", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-202", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-203", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-226", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-227", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-228", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-229", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-230", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-242", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-243", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-244", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-300", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-401", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-422", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-423", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-424", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-425", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-426", new SmsApiResponse { ReturnMessage = "İlgili numaraya mesaj gönderim izni yok", SmsResponseStatus = SendSmsResponseStatus.NotSubscriber } },
            { "-427", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-428", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-429", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-430", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-431", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-432", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-433", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-434", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-436", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-437", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-438", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-439", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-440", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-441", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-442", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-443", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-444", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-445", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-446", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-447", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-448", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-449", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-450", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-451", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-461", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-462", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-463", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-464", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-465", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-466", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-467", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-700", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-801", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-802", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-803", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-808", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-1101", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-1102", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-1103", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-1104", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-1105", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-1107", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-1108", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-1109", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-2098", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-9005", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-233", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-234", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-452", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-453", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.SimChange } },
            { "-553", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.OperatorChange } },
            { "-11001", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-12001", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-12002", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-13001", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-13002", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-13003", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-13004", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-13005", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-13006", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-13007", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-13008", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-13009", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-13010", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-14001", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-14002", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-14241", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-15001", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-15002", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-15004", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-15005", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-15006", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-15007", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-15008", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-15009", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-15010", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-15011", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-15012", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-15013", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-15014", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-15015", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-15016", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-15017", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-15241", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.RejectedByOperator } },
            { "-99999", new SmsApiResponse { ReturnMessage = "", SmsResponseStatus = SendSmsResponseStatus.ClientError} }
        };

        public static readonly Dictionary<OperatorType, Dictionary<string, SmsApiResponse>> OperatorErrorCodes =
        new()
        {
            { OperatorType.TurkTelekom, TurkTelekomErrorCodes },
            { OperatorType.Vodafone, VodafoneErrorCodes },
            { OperatorType.Turkcell, TurkcellErrorCodes },
        };

        public static readonly Dictionary<string, SmsApiTrackingResponse> TurkTelekomTrackingErrorCodes =
        new()
        {
            { "0", new SmsApiTrackingResponse { ReturnMessage = "", SmsTrackingStatus = SmsTrackingStatus.Delivered } },
            { "-2", new SmsApiTrackingResponse { ReturnMessage = "", SmsTrackingStatus = SmsTrackingStatus.Expired } },
            { "-5", new SmsApiTrackingResponse { ReturnMessage = "", SmsTrackingStatus = SmsTrackingStatus.SystemError } },
            { "1", new SmsApiTrackingResponse { ReturnMessage = "", SmsTrackingStatus = SmsTrackingStatus.Pending } },
            { "3", new SmsApiTrackingResponse { ReturnMessage = "", SmsTrackingStatus = SmsTrackingStatus.SystemError } },
            { "5", new SmsApiTrackingResponse { ReturnMessage = "", SmsTrackingStatus = SmsTrackingStatus.SystemError } },
            { "8", new SmsApiTrackingResponse { ReturnMessage = "", SmsTrackingStatus = SmsTrackingStatus.SystemError } },
            { "9", new SmsApiTrackingResponse { ReturnMessage = "", SmsTrackingStatus = SmsTrackingStatus.Expired } },
            { "11", new SmsApiTrackingResponse { ReturnMessage = "", SmsTrackingStatus = SmsTrackingStatus.UnknownNumber } }
        };

        public static readonly Dictionary<string, SmsApiTrackingResponse> TurkcellTrackingErrorCodes =
        new()
        {
            { "-1", new SmsApiTrackingResponse { ReturnMessage = "Geçersiz MSGID", SmsTrackingStatus = SmsTrackingStatus.SystemError } },
            { "0", new SmsApiTrackingResponse { ReturnMessage = "Sms Mesajı İletildi", SmsTrackingStatus = SmsTrackingStatus.Delivered } },
            { "1", new SmsApiTrackingResponse { ReturnMessage = "Sms Mesajı İletilemedi", SmsTrackingStatus = SmsTrackingStatus.SystemError } },
            { "2", new SmsApiTrackingResponse { ReturnMessage = "Sms Mesajı Gönderilmeyi Bekliyor", SmsTrackingStatus = SmsTrackingStatus.Pending } },
            { "3", new SmsApiTrackingResponse { ReturnMessage = "Bu Sms Mesajı İçin Notification İstenmemiş.", SmsTrackingStatus = SmsTrackingStatus.SystemError } },
            { "4", new SmsApiTrackingResponse { ReturnMessage = "Mesaj Sistem Hatası Nedeni İle İletilemedi", SmsTrackingStatus = SmsTrackingStatus.SystemError } },
            { "-99999", new SmsApiTrackingResponse { ReturnMessage = "Sms Sorgulanamadı.", SmsTrackingStatus = SmsTrackingStatus.SystemError } },

        };

        public static readonly Dictionary<string, SmsApiTrackingResponse> VodafoneTrackingErrorCodes =
        new()
        {
            { "-1", new SmsApiTrackingResponse { ReturnMessage = "Geçersiz MSGID", SmsTrackingStatus = SmsTrackingStatus.SystemError } },
            { "300", new SmsApiTrackingResponse { ReturnMessage = "Sms Mesajı İletildi", SmsTrackingStatus = SmsTrackingStatus.Delivered } },
            { "400", new SmsApiTrackingResponse { ReturnMessage = "Sms Zaman Aşımına Uğradı", SmsTrackingStatus = SmsTrackingStatus.Expired } },
            { "600", new SmsApiTrackingResponse { ReturnMessage = "Sms İletilemedi", SmsTrackingStatus = SmsTrackingStatus.SystemError } },
            { "-99999", new SmsApiTrackingResponse { ReturnMessage = "", SmsTrackingStatus = SmsTrackingStatus.SystemError } },

        };

        public static readonly Dictionary<string, SmsApiTrackingResponse> CodecTrackingErrorCodes =
        new()
        {
            { "-1", new SmsApiTrackingResponse { ReturnMessage = "Mesaj için iletim raporu bulunamadı.", SmsTrackingStatus = SmsTrackingStatus.SystemError } },
            { "0", new SmsApiTrackingResponse { ReturnMessage = "Mesaj başarılı şekilde iletildi", SmsTrackingStatus = SmsTrackingStatus.Delivered } },
            { "1", new SmsApiTrackingResponse { ReturnMessage = "Mesaj İletilemedi. Başarısız.", SmsTrackingStatus = SmsTrackingStatus.SystemError } },
            { "2", new SmsApiTrackingResponse { ReturnMessage = "Mesaj operatöre iletildi. İletim raporu bekleniyor.", SmsTrackingStatus = SmsTrackingStatus.Pending } },
            { "4", new SmsApiTrackingResponse { ReturnMessage = "Mesaj İletilemedi. Başarısız.", SmsTrackingStatus = SmsTrackingStatus.SystemError } },
            { "5", new SmsApiTrackingResponse { ReturnMessage = "Mesaj başarılı şekilde iletildi", SmsTrackingStatus = SmsTrackingStatus.Delivered } },
        };

        public static readonly Dictionary<string, SmsApiTrackingResponse> dEngageTrackingErrorCodes =
        new()
        {
            { "DL", new SmsApiTrackingResponse { ReturnMessage = "Mesaj iletildi.", SmsTrackingStatus = SmsTrackingStatus.Delivered } },
            { "WA", new SmsApiTrackingResponse { ReturnMessage = "İletim raporu bekleniyor", SmsTrackingStatus = SmsTrackingStatus.Pending } },
            { "FA", new SmsApiTrackingResponse { ReturnMessage = "Mesaj iletilemedi.", SmsTrackingStatus = SmsTrackingStatus.SystemError } },
        };

        public static readonly Dictionary<OperatorType, Dictionary<string, SmsApiTrackingResponse>> OperatorTrackingErrorCodes =
        new()
        {
            { OperatorType.TurkTelekom, TurkTelekomTrackingErrorCodes },
            { OperatorType.Vodafone, VodafoneTrackingErrorCodes},
            { OperatorType.Turkcell, TurkcellTrackingErrorCodes },
            { OperatorType.Codec, CodecTrackingErrorCodes},
            { OperatorType.dEngageBurgan, dEngageTrackingErrorCodes },
            { OperatorType.dEngageOn, dEngageTrackingErrorCodes}
        };


    }
}
