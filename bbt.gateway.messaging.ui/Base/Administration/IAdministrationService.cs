using bbt.gateway.common.Models;
using bbt.gateway.messaging.ui.Data;
using RestEase;

namespace bbt.gateway.messaging.ui.Base.Administration
{
    public interface IAdministrationService
    {
        [Header("Authorization", "Bearer")]
        [Get("/api/v1/Administration/blacklists/phone/{countryCode}/{prefix}/{number}")]
        Task<BlackListEntriesDto> GetBlackListEntriesByPhone([Path] int countryCode, [Path] int prefix, [Path] int number, int page, int pagesize);
    }
}
