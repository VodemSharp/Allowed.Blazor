using Microsoft.JSInterop;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Allowed.Blazor.Common.Storages
{
    public class LocalStorage : IAsyncDisposable
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly CancellationTokenSource _cts = new();
        private readonly StorageQueue _queue;

        private Task<IJSObjectReference> _module;
        private Task<IJSObjectReference> Module => _module ??=
            _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/Allowed.Blazor.Common/local-storage.js").AsTask();

        public LocalStorage(IJSRuntime jsRuntime, StorageQueue queue)
        {
            _jsRuntime = jsRuntime;
            _queue = queue;
        }

        public async Task InvokeSet(Func<Task> func)
        {
            if (_queue.Ready)
                await func.Invoke();
            else
                _queue.Tasks.Enqueue(func);
        }

        public async Task SetLocal(string name, string value)
        {
            await InvokeSet(async () => await (await Module).InvokeVoidAsync("setLocal", _cts.Token, name, value));
        }

        public async Task<string> GetLocal(string name)
        {
            return await (await Module).InvokeAsync<string>("getLocal", _cts.Token, name);
        }

        public async ValueTask DisposeAsync()
        {
            if (_module != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                await (await _module).DisposeAsync();
            }
        }
    }
}
