namespace bbt.gateway.common.Models.v2
{
    public enum dEngageResponseCodes
    {
        Success = 200,
        BadRequest = 400,
        Unauthorized = 401,
        NotAllowed = 403,
        NotFound = 404,
        TooManyRequest = 429,
        NotVerified = 435
    }
}
