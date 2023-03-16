namespace bbt.gateway.common.Models.v2
{
    public class AddPhoneToWhitelistRequest
    {
        public PhoneString Phone { get; set; }
        public Process CreatedBy { get; set; }
    }
}
