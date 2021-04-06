using Allowed.Blazor.Components.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Allowed.Blazor.Components.Inputs
{
    public partial class Autocomplete<T> : IAsyncDisposable
    {
        [Parameter] public List<AutocompleteItem<T>> Items { get; set; }
        [Parameter] public EventCallback<string> OnInput { get; set; }
        [Parameter] public EventCallback<AutocompleteItem<T>> OnSelect { get; set; }
        [Parameter] public string NotFoundText { get; set; }
        [Parameter] public int? InputDelay { get; set; }

        private string InputValue { get; set; }
        private bool Focused { get; set; } = false;
        private bool Typing { get; set; } = false;
        private bool Processing { get; set; } = false;
        public AutocompleteItem<T> SelectedItem { get; private set; }

        private Timer Timer;
        private static readonly object _locker = new();

        private async Task Input(ChangeEventArgs e)
        {
            SelectedItem = null;

            InputValue = e.Value.ToString();
            Typing = true;

            if (!InputDelay.HasValue)
            {
                await OnInput.InvokeAsync(e.Value.ToString());
                Typing = false;

                return;
            }

            lock (_locker)
            {
                Timer?.Change(Timeout.Infinite, 0);

                if (!string.IsNullOrEmpty(InputValue))
                    Timer = new(SendRequest, e.Value.ToString(), InputDelay.Value, Timeout.Infinite);
            }
        }

        private async void SendRequest(object state)
        {
            await InvokeAsync(async () =>
            {
                Processing = true;
                await OnInput.InvokeAsync(state.ToString());
                Processing = false;
                Typing = false;

                StateHasChanged();
            });
        }

        private void FocusOut() => Focused = false;
        private void Focus() => Focused = true;

        private bool IsDropdownShow() => Focused && !Typing && !string.IsNullOrEmpty(InputValue);

        private async Task SelectItem(AutocompleteItem<T> item)
        {
            SelectedItem = item;

            InputValue = item.Value;
            await OnSelect.InvokeAsync(item);

            Focused = false;
        }

        public async ValueTask DisposeAsync()
        {
            if (Timer != null)
                await Timer.DisposeAsync();
        }
    }
}
