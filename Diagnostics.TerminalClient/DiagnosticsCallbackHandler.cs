using Diagnostics.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;

namespace Diagnostics.TerminalClient
{
    [CallbackBehavior(UseSynchronizationContext = true, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class DiagnosticsCallbackHandler : IDiagnosticsManagerCallback
    {

        public event EventHandler<DiagnosticsMessage> MessageAdded;
        public event EventHandler<DiagnosticsMessage> MessageUpdated;


        public void OnNewMessage(DiagnosticsMessage message)
        {
            var evt = MessageAdded;
            if (evt != null)
                evt(this, message);
        }

        public void OnUpdateMessage(DiagnosticsMessage message)
        {
            var evt = MessageUpdated;
            if (evt != null)
                evt(this, message);
        }
    }
}


