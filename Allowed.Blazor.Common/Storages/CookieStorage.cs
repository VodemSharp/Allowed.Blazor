﻿using Allowed.Blazor.Common.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Allowed.Blazor.Common.Storages
{
    public class CookieStorage : IAsyncDisposable
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly StorageQueue _queue;
        private readonly CookieLocker _cookieLocker;
        private readonly Dictionary<string, string> _tempCookies = new();

        private Task<IJSObjectReference> _module;

        private Task<IJSObjectReference> Module => _module ??=
            _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/Allowed.Blazor.Common/cookie-storage.js",
                Timeout.InfiniteTimeSpan).AsTask();

        public CookieStorage(IHttpContextAccessor accessor, IJSRuntime jsRuntime, StorageQueue queue,
            CookieLocker cookieLocker)
        {
            _jsRuntime = jsRuntime;
            _queue = queue;
            _cookieLocker = cookieLocker;

            foreach (KeyValuePair<string, string> cookie in accessor.HttpContext.Request.Cookies)
                _tempCookies.TryAdd(cookie.Key, cookie.Value);
        }

        public async Task InvokeSet(Func<Task> func, string name, string value)
        {
            await _cookieLocker.LockAsync(async () =>
            {
                if (_queue.Ready)
                {
                    await func.Invoke();
                }
                else
                {
                    if (value != null)
                    {
                        if (_tempCookies.ContainsKey(name))
                            _tempCookies[name] = value;
                        else
                            _tempCookies.Add(name, value);
                    }

                    if (value == null && _tempCookies.ContainsKey(name))
                        _tempCookies.Remove(name);

                    _queue.Tasks.Enqueue(func);
                }
            });
        }

        public async Task SetCookie(string name, string value, int maxAge = 86400, string domain = "",
            string path = "/")
        {
            await InvokeSet(
                async () => await (await Module).InvokeVoidAsync("setCookie", Timeout.InfiniteTimeSpan, name, value,
                    maxAge, domain, path), name, value);
        }

        public async Task SetSessionCookie(string name, string value, string domain = "", string path = "/")
        {
            await InvokeSet(
                async () => await (await Module).InvokeVoidAsync("setSessionCookie", Timeout.InfiniteTimeSpan, name,
                    value, domain, path), name, value);
        }

        public async Task RemoveCookie(string name, string domain = "", string path = "/")
        {
            await InvokeSet(
                async () => await (await Module).InvokeVoidAsync("removeCookie", Timeout.InfiniteTimeSpan, name, domain,
                    path), name, null);
        }

        public async Task<string> GetCookie(string name)
        {
            string result = null;
            await _cookieLocker.LockAsync(async () =>
            {
                if (_queue.Ready)
                    result = await (await Module).InvokeAsync<string>("getCookie", Timeout.InfiniteTimeSpan, name);
                else
                    result = _tempCookies.ContainsKey(name) ? _tempCookies[name] : null;
            });

            return result;
        }

        public async ValueTask DisposeAsync()
        {
            if (_module != null)
                await (await _module).DisposeAsync();
        }
    }
}