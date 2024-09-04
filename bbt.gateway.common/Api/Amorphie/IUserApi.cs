using bbt.gateway.common.Models;
using Microsoft.AspNetCore.Mvc;
using Refit;

namespace bbt.gateway.common.Api.Amorphie
{
    public interface IUserApi
    {
        [Get("/userDevice/deviceToken/{reference}")]
        Task<string> GetDeviceTokenAsync(string reference);
       
    }
}
