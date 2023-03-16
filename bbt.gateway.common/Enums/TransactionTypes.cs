using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbt.gateway.common.Models
{
    public enum TransactionType
    {
        Otp,
        TransactionalSms,
        TransactionalTemplatedSms,
        TransactionalMail,
        TransactionalTemplatedMail,
        TransactionalPush,
        TransactionalTemplatedPush,

    }
}
