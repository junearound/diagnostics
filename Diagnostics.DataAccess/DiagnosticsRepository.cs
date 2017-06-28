using Diagnostics.Contracts;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Diagnostics.DataAccess
{
 
    public class DiagnosticsRepository : IDiagnosticsRepository
    {
        private readonly DiagnosticsDBContext _context;
        public DiagnosticsRepository(DiagnosticsDBContext context)
        {
            _context = context;
        }
        public IQueryable<DiagnosticsMessage> All
        {
            get { return this._context.DiagnosticsMessages; }
        }

        public IQueryable<DiagnosticsMessage> AllIncluding(params Expression<Func<DiagnosticsMessage, object>>[] includeProperties)
        {
            IQueryable<DiagnosticsMessage> query = this._context.DiagnosticsMessages;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return query;
        }

        public DiagnosticsMessage Find(int id)
        {
            return this._context.DiagnosticsMessages.Find(id);//.AsNoTracking()
            //await dataContext.someEntities.FirstOrDefaultAsync(e => e.Id == 1);
        }

        public void Insert(DiagnosticsMessage message)   
        {
            this._context.DiagnosticsMessages.Add(message);
            this._context.SaveChanges();
        
        }

        public void Update(DiagnosticsMessage message)   
        {
            var existing = _context.DiagnosticsMessages.Find(message.Id);
            _context.Entry(existing).CurrentValues.SetValues(message);
            this._context.SaveChanges();
        }

       
        public void Delete(int id)
        {
            var message = this._context.DiagnosticsMessages.Find(id);
            this._context.DiagnosticsMessages.Remove(message);
            this._context.SaveChanges();
        }

        public void Save()
        {
            this._context.SaveChanges();
        }
    }
}
