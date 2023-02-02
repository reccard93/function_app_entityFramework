using MyClassLibrary.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyClassLibrary.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IToDoListRepository ToDoListRepository { get; }
        IDependentsRepository DependentsRepository { get; }
        Task Save();
    }
}
