using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Diagnostics.Contracts
{
    [DataContract, Serializable]
    public class DiagnosticsMessage
    {  
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public Guid SourceId { get; set; }

        [DataMember]
        public SeverityEnum Severity { get; set; }

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public bool IsRead { get; set; } = false;

        [DataMember]
        public Guid Uid { get; set; }

        public override string ToString()
        {
            return $"SourceId:{this.SourceId}; Text:{this.Text}; Severity:{this.Severity}";
        }
    }
}


 
