using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace bbt.gateway.messaging
{
    public class AddRequiredHeaderParameter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "Process",
                In = ParameterLocation.Header,
                Description = "Which process is consuming service",
                Required = true,
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Default = new OpenApiString("new-customer-on-boarding")
                }
            });

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "Action",
                In = ParameterLocation.Header,
                Description = "Action/Method/Service name  of process",
                Required = true,
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Default = new OpenApiString("retry-otp")
                }
            });

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "ItemId",
                In = ParameterLocation.Header,
                Description = "Consumer process called this service for ...",
                Required = true,
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Default = new OpenApiString("application:786868")
                }
            });

             operation.Parameters.Add(new OpenApiParameter
            {
                Name = "Identity",
                In = ParameterLocation.Header,
                Description = "Which user is called service. Processes has to impersonete user info",
                Required = true,
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Default = new OpenApiString("ebt\\u04545")
                }
            });
        }
    }
}