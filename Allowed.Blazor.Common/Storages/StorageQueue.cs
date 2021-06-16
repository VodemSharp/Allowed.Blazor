using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Allowed.Blazor.Common.Storages
{
    public class StorageQueue
    {
        public bool Ready { get; set; }
        public Queue<Func<Task>> Tasks { get; set; } = new();
    }
}
