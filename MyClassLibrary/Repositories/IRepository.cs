﻿using function_app_entityFramework.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace function_app_entityFramework.Repositories
{
    public interface IRepository<TEntity, TKey> where TEntity : class
    {
        Task<TEntity> Get(TKey id);
        Task<IEnumerable<TEntity>> GetAll();
        Task<TEntity> Add(TEntity entity);
        Task Update(TEntity entity);
        Task Delete(TKey id);
        Task SaveChanges();
    }
}
