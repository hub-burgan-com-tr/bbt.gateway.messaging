using Microsoft.AspNetCore.Components;

namespace bbt.gateway.messaging.ui.Pages.Base
{
    public partial class BaseMessageDialog
    {
        [Parameter]
        public string Message { get; set; }
    }
}
