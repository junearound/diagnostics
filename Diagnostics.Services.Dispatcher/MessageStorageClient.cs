using Diagnostics.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Diagnostics.Services.Dispatcher
{

    public partial class MessageStorageClient : ClientBase<IMessageStorage>, IMessageStorage
    {
        public MessageStorageClient()
        {
        }

        public MessageStorageClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        #region IMessageStorage 
        public Task SaveMessage(DiagnosticsMessage message)
        {
            return Channel.SaveMessage(message);
        }
        #endregion
    }
    public partial class MessageStorageClient : IDisposable
    {
        #region IDisposable 

        void IDisposable.Dispose()
        {
            Dispose(true);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    if (State != CommunicationState.Faulted)
                    {
                        Close();
                    }
                }
                finally
                {
                    if (State != CommunicationState.Closed)
                    {
                        Abort();
                    }
                }
            }
        }

        ~MessageStorageClient()
        {
            Dispose(false);
        }

        #endregion
    }
}
