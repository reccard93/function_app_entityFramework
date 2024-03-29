﻿using MyClassLibrary.models;
using MyClassLibrary.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyClassLibrary.Repositories
{
    public interface IDependentsRepository  : IRepository<Dipendenti, int>
    {
        Task<IEnumerable<ToDoList>> GetTasks(int id);
        IEnumerable<Dipendenti> GetAllDependents();
    }
}
