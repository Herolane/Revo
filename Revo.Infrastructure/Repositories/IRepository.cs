﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Revo.Core.Transactions;
using Revo.DataAccess.Entities;
using Revo.Domain.Entities;

namespace Revo.Infrastructure.Repositories
{
    public interface IRepository : IDisposable
    {
        void Add<T>(T aggregate) where T : class, IAggregateRoot;

        T FirstOrDefault<T>(Expression<Func<T, bool>> predicate) where T : class, IAggregateRoot, IQueryableEntity;
        T First<T>(Expression<Func<T, bool>> predicate) where T : class, IAggregateRoot, IQueryableEntity;
        Task<T> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> predicate) where T : class, IAggregateRoot, IQueryableEntity;
        Task<T> FirstAsync<T>(Expression<Func<T, bool>> predicate) where T : class, IAggregateRoot, IQueryableEntity;
        
        T Find<T>(Guid id) where T : class, IAggregateRoot;
        Task<T> FindAsync<T>(Guid id) where T : class, IAggregateRoot;
        IQueryable<T> FindAll<T>() where T : class, IAggregateRoot, IQueryableEntity;
        Task<IList<T>> FindAllAsync<T>() where T : class, IAggregateRoot, IQueryableEntity;
        Task<IList<T>> FindAllAsync<T>(Expression<Func<T, bool>> predicate) where T : class, IAggregateRoot, IQueryableEntity;

        T Get<T>(Guid id) where T : class, IAggregateRoot;
        Task<T> GetAsync<T>(Guid id) where T : class, IAggregateRoot;
        IAsyncQueryableResolver GetQueryableResolver<T>() where T : class, IAggregateRoot, IQueryableEntity;

        IQueryable<T> Where<T>(Expression<Func<T, bool>> predicate) where T : class, IAggregateRoot, IQueryableEntity;
        
        void Remove<T>(T aggregate) where T : class, IAggregateRoot;

        Task SaveChangesAsync();
    }
}
