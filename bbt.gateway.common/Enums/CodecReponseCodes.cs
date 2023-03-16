using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbt.gateway.common.Models
{
    public enum CodecResponseCodes
    {
        Success = 200,
        UndefinedSender = 490,
        NotAuthorized = 401,
        UnknownError = 400
    }
}
