using bbt.gateway.common.Extensions;
using bbt.gateway.common.Models;
using bbt.gateway.common.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Workers
{
    public class OperatorManager
    {
        List<Operator> operators = new List<Operator>();
        private readonly IRepositoryManager _repositoryManager;
        public OperatorManager(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public async Task<OperatorInfo[]> Get()
        {            
            var operatorList = await _repositoryManager.Operators.GetAllAsync();
            var operatorInfoList = new List<OperatorInfo>();

            foreach (var item in operatorList)
            {
                operatorInfoList.Add(item.MapTo<OperatorInfo>());
            }

            return operatorInfoList.ToArray();
        }

        public async Task<Operator> Get(OperatorType type)
        {
            return await _repositoryManager.Operators.FirstOrDefaultAsync(o => o.Type == type);
        }


        public async Task Save(Operator data)
        {
            
            if (await _repositoryManager.Operators.FirstOrDefaultAsync(o => o.Id == data.Id) != null)
            {
                throw new NotSupportedException("Adding new operator is not allowed.");
            }
            else
            {
                _repositoryManager.Operators.Update(data);
            }
            await _repositoryManager.SaveChangesAsync();

        }

        private async Task loadOperators()
        {
            operators = (await _repositoryManager.Operators.GetAllAsync()).ToList();
        }
    }
}
