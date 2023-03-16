using bbt.gateway.common.Models;

namespace bbt.gateway.messaging.Api
{
    public interface IBaseApi
    {
        public void SetOperatorType(Operator op);
        public OperatorType Type { get; set; }
    }
}
