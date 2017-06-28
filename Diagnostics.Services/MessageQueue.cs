using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diagnostics.Services
{

    //public class MessageQueue //: ...
    //{
    //    public static MessageQueue Create(string path) { }; //Nontransactional
    //    public static MessageQueue Create(string path, bool transactional) { };
    //    public static bool Exists(string path) { };
    //    public void Purge() { };
    //    //More members
    //}


    //[ServiceContract]
    //interface IMyContract//Manager
    //{
    //    [OperationContract(
    // IsOneWay = true
    // )]
    //    void MyMethod();// MessageCreated
    //}

    //    MyContractClient proxy = new MyContractClient();
    //    proxy.MyMethod();
    //proxy.Close();


    //    <system.serviceModel>
    //   ...
    //         <endpoint
    //            address = "net.msmq://localhost/private/MyServiceQueue"
    //            binding  = "netMsmqBinding"
    //bindingConfiguration = "NoMSMQSecurity"
    //            contract = "..."
    //         />
    //   ...
    //<bindings>
    //      <netMsmqBinding>
    //         <binding name = "NoMSMQSecurity" >
    //< security mode = "None"/>
    //Queued Calls
    //| 
    //547

}



//public class ServiceHost<T> : ServiceHost
//{
//    protected override void OnOpening()
//    {
//        foreach (ServiceEndpoint endpoint in Description.Endpoints)
//        {
//            endpoint.VerifyQueue();
//        }
//        base.OnOpening();
//    }
//    //More members
//}
//public static class QueuedServiceHelper
//{
//    public static void VerifyQueue(
// this
//  ServiceEndpoint endpoint)
//    {
//        if (endpoint.Binding is NetMsmqBinding)
//        {
//            string queue = GetQueueFromUri(endpoint.Address.Uri);
//            if (MessageQueue.Exists(queue) == false)
//            {
//                MessageQueue.Create(queue, true);
//            }
//        }
//    }
//    //Parses the queue name out of the address
//    static string GetQueueFromUri(Uri uri)
//    {...}
//}


//ServiceHost<MyService> host = new ServiceHost<MyService>();
//host.Open();




//client
//if(MessageQueue.Exists(@".\private$\MyServiceQueue") == false)
//{
//   MessageQueue.Create(@".\private$\MyServiceQueue",
//true
//);
//}
//MyContractClient proxy = new MyContractClient();
//proxy.MyMethod();
//proxy.Close();


//public static class QueuedServiceHelper
//{
//    public static void VerifyQueues()
//    {
//        Configuration config = ConfigurationManager.OpenExeConfiguration(
//                                                       ConfigurationUserLevel.None);
//        ServiceModelSectionGroup sectionGroup =
//                                   ServiceModelSectionGroup.GetSectionGroup(config);
//        foreach (ChannelEndpointElement endpointElement in
//                                                      sectionGroup.Client.Endpoints)
//        {
//            if (endpointElement.Binding == "netMsmqBinding")
//            {
//                string queue = GetQueueFromUri(endpointElement.Address);
//                550
//                |
//                Chapter 9: Queued Services
//               if (MessageQueue.Exists(queue) == false)
//                {
//                    MessageQueue.Create(queue, true);
//                }
//            }
//        }
//    }
//    //More members
//}

//QueuedServiceHelper.VerifyQueues();
//MyContractClient proxy = new MyContractClient();
//proxy.MyMethod();
//proxy.Close();

//EndpointAddress address = new EndpointAddress(...);
//Binding binding = new NetMsmqBinding(...); //Can still read binding from config
//MyContractClient proxy = new MyContractClient(binding, address);
//proxy.Endpoint.VerifyQueue();
//proxy.MyMethod();
//proxy.Close();




//CalculatorResponseClient proxy = new CalculatorResponseClient();
//proxy.OnAddCompleted(result,error);
//proxy.Close();