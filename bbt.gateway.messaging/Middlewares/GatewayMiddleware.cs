using bbt.gateway.common.Models;
using bbt.gateway.common.Repositories;
using bbt.gateway.messaging.Exceptions;
using bbt.gateway.messaging.Workers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace bbt.gateway.messaging.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class GatewayMiddleware
    {
        private readonly RequestDelegate _next;
        
        public GatewayMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context,IConfiguration _configuration)
        {
            if (context.Request.Headers["X-Secret"] != _configuration["Apisix:Secret"])
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Service Can Not Called Directly,Use Gateway Url Instead");
            }
            else
            {
                await _next(context);
            }
        }

       
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class GatewayMiddlewareExtensions
    {
        public static IApplicationBuilder UseGatewayMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseWhen(context => (context.Request.Path.Value.IndexOf("/Administration/operators") != -1
            ), builder =>
            {
                builder.UseMiddleware<GatewayMiddleware>();
            });
        }
    }
}
