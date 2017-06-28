using Diagnostics.Contracts;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diagnostics.DataAccess
{
     
    public class DiagnosticsMessageConfiguration : EntityTypeConfiguration<DiagnosticsMessage>
    {
        public DiagnosticsMessageConfiguration()
        {
            this.Property(m => m.Text).HasMaxLength(10);
            this.Property(m => m.Uid)
                    .IsRequired();
            this.Property(m => m.SourceId)
                    .IsRequired();
            this.HasKey(b => b.Id);
        }
    }
}
