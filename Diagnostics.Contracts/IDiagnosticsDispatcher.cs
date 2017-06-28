using System.ServiceModel;
using System.Threading.Tasks;

namespace Diagnostics.Contracts
{
     
    [ServiceContract]
    public interface IDiagnosticsDispatcher
    {
        [OperationContract(IsOneWay = true)]
        Task PushMessageAsync(DiagnosticsMessage msg);
    }
}
