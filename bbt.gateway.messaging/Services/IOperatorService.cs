using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.SecurityToken.Model;
using bbt.gateway.common.Models;

namespace bbt.gateway.messaging.Services
{
    public interface IOperatorService
    {
        public Task<Operator> GetOperator(OperatorType type);
        public Task RevokeCache();
    }
}