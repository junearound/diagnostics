using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Diagnostics.Services;
using System.Threading;
using Diagnostics.Services.Dispatcher;
using Diagnostics.Services.Manager;
using System.Messaging;
using Diagnostics.DataAccess;
using Diagnostics.Contracts;

namespace Diagnostics.ServiceHost
{

    class Program
    {
        static Mutex _mutex; 
        internal static System.ServiceModel.ServiceHost _dispatcherServiceHost = null;
        internal static System.ServiceModel.ServiceHost _managerServiceHost = null;
        private static void StartDispatcherService()
        {

            _dispatcherServiceHost = new System.ServiceModel.ServiceHost(typeof(DiagnosticsDispatcher));
            _dispatcherServiceHost.Description.Behaviors.Add(new InstanceProviderBehavior());
            _dispatcherServiceHost.Open();
        }

        private static void StopDispatcherService()
        {
            if (_dispatcherServiceHost.State != CommunicationState.Closed)
                _dispatcherServiceHost.Close();
        }

        private static void StartManagerService()
        {
            //when the InstanceContextMode is SingleCall the IInstanceProvider is not invoked to create the instance
            //TODO factory
            IDiagnosticsRepository repository = new DiagnosticsRepository(new DiagnosticsDBContext());
            _managerServiceHost = new System.ServiceModel.ServiceHost(new DiagnosticsManager(repository));
            _managerServiceHost.Open();
        }     

        private static void StopManagerService()
        {
            if (_managerServiceHost.State != CommunicationState.Closed)
                _managerServiceHost.Close();
        }



        static void Main(string[] args)
        {
            string mutex_id = "{A78E8BD8-B2C9-49E0-B16D-ABF3078CBABF}";
            _mutex = new Mutex(false, mutex_id);
            if (!_mutex.WaitOne(0, false))
            {
                Console.WriteLine("Приложение уже  запущено");
                return;
            }

            try
            {
                string queue = @".\private$\DiagnosticsQueue";
                if (!MessageQueue.Exists(queue))
                {
                    MessageQueue.Create(queue, false);
                }


                StartDispatcherService();
                StartManagerService();
                Console.WriteLine("Сервисы размещены, для выхода нажмите любую клавишу\n");
                Console.WriteLine("Конечные точки :\n");
                _dispatcherServiceHost.Description.Endpoints.ToList().ForEach(endpoints => Console.WriteLine(endpoints.Address.ToString()));
                _managerServiceHost.Description.Endpoints.ToList().ForEach(endpoints => Console.WriteLine(endpoints.Address.ToString()));
                Console.ReadKey();
                StopDispatcherService();
                StopManagerService();
                Console.WriteLine("Сервисы остановлены");
                _mutex.ReleaseMutex();
            }
            catch (Exception e)
            {
                Console.WriteLine("\nEXCEPTION: ");
                Console.WriteLine(e);
                Console.ReadKey();
            }

        }



        //public ServiceHostBase CreateServiceHost(string service, Uri[] baseAddresses)

        //{

        //    // The service parameter is ignored here because we know our service.

        //    ServiceHost serviceHost = new ServiceHost(typeof(HelloService),

        //        baseAddresses);

        //    serviceHost.Opening += new EventHandler(serviceHost_Opening);

        //    serviceHost.Opened += new EventHandler(serviceHost_Opened);

        //    serviceHost.Closing += new EventHandler(serviceHost_Closing);

        //    serviceHost.Closed += new EventHandler(serviceHost_Closed);

        //    serviceHost.Faulted += new EventHandler(serviceHost_Faulted);

        //    serviceHost.UnknownMessageReceived += new EventHandler<UnknownMessageReceivedEventArgs>(serviceHost_UnknownMessageReceived);



        //    return serviceHost;

        //}


        //public void ConfigureServices(IServiceCollection services)
        //{
        //    // Add framework services.
        //    services.AddMvc();
        //    services.AddSingleton<IProductRepository, ProductRepository>();
        //}
    }
}
