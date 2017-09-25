﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GTRevo.Core.Commands;
using GTRevo.Infrastructure.Core.Domain.Events;
using GTRevo.Infrastructure.Core.Domain.EventSourcing;

namespace GTRevo.Infrastructure.Core.Domain
{
    public class Saga : EventSourcedAggregateRoot, ISaga
    {
        private readonly List<ICommand> uncommitedCommands = new List<ICommand>();
        private readonly Dictionary<string, string> keys = new Dictionary<string, string>();
        private readonly ReadOnlyDictionary<Type, SagaConventionEventInfo> events;

        public Saga(Guid id) : base(id)
        {
            events = SagaConventionConfigurationCache.GetSagaConfigurationInfo(this.GetType()).Events;
            Keys = new ReadOnlyDictionary<string, string>(keys);
        }

        public override bool IsChanged => base.IsChanged || uncommitedCommands.Any();

        public IEnumerable<ICommand> UncommitedCommands => uncommitedCommands;
        public IReadOnlyDictionary<string, string> Keys { get; }

        public bool IsEnded { get; }

        public void HandleEvent(DomainEvent ev)
        {
            if (events.TryGetValue(ev.GetType(), out SagaConventionEventInfo eventInfo))
            {
                eventInfo.HandleDelegate(this, ev);
            }
        }

        public override void Commit()
        {
            base.Commit();
            uncommitedCommands.Clear();
        }

        protected void SendCommand(ICommand command)
        {
            uncommitedCommands.Add(command);
        }

        protected string GetKeyOrDefault(string name)
        {
            if (keys.TryGetValue(name, out string value))
            {
                return value;
            }

            return null;
        }

        protected void SetKey(string name, string value)
        {
            if (value != null)
            {
                keys[name] = value;
            }
            else
            {
                keys.Remove(name);
            }
        }
    }
}
