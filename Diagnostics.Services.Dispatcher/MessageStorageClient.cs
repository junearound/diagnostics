using Diagnostics.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Diagnostics.Services.Dispatcher
{

    public class MessageStorageClient : ClientBase<IMessageStorage>
    {
        public MessageStorageClient()
        {
        }

        public MessageStorageClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }
        

        public Task SaveMessage(DiagnosticsMessage message)
        {
            return Channel.SaveMessage(message);
        }
    }
}
