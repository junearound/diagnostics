using Diagnostics.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Diagnostics.DataAccess
{
     
    public interface IDiagnosticsRepository
    {
        IQueryable<DiagnosticsMessage> All { get; }
        IQueryable<DiagnosticsMessage> AllIncluding(params Expression<Func<DiagnosticsMessage, object>>[] includeProperties);
        DiagnosticsMessage Find(int id); //TODO uid
        void Insert(DiagnosticsMessage message);
        void Update(DiagnosticsMessage message);
        void Delete(int id);
        void Save();
    }

}
