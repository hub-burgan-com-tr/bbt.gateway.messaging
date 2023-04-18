
using bbt.gateway.messaging.ui.Data;
using bbt.gateway.messaging.ui.Pages.Base;
using Radzen;
using Radzen.Blazor;
using bbt.gateway.common.Models.v2;

namespace bbt.gateway.messaging.ui.Pages
{
    public partial class SmsReports : BaseComponent
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
        void SearchSmsReports(LoadDataArgs args = null)
        {
            if (searchModel.StartDate.Date > searchModel.EndDate.Date)
            {
                if (!IsFirstLoad)
                    OpenModal("Başlangıç Tarihi Bitiş tarihinden büyük  olamaz");
            }
            var res = MessagingGateway.SmsReportAsync(CreateQueryParams());
            ReportGrid = res.Result;

             searchCompleted = true;
            StateHasChanged();

        }
    
        public int Total(common.Models.v2.OperatorReport data)
        {
            return (data.FastCount + data.OtpCount+data.ForeignOtpCount+data.ForeignFastCount);
        }
        public string Percent(PieChartItem data)
        {
            if (data.Type == SmsTypes.Otp)
            {
                return (data.Value * 100 / OtpCount).ToString();
            }
            else if (data.Type == SmsTypes.Fast)
            {
                return (data.Value * 100 / FastCount).ToString();
            }
            return "";
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
