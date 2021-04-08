using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Allowed.Blazor.Common.Storages
{
    public class LocalStorage : IAsyncDisposable
    {
        private readonly IJSRuntime _jsRuntime;

        private Task<IJSObjectReference> _module;
        private Task<IJSObjectReference> Module => _module ??=
            _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/Allowed.Blazor.Common/local-storage.js").AsTask();

        public LocalStorage(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task SetLocal(string name, string value)
        {
            await (await Module).InvokeVoidAsync("setLocal", name, value);
        }

        public async Task<string> GetLocal(string name)
        {
            return await (await Module).InvokeAsync<string>("getLocal", name);
        }

        public async ValueTask DisposeAsync()
        {
            if (_module != null)
                await (await _module).DisposeAsync();
        }
    }
}
