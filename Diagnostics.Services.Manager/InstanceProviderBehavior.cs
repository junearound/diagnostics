using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Diagnostics.DataAccess;

namespace Diagnostics.Services.Manager
{
    public class InstanceProviderBehaviorAttribute : Attribute,IServiceBehavior//Attribute, InstanceProviderBehaviorAttribute
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
                        ed.DispatchRuntime.InstanceProvider = new DiagnosticsManagerInstanceProvider();// ServiceInstanceProvider();
                    }
                }
            }
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

         
        
    }

    public class DiagnosticsManagerInstanceProvider : IInstanceProvider//ServiceInstanceProvider
    {

        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            //DiagnosticsRepositoryFactory.Instance.CreateProductRepository(new DiagnosticsDBContext());
            IDiagnosticsRepository repository = DiagnosticsRepositoryFactory.Instance.CreateRepository();
            return new DiagnosticsManager(repository);
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
