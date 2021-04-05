using Allowed.Blazor.Components.Models;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Allowed.Blazor.Components.Inputs
{
    public partial class Autocomplete<T>
    {
        [Parameter] public List<AutocompleteItem<T>> Items { get; set; }
        [Parameter] public EventCallback<string> OnInput { get; set; }
        [Parameter] public EventCallback<AutocompleteItem<T>> OnSelect { get; set; }
        [Parameter] public string NotFoundText { get; set; }

        private string InputValue { get; set; }
        private bool Focused { get; set; } = false;
        public AutocompleteItem<T> SelectedItem { get; private set; }

        private async Task Input(ChangeEventArgs e)
        {
            SelectedItem = null;

            InputValue = e.Value.ToString();
            await OnInput.InvokeAsync(InputValue);

            if (!string.IsNullOrEmpty(InputValue))
                Focused = true;
            else
                Focused = false;
        }

        private void FocusOut() => Focused = false;
        private void Focus() => Focused = true;

        private bool IsDropdownShow() => Focused && !string.IsNullOrEmpty(InputValue);

        private async Task SelectItem(AutocompleteItem<T> item)
        {
            SelectedItem = item;

            InputValue = item.Value;
            await OnSelect.InvokeAsync(item);

            Focused = false;
        }
    }
}
