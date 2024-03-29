﻿using MyClassLibrary.models;
using MyClassLibrary.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyClassLibrary.Repositories
{
    public interface IToDoListRepository : IRepository<ToDoList, int>
    {
        Task<ToDoList> GetByName(string name);
    }
}
