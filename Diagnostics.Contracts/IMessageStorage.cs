using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Diagnostics.Contracts
{
    
    [ServiceContract]
    public interface IMessageStorage
    {
        [OperationContract(IsOneWay = true)]
        Task SaveMessage(DiagnosticsMessage msg);
    }
}
