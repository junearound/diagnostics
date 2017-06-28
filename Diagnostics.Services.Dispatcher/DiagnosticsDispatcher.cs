using Diagnostics.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Diagnostics.Services.Dispatcher
{

 
    //[ServiceBehavior(
    //InstanceContextMode = InstanceContextMode.Single,
    //ConcurrencyMode = ConcurrencyMode.Multiple)]
    [ServiceBehavior(
    InstanceContextMode = InstanceContextMode.PerSession,
    ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class DiagnosticsDispatcher : IDiagnosticsDispatcher 
    {
        private readonly MessageStorageClient _proxy = new MessageStorageClient();
        public DiagnosticsDispatcher()
        {
            _proxy.Open();
           // _proxy = new MessageStorageClient();
        }

        public async Task PushMessageAsync(DiagnosticsMessage msg) 
        {
            if (msg == null)
                return;
            var task = Task.Factory.StartNew(() =>
            {
                try
                {

                    string msgDescription = msg.ToString(); 
                    Console.WriteLine($"Новое сообщение: {msgDescription}");


                    //MessageStorageClient proxy = new MessageStorageClient();//Create proxy
                    //proxy.Open();
                    //proxy.SaveMessage(msg);
                    //proxy.Close();

                    //_proxy.Open();
                    _proxy.SaveMessage(msg);
                    //_proxy.Close();
                }
                catch (Exception ex)//(FaultException fe)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}: {ex.StackTrace}");
                }
            });
            await task.ConfigureAwait(false);
        }

        ~DiagnosticsDispatcher()//TODO remove
        {
            _proxy.Close();
           
        }

    }
}
 
