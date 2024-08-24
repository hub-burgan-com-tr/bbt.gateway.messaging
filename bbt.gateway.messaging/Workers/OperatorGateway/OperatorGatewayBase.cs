using bbt.gateway.common.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using bbt.gateway.common;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace bbt.gateway.messaging.Workers.OperatorGateway
{
    public abstract class OperatorGatewayBase : IOperatorGatewayBase
    {
        private OperatorType type;
        private readonly IConfiguration _configuration;
        private DbContextOptions<DatabaseContext> _dbOptions;
        private readonly ITransactionManager _transactionManager;
        protected OperatorGatewayBase(IConfiguration configuration,ITransactionManager transactionManager) 
        {
            _transactionManager = transactionManager;
            _configuration = configuration;
            _dbOptions = new DbContextOptionsBuilder<DatabaseContext>()
                .UseSqlServer(_configuration.GetConnectionString("DefaultConnection"))
                .Options;
        }

        public OperatorType Type
        {
            get { return type; }
            set
            {
                type = value;
                OperatorConfig = _transactionManager.ActiveOperator;
            }
        }
        public Operator OperatorConfig { get; set; }

        public IConfiguration Configuration => _configuration;

        public ITransactionManager TransactionManager => _transactionManager;

        public async Task SaveOperator()
        {
            using var databaseContext = new DatabaseContext(_dbOptions);
            databaseContext.Operators.Update(OperatorConfig);
            await databaseContext.SaveChangesAsync();

            await _transactionManager.RevokeOperatorsAsync();
        }

        public async Task<PhoneConfiguration> GetPhoneConfiguration(Phone phone)
        {
            using var databaseContext = new DatabaseContext(_dbOptions);
            return await databaseContext.PhoneConfigurations.AsNoTracking().Where(i =>
                i.Phone.CountryCode == phone.CountryCode &&
                i.Phone.Prefix == phone.Prefix &&
                i.Phone.Number == phone.Number
                ).Include(p => p.BlacklistEntries)
                .FirstOrDefaultAsync();
        }

        public async Task GetOperatorAsync(OperatorType type)
        {
            this.type = type;
            OperatorConfig = await _transactionManager.GetOperatorAsync(type);
        }
    }



}
