using bbt.gateway.common.Models.v2;

namespace bbt.gateway.messaging.ui.Data
{
    public class SmsRaporTempResponse
    {
        public SmsRaporTempOtp Otp { get; set; }
        public SmsRaporTempFast Fast { get; set; }
    }
    public class SmsRaporTempOtp
    {
        public int Turkcell { get; set; }
        public int Vodafone { get; set; }
        public int TurkTelekom { get; set; }
        public int Yurtdisi { get; set; }
    }
    public class SmsRaporTempFast
    {
        public int dEngage { get; set; }
        public int Codec { get; set; }
    }
    public class SmsRaporRequest
    {
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now;
    }
    public class PieChartItem
    {
        public string Title { get; set; }
        public int Value { get; set; }
        public double Percent { get; set; }
        public SmsTypes Type { get; set; }
    }
}
