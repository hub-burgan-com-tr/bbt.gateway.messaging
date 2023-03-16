using System;
using System.Net;

namespace bbt.gateway.messaging.Exceptions
{
    public class WorkflowException:Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        public WorkflowException(string message,HttpStatusCode statusCode) : base(message) {
            StatusCode = statusCode;
        }
    }
}
