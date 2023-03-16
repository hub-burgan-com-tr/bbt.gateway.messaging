namespace bbt.gateway.messaging.ui.Base.Token
{
    public interface ITokenService
    {
        Task<string> GetToken();
        OktaSettings GetOktaSettings();
    }
}
