using MyClassLibrary.Data;
using MyClassLibrary.models;
using MyClassLibrary.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyClassLibrary.Repositories
{
    public class ToDoListRepository : Repository<ToDoList, int>, IToDoListRepository
    {
        private ToDoListDbContext _ToDoListDbContext;
        public ToDoListRepository(ToDoListDbContext dbContext) : base(dbContext)
        {
            _ToDoListDbContext = dbContext;
        }

        public async Task<ToDoList> GetByName(string name)
        {
            return await _dbContext.Set<ToDoList>().FirstOrDefaultAsync(t => t.Name == name);
        }
    }
}
