using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diagnostics.DataAccess
{
 
    public class DiagnosticsRepositoryFactory
    {
        private static DiagnosticsRepositoryFactory _instance;
        private IDiagnosticsRepository _repository;

        private DiagnosticsRepositoryFactory() { }

        public static DiagnosticsRepositoryFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DiagnosticsRepositoryFactory();
                }

                return _instance;
            }
        }

        public void SetRepository(IDiagnosticsRepository repository)
        {
            this._repository = repository;
        }

        public IDiagnosticsRepository CreateRepository()
        {
            return this._repository;
        }
    }
}
