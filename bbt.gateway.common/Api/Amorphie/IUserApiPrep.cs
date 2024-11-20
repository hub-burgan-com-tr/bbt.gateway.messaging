using bbt.gateway.common.Api.Amorphie.Model;
using bbt.gateway.common.Models;
using Microsoft.AspNetCore.Mvc;
using Refit;
namespace bbt.gateway.common.Api.Amorphie
{
    public interface IUserApiPrep
    {
        [Get("/userDevice/deviceToken/{reference}")]
        Task<RevampDevice> GetDeviceTokenAsync(string reference);

    }
}