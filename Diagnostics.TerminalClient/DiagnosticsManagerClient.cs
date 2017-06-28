using Diagnostics.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Diagnostics.TerminalClient
{
    
    public class DiagnosticsManagerClient : DuplexClientBase<IDiagnosticsManager>, IDiagnosticsManager
    {
        public DiagnosticsManagerClient(InstanceContext callbackInstance, WSDualHttpBinding binding, EndpointAddress endpointAddress)
            : base(callbackInstance, binding, endpointAddress) { }

        public Task<bool> Subscribe(string filter)
        {
           return Channel.Subscribe(filter);
        }
        public Task<bool> Unsubscribe()
        {
            return Channel.Unsubscribe();
        }
        public Task ChangeFilter(string newFilter)
        {
            return Channel.ChangeFilter(newFilter);
        }
        public Task UpdateMessage(DiagnosticsMessage message)
        {
            return Channel.UpdateMessage(message);
        }

    }
    //endPoint.Behaviors.Add(new MyBehavior());
    ////or
    //foreach (var endPoint in host.Description.Endpoints)
    //endPoint.Behaviors.Add(new MyBehavior());
    //public class MyBehavior : IEndpointBehavior
    //{
    //    public void AddBindingParameters(ServiceEndpoint endpoint,
    //        BindingParameterCollection bindingParameters)
    //    {
    //    }
    //    public void ApplyClientBehavior(ServiceEndpoint endpoint,
    //        ClientRuntime clientRuntime)
    //    {
    //    }
    //    public void ApplyDispatchBehavior(ServiceEndpoint endpoint,
    //        EndpointDispatcher endpointDispatcher)
    //    {
    //        endpointDispatcher.DispatchRuntime.AutomaticInputSessionShutdown = false;
    //    }
    //    public void Validate(ServiceEndpoint endpoint)
    //    {
    //    }
    //}
}
