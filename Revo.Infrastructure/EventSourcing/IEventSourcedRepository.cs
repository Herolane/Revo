﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Revo.Core.Transactions;
using Revo.DataAccess.Entities;
using Revo.Domain.Entities.EventSourcing;

namespace Revo.Infrastructure.EventSourcing
{
    public interface IEventSourcedRepository<TBase>
        where TBase : class, IEventSourcedAggregateRoot
    {
        IEnumerable<IRepositoryFilter> DefaultFilters { get; }
        bool IsChanged { get; }

        void Add<T>(T aggregate) where T : class, TBase;

        T Find<T>(Guid id) where T : class, TBase;
        TBase Find(Guid id);
        Task<T> FindAsync<T>(Guid id) where T : class, TBase;
        Task<TBase> FindAsync(Guid id);

        T Get<T>(Guid id) where T : class, TBase;
        TBase Get(Guid id);
        Task<T> GetAsync<T>(Guid id) where T : class, TBase;
        Task<TBase> GetAsync(Guid id);

        IEnumerable<TBase> GetLoadedAggregates();

        void Remove<T>(T aggregateRoot) where T : class, TBase;
        
        Task SaveChangesAsync();
    }
}
