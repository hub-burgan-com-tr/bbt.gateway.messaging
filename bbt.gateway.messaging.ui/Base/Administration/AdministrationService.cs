using bbt.gateway.common.Models;
using bbt.gateway.messaging.ui.Data;
using Microsoft.AspNetCore.Components;

namespace bbt.gateway.messaging.ui.Base.Administration
{


    public class AdministrationService : BaseRefit<IAdministrationService>
    {
        public AdministrationService(IConfiguration config, IHttpContextAccessor httpContextAccessor,NavigationManager navigationManager) : base(config.GetValue<string>("Api:MessagingGateway"), httpContextAccessor, navigationManager)
        {
        }

        public override string controllerName => "/api/v1/Administration/";

        public async Task<BlackListEntriesDto> GetBlackListEntriesByPhone(int countryCode, int prefix, int number, QueryParams queryParams)
        {
            try
            {
                
                return await ExecutePolly(() =>
                {
                    return api.GetBlackListEntriesByPhone(countryCode, prefix, number, queryParams.page,queryParams.pageSize).Result;
                }
                    );
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("401 (Unauthorized)"))
                {
                    return new BlackListEntriesDto() ;
                }

                throw;
            }
        }
    }
}
