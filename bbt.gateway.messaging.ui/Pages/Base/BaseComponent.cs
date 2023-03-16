using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

namespace bbt.gateway.messaging.ui.Pages.Base
{
    public class BaseComponent : ComponentBase
    {
        [Inject]
        public DialogService dialogService { get; set; }
        [Inject]
        public IJSRuntime JS { get; set; }
        public bool IsFirstLoad { get; set; } = false;
        public void OpenModal(string Message)
        {
            dialogService.Open<BaseMessageDialog>("Bilgilendirme",
              new Dictionary<string, object>() { { "Message", Message } },
              new DialogOptions() { CloseDialogOnOverlayClick = true });
        }
        public async void OpenModalAsync(string Message)
        {
            await dialogService.OpenAsync<BaseMessageDialog>("Bilgilendirme",
                 new Dictionary<string, object>() { { "Message", Message } },
                 new DialogOptions() { CloseDialogOnOverlayClick = true });
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                IsFirstLoad = true;
            }
            else
            {
                IsFirstLoad = false;
            }
            base.OnAfterRender(firstRender);
        }
    }
}
