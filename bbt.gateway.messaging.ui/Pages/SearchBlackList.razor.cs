
using bbt.gateway.common.Models;
using bbt.gateway.messaging.ui.Data;
using bbt.gateway.messaging.ui.Pages.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using System.DirectoryServices;

namespace bbt.gateway.messaging.ui.Pages
{

    public partial class SearchBlackList:BaseComponent
    {
 
        [Inject]
        public bbt.gateway.messaging.ui.Base.Administration.AdministrationService administrationService { get; set; }
        private IEnumerable<BlackListEntry>? blackListEntries;
        private SearchModel searchModel = new SearchModel();
        private int pageCount = 10;
        private int rowsCount = 0;
        private bool useSpinner;
        private RadzenDataGrid<BlackListEntry> grid;
        void SelectionChanged(int i)
        {
         
            searchModel.SelectedSearchType = i;
            searchModel.FilterValue = string.Empty;
        }

        protected override async Task OnInitializedAsync()
        {
          
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                grid.EmptyText = "Hiç Kayıt Bulunamadı.";
            }
        }
        void OnChange(DateTime? value, string name, string format)
        {

        }
        void Save()
        {

        }
        async Task SearchBlackListEntries(LoadDataArgs args = null)
        {
           // useSpinner = true;
            if (args == null)
            {
                searchModel.Skip = 0;
                searchModel.Take = pageCount;
            }
            else
            {
                var skip = args.Skip ?? 0;
                var top = args.Top ?? pageCount;
                searchModel.Skip = skip / top;
                searchModel.Take = top;
            }
            switch (searchModel.SelectedSearchType)
            {
                case 1:
                    await SearchWithPhone();
                    break;

                case 2:
                    await SearchWithCustomerNo();
                    break;
                default:
                    throw new Exception();
            }
            useSpinner = false;
        }
        async Task SearchWithPhone()
        {
            if(!string.IsNullOrEmpty(searchModel.FilterValue))
            {
                Phone phone = new Phone(searchModel.FilterValue);
                try
                {
                    var res2 = administrationService.GetBlackListEntriesByPhone(phone.CountryCode, phone.Prefix, phone.Number, CreateQueryParams());
                    //blackListEntries = await MessagingGateway.GetBlackListByPhone(phone.CountryCode, phone.Prefix, phone.Number, CreateQueryParams());
                    //rowsCount = blackListEntries.Count();
                    //var res = await MessagingGateway.GetBlackListEntriesByPhone(phone.CountryCode, phone.Prefix, phone.Number, CreateQueryParams());
                    blackListEntries = res2.Result.BlackListEntries;
                    rowsCount = res2.Result.Count;
                }
                catch(Exception ex)
                {

                }
               
            }
            else
            {
                if (!IsFirstLoad)
                    OpenModal("Lütfen telefon bilgisini doldurunuz.");
            }
            
        }
        async Task SearchWithCustomerNo()
        {
            if (!string.IsNullOrEmpty(searchModel.FilterValue))
            {
                try
                {
                    //blackListEntries = await MessagingGateway.GetBlackListByPhone(phone.CountryCode, phone.Prefix, phone.Number, CreateQueryParams());
                    //rowsCount = blackListEntries.Count();
                    var res = await MessagingGateway.GetBlackListEntriesByCustomerNo(Convert.ToUInt64(searchModel.FilterValue), CreateQueryParams());
                    blackListEntries = res.BlackListEntries;
                    rowsCount = res.Count;
                }
                catch (Exception ex)
                {

                }

            }
            else
            {
                if (!IsFirstLoad)
                    OpenModal("Lütfen müşteri no bilgisini doldurunuz.");
            }

        }
        QueryParams CreateQueryParams()
        {
            return new QueryParams()
            {

                page = searchModel.Skip,
                pageSize = searchModel.Take,
            };
        }
       
    }
}
