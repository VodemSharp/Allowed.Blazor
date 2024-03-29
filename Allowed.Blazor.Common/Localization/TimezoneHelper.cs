﻿using Microsoft.JSInterop;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Allowed.Blazor.Common.Localization
{
    public class TimezoneHelper : IAsyncDisposable
    {
        private readonly IJSRuntime _jsRuntime;

        private Task<IJSObjectReference> _module;

        private Task<IJSObjectReference> Module => _module ??=
            _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/Allowed.Blazor.Common/timezone.js",
                Timeout.InfiniteTimeSpan).AsTask();

        public TimezoneHelper(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<double> GetTimezoneOffset() => await (await Module).InvokeAsync<double>("getTimezoneOffset",
            Timeout.InfiniteTimeSpan);

        public async Task<DateTime> ToLocalTime(DateTime dateTime)
        {
            return dateTime.AddMinutes(-1 * await GetTimezoneOffset());
        }

        public async ValueTask DisposeAsync()
        {
            if (_module != null)
                await (await _module).DisposeAsync();
        }
    }
}