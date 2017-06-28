using Diagnostics.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diagnostics.Services
{
    //    <endpoint
    //   address = "net.msmq://localhost/
    //private
    ///MyServiceQueue"
    //   binding  = "netMsmqBinding"
    //   ...
    ///>
    //    public class MessageQueue : ...
    //{
    //        public static MessageQueue Create(string path); //Nontransactional
    //        public static MessageQueue Create(string path, bool transactional);
    //        public static bool Exists(string path);
    //        public void Purge();
    //        //More members
    //    }
//    ServiceHost host = new ServiceHost(typeof(MyService));
//if(MessageQueue.Exists(@".\private$\MyServiceQueue") == false)
//{
//   MessageQueue.Create(@".\private$\MyServiceQueue",
//true
//);
//}
//host.Open();

    //InstanceContextMode •	Single or •	PerSession  ConcurrencyMode •	Multiple 
    public class DiagnosticsService : IDiagnosticsDispatcher//, IDiagnosticsManager, IDiagnosticsService
    {
        //DiagnosticsMessage[] Messages = null;//ConcurrentDictionary
        public DiagnosticsService()
        {
             
        }

        //public async Task<string> PushMessage(Message msg)
        //{
        //    var task = Task.Factory.StartNew(() =>
        //    {
        //        Thread.Sleep(10000);
        //        return "Return from Server : " + msg;
        //    });
        //    return await task.ConfigureAwait(false);
        //}

        //public async Task<string> HelloAsync(string name)
        //{
        //    return await Task.Factory.StartNew(() => "hello " + name);
        //}

        
 

        public async Task PushMessage(DiagnosticsMessage msg)//ok
        {
            var task = Task.Factory.StartNew(() =>
            {
           
                string msgDescription = msg.ToString();//string msgDescription =  $"SourceId:{msg.SourceId}; Text:{msg.Text}; Severity:{msg.Severity}";
                Console.WriteLine(msgDescription);//"Message added "+ msg.SourceId + msg.Text+ msg.Severity
                                                  //    Console.WriteLine("Current threadId: " + Thread.CurrentThread.ManagedThreadId);
                                                  //return "Return from Server : " + msg;
                                                  // MessageBox.Show($"{msg.SourceId} - {msg.Text}");
                //MessageStoreClient proxy = new MessageStoreClient();//Create proxy
                //await proxy.SaveMessage(msg);
                //proxy.Close();
            });
            await task.ConfigureAwait(false);
            // return await task.ConfigureAwait(false);
            //return  task;

        }

        //public async Task<Message[]> GetMessages()
        //{
        //    //Thread.Sleep(10000);
        //    await Task.Delay(TimeSpan.FromSeconds(1));
        //    return Messages;
        //}

    }
}
//[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
