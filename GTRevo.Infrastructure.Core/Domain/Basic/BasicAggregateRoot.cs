﻿using System;
using GTRevo.DataAccess.Entities;

namespace GTRevo.Infrastructure.Core.Domain.Basic
{
    /// <summary>
    /// Aggregate root that is typically persisted to a RDBMS store using an ORM.
    /// </summary>
    [DatabaseEntity]
    public abstract class BasicAggregateRoot : AggregateRoot, IQueryableEntity, IRowVersioned
    {
        public BasicAggregateRoot(Guid id) : base(id)
        {
        }

        protected BasicAggregateRoot()
        {
        }

        public new int Version
        {
            get { return base.Version; }
            set { base.Version = value;  }
        }
    }
}
