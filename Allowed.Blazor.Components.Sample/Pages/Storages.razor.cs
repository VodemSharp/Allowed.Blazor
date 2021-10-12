using Allowed.Blazor.Common.Storages;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Allowed.Blazor.Components.Sample.Pages
{
    [Route("storages")]
    public partial class Storages
    {
        [Inject] private CookieStorage CookieStorage { get; set; }
        private string TestCookie { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await CookieStorage.SetCookie("test", Guid.NewGuid().ToString());
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if(firstRender)
            {
                TestCookie = await CookieStorage.GetCookie("test");
                StateHasChanged();
            }
        }
    }
}
