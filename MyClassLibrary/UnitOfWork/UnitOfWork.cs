using MyClassLibrary.Data;
using Microsoft.EntityFrameworkCore;
using MyClassLibrary.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyClassLibrary.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ToDoListDbContext _context;
        public DependentsRepository _dependentsRepository;
        public ToDoListRepository _toDoListRepository;

        public UnitOfWork(ToDoListDbContext context)
        {
            _context = context;
        }

        public IDependentsRepository DependentsRepository
        {
            get
            {
                if (_dependentsRepository == null)
                {
                    _dependentsRepository = new DependentsRepository(_context);
                }
                return _dependentsRepository;
            }
        }

        public IToDoListRepository ToDoListRepository
        {
            get
            {
                if (_toDoListRepository == null)
                {
                    _toDoListRepository = new ToDoListRepository(_context);
                }
                return _toDoListRepository;
            }
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }

}
