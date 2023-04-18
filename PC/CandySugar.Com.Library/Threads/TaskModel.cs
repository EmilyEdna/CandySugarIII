using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CandySugar.Com.Library.Threads
{
    public class TaskModel
    {
        public Task ThreadTask { get; set; }
        public CancellationTokenSource Cts { get; set; } = new CancellationTokenSource();
        public Action RunTask { get; set; }
    }
}
