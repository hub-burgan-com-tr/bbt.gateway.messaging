using Newtonsoft.Json.Linq;

namespace bbt.gateway.common.Helpers
{
    public static class JsonHelper
    {
        public static string GetJsonStringValue(JObject obj, string path)
        {
            var retVal = "";

            var token = obj.SelectToken(path);

            if (token != null && token.Type != JTokenType.Null)
            {
                retVal = token.ToString();
            }

            return retVal;
        }
    }
}