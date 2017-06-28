using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Threading.Tasks;
using Diagnostics.Contracts;

namespace Diagnostics.EmitterClient
{
 
    public class EmitterClient : ClientBase<IDiagnosticsDispatcher>
    {
        public EmitterClient()
        {
        }

        public EmitterClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }
 

        public Task PushMessageAsync(DiagnosticsMessage message)
        {
            return Channel.PushMessageAsync(message);
        }
    }
}
