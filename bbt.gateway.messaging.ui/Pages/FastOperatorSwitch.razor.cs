using bbt.gateway.messaging.ui.Models;
using bbt.gateway.messaging.ui.Pages.Base;
using Microsoft.AspNetCore.Components;
using Radzen;
using System.ComponentModel.Design;

namespace bbt.gateway.messaging.ui.Pages
{
    public partial class FastOperatorSwitch : BaseComponent
    {
        public FastOperatorModel fastOperatorModel { get; set; } = new FastOperatorModel();

        protected override async Task OnInitializedAsync()
        {
            var fastOperator = await MessagingGatewayService.GetFastOperator();

            if (fastOperator == null)
            {
                dialogService.Open<BaseMessageDialog>("BilgileHatandirme",
                   new Dictionary<string, object>() { { "Message", "Fast Operatör Bilgisi Getirilemedi." } },
                   new DialogOptions() { CloseDialogOnOverlayClick = true });

                return;
            }

            fastOperatorModel.FastOperatorValue = fastOperator.Value;
        }

        async Task Save(LoadDataArgs args = null)
        {
            await MessagingGatewayService.ChangeFastOperator(fastOperatorModel.FastOperatorValue);

            dialogService.Open<BaseMessageDialog>("Bilgilendirme",
             new Dictionary<string, object>() { { "Message", "Bilgiler Başarıyla Kaydedildi" } },
             new DialogOptions() { CloseDialogOnOverlayClick = true });
        }
    }
}
