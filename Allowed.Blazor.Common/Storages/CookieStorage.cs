using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Allowed.Blazor.Common.Storages
{
    public class CookieStorage : IAsyncDisposable
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly StorageQueue _queue;
        private readonly Dictionary<string, string> _tempCookies = new();

        private Task<IJSObjectReference> _module;
        private Task<IJSObjectReference> Module => _module ??=
            _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/Allowed.Blazor.Common/cookie-storage.js").AsTask();

        public CookieStorage(IHttpContextAccessor accessor, IJSRuntime jsRuntime, StorageQueue queue)
        {
            _jsRuntime = jsRuntime;
            _queue = queue;

            foreach (KeyValuePair<string, string> cookie in accessor.HttpContext.Request.Cookies)
                _tempCookies.TryAdd(cookie.Key, cookie.Value);
        }

        public async Task InvokeSet(Func<Task> func, string name, string value)
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
        }

        public async Task SetCookie(string name, string value, int maxAge = 86400, string domain = "", string path = "/")
        {
            await InvokeSet(async () => await (await Module).InvokeVoidAsync("setCookie", name, value, maxAge, domain, path),
                name, value);
        }

        public async Task SetSessionCookie(string name, string value, string domain = "", string path = "/")
        {
            await InvokeSet(async () => await (await Module).InvokeVoidAsync("setSessionCookie", name, value, domain, path),
                name, value);
        }

        public async Task RemoveCookie(string name, string domain = "", string path = "/")
        {
            await InvokeSet(async () => await (await Module).InvokeVoidAsync("removeCookie", name, domain, path),
                name, null);
        }

        public async Task<string> GetCookie(string name)
        {
            if (_queue.Ready)
                return await (await Module).InvokeAsync<string>("getCookie", name);
            else
                return _tempCookies.ContainsKey(name) ? _tempCookies[name] : null;
        }

        public async ValueTask DisposeAsync()
        {
            if (_module != null)
                await (await _module).DisposeAsync();
        }
    }
}
