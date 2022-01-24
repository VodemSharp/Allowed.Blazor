using Microsoft.JSInterop;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Allowed.Blazor.Common.Console
{
    public class ConsoleWriter : IAsyncDisposable
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly CancellationTokenSource _cts = new();

        private Task<IJSObjectReference> _module;
        private Task<IJSObjectReference> Module => _module ??=
            _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/Allowed.Blazor.Common/console-writer.js").AsTask();

        public ConsoleWriter(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task Info(string value)
        {
            await (await Module).InvokeVoidAsync("info", _cts.Token, value);
        }

        public async Task Warning(string value)
        {
            await (await Module).InvokeVoidAsync("warning", _cts.Token, value);
        }

        public async Task Error(string value)
        {
            await (await Module).InvokeVoidAsync("error", _cts.Token, value);
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
