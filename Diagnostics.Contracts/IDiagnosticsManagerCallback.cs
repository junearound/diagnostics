using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Diagnostics.Contracts
{
    [ServiceContract]
    public interface IDiagnosticsManagerCallback
    {
        [OperationContract(IsOneWay = true)]
        void OnNewMessage(DiagnosticsMessage message);//Task

        [OperationContract(IsOneWay = true)]
        void OnUpdateMessage(DiagnosticsMessage message);//Task

        //[OperationContract(IsOneWay = true)]
        //void GetMessages(IEnumerable<DiagnosticsMessage> messages);
    }
}


 

 