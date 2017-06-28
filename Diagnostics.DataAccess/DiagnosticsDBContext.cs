using Diagnostics.Contracts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diagnostics.DataAccess
{
 
    public class DiagnosticsDBContext : DbContext
    {
        public DiagnosticsDBContext(): base("DBConnection")
        {
            Configuration.AutoDetectChangesEnabled = true;
            Configuration.LazyLoadingEnabled = true;
            Configuration.ProxyCreationEnabled = true;
            Configuration.ValidateOnSaveEnabled = true;
            this.Configuration.ProxyCreationEnabled = false;
             AppDomain.CurrentDomain.SetData("DataDirectory", System.IO.Directory.GetCurrentDirectory());
           // var dataDirectory = ConfigurationManager.AppSettings["DataDirectory"];
           // AppDomain.CurrentDomain.SetData("DataDirectory", dataDirectory);
            this.Database.CreateIfNotExists();
        }

        protected override void Dispose(bool disposing)
        {
            Configuration.LazyLoadingEnabled = false;
            base.Dispose(disposing);
        }

        public DbSet<DiagnosticsMessage> DiagnosticsMessages { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new DiagnosticsMessageConfiguration());
        }

    }

}
