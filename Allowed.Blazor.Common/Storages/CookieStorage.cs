using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Allowed.Blazor.Common.Storages
{
    public class CookieStorage : IAsyncDisposable
    {
        private readonly IJSRuntime _jsRuntime;

        private Task<IJSObjectReference> _module;
        private Task<IJSObjectReference> Module => _module ??=
            _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/Allowed.Blazor.Common/cookie-storage.js").AsTask();

        public CookieStorage(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task SetCookie(string name, string value, int maxAge = 86400, string domain = "", string path = "/")
        {
            await (await Module).InvokeVoidAsync("setCookie", name, value, maxAge, domain, path);
        }

        public async Task SetSessionCookie(string name, string value, string domain = "", string path = "/")
        {
            await (await Module).InvokeVoidAsync("setSessionCookie", name, value, domain, path);
        }

        public async Task RemoveCookie(string name, string domain = "", string path = "/")
        {
            await (await Module).InvokeVoidAsync("removeCookie", name, domain, path);
        }

        public async Task<string> GetCookie(string name)
        {
            return await (await Module).InvokeAsync<string>("getCookie", name);
        }

        public async ValueTask DisposeAsync()
        {
            if (_module != null)
                await (await _module).DisposeAsync();
        }
    }
}
