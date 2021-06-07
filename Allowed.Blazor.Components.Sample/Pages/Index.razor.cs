using Allowed.Blazor.Components.Models;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Allowed.Blazor.Components.Sample.Pages
{
    [Route("/")]
    public partial class Index
    {
        private readonly List<AutocompleteItem<int>> Colors = new()
        {
            new AutocompleteItem<int> { Id = 1, Value = "Red" },
            new AutocompleteItem<int> { Id = 2, Value = "Green" },
            new AutocompleteItem<int> { Id = 3, Value = "Blue" },
        };

        private readonly List<AutocompleteItem<int>> Directions = new()
        {
            new AutocompleteItem<int> { Id = 1, Value = "Right" },
            new AutocompleteItem<int> { Id = 2, Value = "Left" }
        };

        private readonly List<AutocompleteItem<int>> Shapes = new()
        {
            new AutocompleteItem<int> { Id = 1, Value = "Cube" },
            new AutocompleteItem<int> { Id = 2, Value = "Ball" }
        };

        private List<AutocompleteItem<int>> SearchColors = new();
        private List<AutocompleteItem<int>> SearchDirections = new();
        private List<AutocompleteItem<int>> SearchShapes = new();

        private string ColorValue { get; set; }
        private string DirectionValue { get; set; }
        private string ShapeValue { get; set; }

        public async Task OnColorInput(string value)
        {
            await Task.Delay(500);
            SearchColors = Colors.Where(i => i.Value.ToLower().Contains(value.ToLower())).ToList();
        }

        public void OnDirectionInput(string value)
        {
            SearchDirections = Directions.Where(i => i.Value.ToLower().Contains(value.ToLower())).ToList();
        }

        public void OnShapeInput(string value)
        {
            SearchShapes = Shapes.Where(i => i.Value.ToLower().Contains(value.ToLower())).ToList();
        }

        public void OnColorSelect(AutocompleteItem<int> item)
        {
            ColorValue = item.Value;
            SearchColors = Colors.Where(i => i.Value.ToLower().Contains(item.Value.ToLower())).ToList();
        }

        public void OnDirectionSelect(AutocompleteItem<int> item)
        {
            DirectionValue = string.Empty;
            SearchColors = Colors.Where(i => i.Value.ToLower().Contains(item.Value.ToLower())).ToList();
        }

        public void OnShapeSelect(AutocompleteItem<int> item)
        {
            SearchColors = Colors.Where(i => i.Value.ToLower().Contains(item.Value.ToLower())).ToList();
        }
    }
}
