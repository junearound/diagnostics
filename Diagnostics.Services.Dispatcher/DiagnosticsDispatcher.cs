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
    [ServiceBehavior(
    InstanceContextMode = InstanceContextMode.PerSession,
    ConcurrencyMode = ConcurrencyMode.Multiple)]
    //[InstanceProviderBehavior]
    public class DiagnosticsDispatcher : IDiagnosticsDispatcher 
    {
        private readonly IMessageStorage _proxy;
        public DiagnosticsDispatcher(IMessageStorage proxy)
        {
            _proxy = proxy;
            if (_proxy != null&& _proxy is ICommunicationObject)
               ( (ICommunicationObject)_proxy).Open();//TODO change
       
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
                    _proxy.SaveMessage(msg);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}: {ex.StackTrace}");
                }
            });
            await task.ConfigureAwait(false);
        }

        ~DiagnosticsDispatcher()//TODO remove
        {
            if (_proxy!=null&&_proxy is IDisposable)
                ((IDisposable)_proxy).Dispose();
           
        }

    }
}
 
