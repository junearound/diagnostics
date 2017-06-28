using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Diagnostics.Contracts
{
    [DataContract(Name = "Severity")]
    public enum SeverityEnum
    {
        [EnumMember]
        Statistics = 0,
        [EnumMember]
        Error = 1,
        [EnumMember]
        Warning = 2
    }
}
