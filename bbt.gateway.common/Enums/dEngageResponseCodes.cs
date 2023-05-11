using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbt.gateway.common.Models
{
    public  enum dEngageResponseCodes
    {
        Success = 200,
        BadRequest = 400,
        Unauthorized = 401,
        NotAllowed = 403,
        NotFound = 404,
        TooManyRequest=429,
        NotVerified = 435
    }
}
