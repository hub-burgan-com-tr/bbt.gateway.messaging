using bbt.gateway.common.Models;
using bbt.gateway.messaging.Workers;

namespace bbt.gateway.messaging.Api
{
    public class BaseApi
    {
        private OperatorType _type;
        protected readonly ITransactionManager TransactionManager;
        public BaseApi(ITransactionManager transactionManager)
        {
            TransactionManager = transactionManager;
        }

        public OperatorType Type
        {
            get { return _type; }
            set
            {
                _type = value;
            }
        }

        public void SetOperatorType(Operator op) => OperatorConfig = op;

        protected Operator OperatorConfig { get; set; }
    }
}
