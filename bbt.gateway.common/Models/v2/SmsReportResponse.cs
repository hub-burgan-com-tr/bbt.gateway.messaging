namespace bbt.gateway.common.Models.v2
{


    public partial class OperatorReport
    {
        public int OtpCount { get; set; } = 0;
        public int ForeignOtpCount { get; set; } = 0;
        public int FastCount { get; set; } = 0;
        public int ForeignFastCount { get; set; } = 0;
        public int SuccessfullOtpRequestCount { get; set; } = 0;
        public int UnsuccessfullOtpRequestCount { get; set; } = 0;
        public int SuccessfullForeignOtpRequestCount { get; set; } = 0;
        public int UnsuccessfullForeignOtpRequestCount { get; set; } = 0;
        public int SuccessfullForeignFastRequestCount { get; set; } = 0;
        public int UnsuccessfullForeignFastRequestCount { get; set; } = 0;
        public int SuccessfullFastRequestCount { get; set; } = 0;
        public int UnsuccessfullFastRequestCount { get; set; } = 0;

        public static OperatorReport operator +(OperatorReport a, OperatorReport b){
            OperatorReport r = new OperatorReport();
            r.OtpCount = a.OtpCount + b.OtpCount;
            r.FastCount = a.FastCount + b.FastCount;
            r.ForeignFastCount = a.ForeignFastCount + b.ForeignFastCount;
            r.SuccessfullOtpRequestCount = a.SuccessfullOtpRequestCount + b.SuccessfullOtpRequestCount;
            r.UnsuccessfullOtpRequestCount = a.UnsuccessfullOtpRequestCount + b.UnsuccessfullOtpRequestCount;
            r.SuccessfullForeignOtpRequestCount = a.SuccessfullForeignOtpRequestCount + b.SuccessfullForeignOtpRequestCount;
            r.UnsuccessfullForeignOtpRequestCount = a.UnsuccessfullForeignOtpRequestCount + b.UnsuccessfullForeignOtpRequestCount;
            r.SuccessfullFastRequestCount = a.SuccessfullFastRequestCount + b.SuccessfullFastRequestCount;
            r.UnsuccessfullFastRequestCount = a.UnsuccessfullFastRequestCount + b.UnsuccessfullFastRequestCount;
            r.SuccessfullForeignFastRequestCount = a.SuccessfullForeignFastRequestCount + b.SuccessfullForeignFastRequestCount;
            r.UnsuccessfullForeignFastRequestCount = a.UnsuccessfullForeignFastRequestCount + b.UnsuccessfullForeignFastRequestCount;
            r.Operator=a.Operator|b.Operator;
            return r;
        }
        public OperatorType Operator { get; set; }

    }

    
}
