using MyClassLibrary.Data;
using MyClassLibrary.models;
using MyClassLibrary.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyClassLibrary.Repositories
{
    public class DependentsRepository : Repository<Dipendenti, int>, IDependentsRepository
    {
        private ToDoListDbContext _ToDoListDbContext;
        public DependentsRepository(ToDoListDbContext Context) : base(Context)
        {
            _ToDoListDbContext = Context;
        }

        public async Task<IEnumerable<ToDoList>> GetTasks(int id)
        {
            var dependent = await Get(id);
            return dependent.Tasks;
        }
        public IEnumerable<Dipendenti> GetAllDependents()
        {
            return _ToDoListDbContext.Dipendenti.ToList();
        }
    }
}
