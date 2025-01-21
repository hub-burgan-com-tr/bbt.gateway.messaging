using bbt.gateway.common.Models;
using bbt.gateway.messaging.ui.Data;
using bbt.gateway.messaging.ui.Pages.Base;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace bbt.gateway.messaging.ui.Pages
{
    public partial class SearchMessages : BaseComponent
    {
        private IEnumerable<Transaction>? transactions;
        private SearchModel searchModel = new SearchModel();
        private int pageCount = 10;
        private int rowsCount = 0;
        private bool useSpinner;
        private bool closeExcel = false;
        private RadzenDataGrid<Transaction> grid;
        private string base64Value = string.Empty;
        Transaction transactionFirst = new Transaction();
        void SelectionChanged(int i)
        {
            searchModel.SelectedSearchType = i;
            searchModel.FilterValue = string.Empty;
        }

        protected override async Task OnInitializedAsync()
        {
            useSpinner = false;
            dialogService.OnOpen += Open;
            dialogService.OnClose += Close;
        }

        public void Dispose()
        {
            // The DialogService is a singleton so it is advisable to unsubscribe.
            dialogService.OnOpen -= Open;
            dialogService.OnClose -= Close;
        }

        void Open(string title, Type type, Dictionary<string, object> parameters, DialogOptions options)
        {

        }

        void Close(dynamic result)
        {

        }

        public async Task OpenSmsDetails(Transaction txn)
        {
            await dialogService.OpenAsync<MessageDetails>("title", new Dictionary<string, object>() { { "Txn", txn } },
                new DialogOptions() { ShowTitle = false, Style = "min-height:auto;min-width:auto;max-height:600px;width:60%", CloseDialogOnEsc = false });
        }

        public EnumBasari CheckSmsStatus(Transaction txn)
        {
            if (txn.TransactionType == TransactionType.Otp)
            {
                if (txn.OtpRequestLog != null)
                {
                    if (txn.OtpRequestLog.ResponseLogs != null)
                    {
                        if (txn.OtpRequestLog.ResponseLogs.Any(l => l.TrackingStatus == SmsTrackingStatus.Delivered))
                        {
                            return EnumBasari.Basarili;
                        }
                        else if (txn.OtpRequestLog.ResponseLogs.Any(l => l.TrackingStatus == SmsTrackingStatus.Pending))
                        {
                            return EnumBasari.SmsKontrolGerekli;
                        }
                        else
                        {
                            return EnumBasari.Basarisiz;
                        }
                    }
                }
            }

            if (txn.TransactionType == TransactionType.TransactionalMail || txn.TransactionType == TransactionType.TransactionalTemplatedMail)
            {
                if (txn.MailRequestLog != null)
                {
                    if (txn.MailRequestLog.ResponseLogs != null)
                    {
                        if (txn.MailRequestLog.ResponseLogs.Any(l => l.ResponseCode == "0"))
                        {
                            return EnumBasari.Basarili;
                        }
                        else
                        {
                            return EnumBasari.Basarisiz;
                        }
                    }
                }
            }

            if (txn.TransactionType == TransactionType.TransactionalSms || txn.TransactionType == TransactionType.TransactionalTemplatedSms)
            {
                if (txn.SmsRequestLog != null)
                {
                    if (txn.SmsRequestLog.ResponseLogs != null)
                    {
                        if (txn.SmsRequestLog.ResponseLogs.Any(l => l.OperatorResponseCode == 0))
                        {
                            return EnumBasari.Basarili;
                        }
                        else
                        {
                            return EnumBasari.Basarisiz;
                        }
                    }
                }
            }

            if (txn.TransactionType == TransactionType.TransactionalPush || txn.TransactionType == TransactionType.TransactionalTemplatedPush)
            {
                if (txn.PushNotificationRequestLog != null)
                {
                    if (txn.PushNotificationRequestLog.ResponseLogs != null)
                    {
                        if (txn.PushNotificationRequestLog.ResponseLogs.Any(l => l.ResponseCode == "0"))
                        {
                            return EnumBasari.Basarili;
                        }
                        else
                        {
                            return EnumBasari.Basarisiz;
                        }
                    }
                }
            }

            return EnumBasari.Basarisiz; ;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                grid.EmptyText = "Hiç Kayıt Bulunamadı.";
            }
        }

        async Task SearchTransactions(LoadDataArgs args = null)
        {
            useSpinner = true;
            if (args == null)
            {
                searchModel.Skip = 0;
                searchModel.Take = pageCount;
            }
            else
            {
                var skip = args.Skip ?? 0;
                var top = args.Top ?? pageCount;
                searchModel.Skip = skip / top;
                searchModel.Take = top;
            }
            if (string.IsNullOrEmpty(searchModel.FilterValue)
                || (searchModel.StartDate == null && searchModel.StartDate.Year < 2000)
                || (searchModel.EndDate == null && searchModel.StartDate.Year < 2000))
            {
                useSpinner = false;

                if (!IsFirstLoad)
                    OpenModal("Lütfen alanları doldurunuz.");
            }
            else
            {
                if (((searchModel.SelectedSearchType != 4 && searchModel.MessageType == MessageTypeEnum.Sms) || searchModel.SelectedSearchType == 3) && searchModel.SmsType == SmsTypeEnum.Unselected)
                {
                    useSpinner = false;

                    if (!IsFirstLoad)
                        OpenModal("Lütfen Sms türünü seçiniz.");
                }
                else
                {
                    if ((searchModel.SelectedSearchType == 1 || searchModel.SelectedSearchType == 2) && searchModel.MessageType == MessageTypeEnum.Unselected)
                    {
                        useSpinner = false;

                        if (!IsFirstLoad)
                            OpenModal("Lütfen Mesaj türünü seçiniz.");
                    }
                    else if (searchModel.StartDate.Date > searchModel.EndDate.Date)
                    {
                        useSpinner = false;

                        if (!IsFirstLoad)
                            OpenModal("Başlangıç Tarihi Bitiş tarihinden büyük olamaz");
                    }
                    else
                    {
                        useSpinner = true;

                        switch (searchModel.SelectedSearchType)
                        {
                            case 1:
                                await SearchWithCustomerNo();
                                break;
                            case 2:
                                await SearchWithCitizenshipNo();
                                break;
                            case 3:
                                await SearchWithPhone();
                                break;
                            case 4:
                                await SearchWithMail();
                                break;
                            default:
                                throw new Exception();
                        }

                        useSpinner = false;
                    }
                }
            }
        }

        async Task SearchWithPhone()
        {
            Phone phone = new Phone(searchModel.FilterValue);

            var phoneV2 = new common.Models.v2.Phone
            {
                Prefix = phone.Prefix,
                Number = phone.Number,
                CountryCode = phone.CountryCode
            };

            var resProfile = await MessagingGateway.GetCustomerProfileByPhoneNumber(phoneV2);

            if (resProfile.IsSuccess == false)
            {
                OpenModal("Müşteri profil servisi hata aldı.");
                return;
            }

            if (resProfile.IsStaff)
            {
                OpenModal("Banka personeline ait mesajların gösterimi engellenmiştir.");
                return;
            }


            var res = await MessagingGateway.GetTransactionsByPhone(new Phone(searchModel.FilterValue), CreateQueryParams());
            transactions = res.Transactions.AsODataEnumerable();
            rowsCount = res.Count;

            if (rowsCount > 0)
            {
                transactionFirst = transactions.FirstOrDefault();
            }
            else
            {
                transactionFirst = new Transaction();
            }
        }

        async Task SearchWithMail()
        {
            var resProfile = await MessagingGateway.GetCustomerProfileByEmail(searchModel.FilterValue);

            if (resProfile.IsSuccess == false)
            {
                OpenModal("Müşteri profil servisi hata aldı.");
                return;
            }

            if (resProfile.IsStaff)
            {
                OpenModal("Banka personeline ait mesajların gösterimi engellenmiştir.");
                return;
            }

            var res = await MessagingGateway.GetTransactionsByMail(searchModel.FilterValue, CreateQueryParams());
            transactions = res.Transactions;
            rowsCount = res.Count;
            if (rowsCount > 0)
            {
                transactionFirst = transactions.FirstOrDefault();
            }
            else
            {
                transactionFirst = new Transaction();
            }
        }

        async Task SearchWithCustomerNo()
        {
            try
            {
                var resProfile = await MessagingGateway.GetCustomerProfileByCustomerNo(Convert.ToUInt64(searchModel.FilterValue));

                if (resProfile.IsSuccess == false)
                {
                    OpenModal("Müşteri profil servisi hata aldı.");
                    return;
                }

                if (resProfile.IsStaff)
                {
                    OpenModal("Banka personeline ait mesajların gösterimi engellenmiştir.");
                    return;
                }

                var res = await MessagingGateway.GetTransactionsByCustomerNo(Convert.ToUInt64(searchModel.FilterValue), Constants.MessageTypeMap[searchModel.MessageType], CreateQueryParams());
                transactions = res.Transactions;
                rowsCount = res.Count;

                if (rowsCount > 0)
                {
                    transactionFirst = transactions.FirstOrDefault();
                }
                else
                {
                    transactionFirst = new Transaction();
                }
            }
            catch (Exception ex)
            {
                transactions = new List<Transaction>();
                transactionFirst = new Transaction();
                rowsCount = 0;
            }
        }

        async Task SearchWithCitizenshipNo()
        {
            var resProfile = await MessagingGateway.GetCustomerProfileByCitizenshipNumber(searchModel.FilterValue);

            if (resProfile.IsSuccess == false)
            {
                OpenModal("Müşteri profil servisi hata aldı.");
                return;
            }

            if (resProfile.IsStaff)
            {
                OpenModal("Banka personeline ait mesajların gösterimi engellenmiştir.");
                return;
            }

            var res = await MessagingGateway.GetTransactionsByCitizenshipNo(searchModel.FilterValue, Constants.MessageTypeMap[searchModel.MessageType], CreateQueryParams());
            transactions = res.Transactions;
            rowsCount = res.Count;

            if (rowsCount > 0)
            {
                transactionFirst = transactions.FirstOrDefault();
            }
            else
            {
                transactionFirst = new Transaction();
            }
        }

        void OnChange(DateTime? value, string name, string format)
        {

        }
        public async void ExcelDownload()
        {
            closeExcel = true;

            StateHasChanged();
            try
            {
                switch (searchModel.SelectedSearchType)
                {
                    case 1:
                        base64Value = await MessagingGateway.GetTransactionsExcelReportWithCustomer(Convert.ToUInt64(searchModel.FilterValue), Constants.MessageTypeMap[searchModel.MessageType], CreateExcelQueryParams());
                        await JS.InvokeAsync<object>("JSInteropExt.saveAsFile", "Rapor.xlsx", "application/vdn.ms-excel", base64Value);
                        break;
                    case 2:
                        base64Value = await MessagingGateway.GetTransactionsExcelReportWithCitizenshipNo(searchModel.FilterValue, Constants.MessageTypeMap[searchModel.MessageType], CreateExcelQueryParams());
                        await JS.InvokeAsync<object>("JSInteropExt.saveAsFile", "Rapor.xlsx", "application/vdn.ms-excel", base64Value);
                        break;
                    case 3:
                        base64Value = await MessagingGateway.GetTransactionsExcelReportWithPhone(new Phone(searchModel.FilterValue), CreateExcelQueryParams());
                        await JS.InvokeAsync<object>("JSInteropExt.saveAsFile", "Rapor.xlsx", "application/vdn.ms-excel", base64Value);
                        break;
                    case 4:
                        base64Value = await MessagingGateway.GetTransactionsExcelReportWithMail(searchModel.FilterValue, CreateExcelQueryParams());
                        await JS.InvokeAsync<object>("JSInteropExt.saveAsFile", "Rapor.xlsx", "application/vdn.ms-excel", base64Value);
                        break;
                    default:
                        throw new Exception();
                }
            }
            catch (Exception ex)
            {

            }

            closeExcel = false;
            StateHasChanged();
        }
        void SelectMessageType(object value, string name)
        {
            if (value == null)
            {
                searchModel.MessageType = MessageTypeEnum.Unselected;
            }
            else
            {
                searchModel.MessageType = Enum.Parse<MessageTypeEnum>(value.ToString());

            }

            searchModel.SmsType = SmsTypeEnum.Unselected;
        }

        void SelectSmsType(object value, string name)
        {
            if (value == null)
            {
                searchModel.SmsType = SmsTypeEnum.Unselected;
            }
            else
            {
                searchModel.SmsType = Enum.Parse<SmsTypeEnum>(value.ToString());
            }
        }

        void MessageTableSort(DataGridColumnSortEventArgs<Transaction> args)
        {
        }

        QueryParams CreateQueryParams()
        {
            return new QueryParams()
            {
                StartDate = searchModel.StartDate.Date,
                EndDate = searchModel.EndDate.Date.AddDays(1),
                page = searchModel.Skip,
                pageSize = searchModel.Take,
                smsType = Constants.SmsTypeMap[searchModel.SmsType],
                createdName = searchModel.CreatedBy == null ? "" : searchModel.CreatedBy,
            };
        }
        QueryParams CreateExcelQueryParams()
        {
            return new QueryParams()
            {
                StartDate = searchModel.StartDate.Date,
                EndDate = searchModel.EndDate.Date.AddDays(1),
                page = 0,
                pageSize = rowsCount + 1,
                smsType = Constants.SmsTypeMap[searchModel.SmsType],
                createdName = searchModel.CreatedBy == null ? "" : searchModel.CreatedBy,
            };
        }
    }
}