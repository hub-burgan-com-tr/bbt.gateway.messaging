using bbt.gateway.common.Models;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Workers.OperatorGateway
{
    public interface IOperatorGatewayBase
    {
        public IConfiguration Configuration { get; }
        public Task<PhoneConfiguration> GetPhoneConfiguration(Phone phone);
        public Task SaveOperator();
        public Task GetOperatorAsync(OperatorType type);
        public Operator OperatorConfig { get; set; }
        public OperatorType Type { get; set; }
        public ITransactionManager TransactionManager { get; }
    }
}
