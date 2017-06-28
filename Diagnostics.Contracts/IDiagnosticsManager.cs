using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Diagnostics.Contracts
{
     
    [ServiceContract(CallbackContract = typeof(IDiagnosticsManagerCallback))]
    public interface IDiagnosticsManager
    {
 
        [OperationContract(IsOneWay = false)]//(IsInitiating = true)
        Task<bool> Subscribe(string filter);

        [OperationContract(IsOneWay = false)]
        Task<bool> Unsubscribe();

        [OperationContract(IsOneWay = true)]
        Task ChangeFilter(string newFilter);

        [OperationContract(IsOneWay = true)]//(IsOneWay = true, IsTerminating = true)
        Task UpdateMessage(DiagnosticsMessage message);

        //[OperationContract(IsOneWay = false)]
        //Task<IEnumerable<DiagnosticsMessage>> GetMessages();

    }
}
