using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;


namespace Diagnostics.Services.Dispatcher
{
    public class InstanceProviderBehavior : IServiceBehavior//Attribute, InstanceProviderBehaviorAttribute
    {
        public void AddBindingParameters(ServiceDescription serviceDescription,  
            ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, 
            BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcher cd in serviceHostBase.ChannelDispatchers)
            {
                foreach (EndpointDispatcher ed in cd.Endpoints)
                {
                    if (!ed.IsSystemEndpoint)
                    {
                        ed.DispatchRuntime.InstanceProvider = new DiagnosticsDispatcherInstanceProvider(); 
                    }
                }
            }
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

         
        
    }

    public class DiagnosticsDispatcherInstanceProvider : IInstanceProvider 
    {

        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            MessageStorageClient proxy = new MessageStorageClient();
            return new DiagnosticsDispatcher(proxy);
        }

        public object GetInstance(InstanceContext instanceContext)
        {
            return this.GetInstance(instanceContext, null);
        }

        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
        }
    }
}
