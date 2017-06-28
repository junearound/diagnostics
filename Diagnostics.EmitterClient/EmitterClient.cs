using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Threading.Tasks;
using Diagnostics.Contracts;

namespace Diagnostics.EmitterClient
{
 
    public partial  class EmitterClient : ClientBase<IDiagnosticsDispatcher>
    {
        public EmitterClient()
        {
        }

        public EmitterClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        #region IDiagnosticsDispatcher 
        public Task PushMessageAsync(DiagnosticsMessage message)
        {
            return Channel.PushMessageAsync(message);
        }
        #endregion
    }
    public partial class EmitterClient : IDisposable
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

        ~EmitterClient()
        {
            Dispose(false);
        }

        #endregion
    }
}
