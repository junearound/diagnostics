using Diagnostics.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diagnostics.Services.Manager
{
    public class Subscriber
    {
        public string Filter { get; set; }
        public IDiagnosticsManagerCallback Callback { get; set; } 
    }
}
