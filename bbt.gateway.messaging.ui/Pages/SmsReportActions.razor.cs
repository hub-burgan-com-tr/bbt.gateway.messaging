
using bbt.gateway.messaging.ui.Data;
using bbt.gateway.messaging.ui.Pages.Base;
using Radzen;
using Radzen.Blazor;
using bbt.gateway.common.Models.v2;
using bbt.gateway.common.GlobalConstants;
using System.Collections.Concurrent;

namespace bbt.gateway.messaging.ui.Pages
{
    public partial class SmsReportActions : BaseComponent
    {
        private SmsRaporRequest searchModel = new SmsRaporRequest();
        //private SmsReportResponse response = new SmsReportResponse();
        private List<PieChartItem> pieChartItemFast = new List<PieChartItem>();
        private List<PieChartItem> pieChartItemOtp = new List<PieChartItem>();
        private List<PieChartItem> pieChartItemAllForGrid = new List<PieChartItem>();
        private List<OperatorReport> ReportGrid = new List<OperatorReport>();
        private Boolean searchCompleted = false;
        private RadzenDataGrid<OperatorReport> grid;
        private int OtpCount { get; set; } = 0;
        private int FastCount { get; set; } = 0;
        async void SearchSmsReports(LoadDataArgs args = null)
        {
            ReportGrid = new List<OperatorReport>();
            if (searchModel.StartDate.Date > searchModel.EndDate.Date)
            {
                if (!IsFirstLoad)
                    OpenModal("Başlangıç Tarihi Bitiş tarihinden büyük  olamaz");
               
            }
            else
            {
                ConcurrentBag<OperatorReport> operatorReports = new ConcurrentBag<OperatorReport>();
                var taskList = new List<Task>();

                foreach (var @operator in GlobalConstants.reportOperators.Keys)
                {
                    taskList.Add(OperatorReportProcess(operatorReports,@operator));
                }

                await Task.WhenAll(taskList);

                ReportGrid = operatorReports.ToList();

                searchCompleted = true;
                StateHasChanged();
            }
           

        }

        public async Task OperatorReportProcess(ConcurrentBag<OperatorReport> operatorReports, int @operator)
        {
            operatorReports.Add(await MessagingGatewayService.SmsReportAsync(@operator, CreateQueryParams()));
        }

        public int Total(common.Models.v2.OperatorReport data)
        {
            return (data.FastCount + data.OtpCount + data.ForeignOtpCount + data.ForeignFastCount);
        }
        public int TotalSuccessFull(common.Models.v2.OperatorReport data)
        {
            return (data.SuccessfullFastRequestCount + data.SuccessfullOtpRequestCount + data.SuccessfullForeignFastRequestCount + data.SuccessfullForeignOtpRequestCount);
        }
        public double Percent(int all, int value)
        {
            if (all == 0)
                return 0;
                return (value * 100 / all);
        }
        QueryParams CreateQueryParams()
        {
            return new QueryParams()
            {
                StartDate = searchModel.StartDate.Date,
                EndDate = searchModel.EndDate.Date.AddDays(1),

            };
        }


    }
}
