﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTRevo.Infrastructure.Core.Domain;
using GTRevo.Infrastructure.Core.Domain.Attributes;
using GTRevo.Infrastructure.Core.Domain.Events;
using NSubstitute;
using Xunit;

namespace GTRevo.Infrastructure.Tests.Core.Domain
{
    public class SagaConventionConfigurationScannerTests
    {
        [Fact]
        public void GetSagaConfiguration_GetsEventInfos()
        {
            var configurationInfo = SagaConventionConfigurationScanner.GetSagaConfiguration(typeof(Saga1));

            Assert.Equal(1, configurationInfo.Events.Count);
            Assert.True(configurationInfo.Events.TryGetValue(typeof(Event1),
                out SagaConventionEventInfo eventInfo));

            Saga1 saga = new Saga1(Guid.NewGuid());
            Event1 event1 = new Event1();
            eventInfo.HandleDelegate(saga, event1);

            Assert.Equal(1, saga.HandledEvents.Count);
            Assert.Equal(event1, saga.HandledEvents[0]);
        }

        [Fact]
        public void GetSagaConfiguration_GetsInherited()
        {
            var configurationInfo = SagaConventionConfigurationScanner.GetSagaConfiguration(typeof(Saga2));

            Assert.Equal(2, configurationInfo.Events.Count);
            Assert.True(configurationInfo.Events.TryGetValue(typeof(Event1),
                out SagaConventionEventInfo eventInfo));

            Saga1 saga = new Saga1(Guid.NewGuid());
            Event1 event1 = new Event1();
            eventInfo.HandleDelegate(saga, event1);

            Assert.Equal(1, saga.HandledEvents.Count);
            Assert.Equal(event1, saga.HandledEvents[0]);
        }

        [Fact]   
        public void GetSagaConfiguration_GetsAttributeConfiguredEvent()
        {
            var configurationInfo = SagaConventionConfigurationScanner.GetSagaConfiguration(typeof(Saga3));

            Assert.Equal(1, configurationInfo.Events.Count);
            Assert.True(configurationInfo.Events.TryGetValue(typeof(Event1),
                out SagaConventionEventInfo eventInfo));

            Saga3 saga = new Saga3(Guid.NewGuid());
            Event1 event1 = new Event1() {Foo = 5};
            eventInfo.HandleDelegate(saga, event1);

            Assert.Equal(1, saga.HandledEvents.Count);
            Assert.Equal(event1, saga.HandledEvents[0]);

            Assert.Equal("5", eventInfo.EventKeyExpression(event1));
            Assert.Equal("foo", eventInfo.SagaKey);
            Assert.True(eventInfo.IsStartingIfSagaNotFound);
        }

        public class Saga1 : Saga
        {
            public Saga1(Guid id) : base(id)
            {
            }

            public List<DomainEvent> HandledEvents { get; } = new List<DomainEvent>();

            [SagaEvent(IsAlwaysStarting = true)]
            private void Handle(Event1 ev)
            {
                HandledEvents.Add(ev);
            }
        }

        public class Saga2 : Saga1
        {
            public Saga2(Guid id) : base(id)
            {
            }

            [SagaEvent(IsAlwaysStarting = true)]
            private void Handle(Event2 ev)
            {
                HandledEvents.Add(ev);
            }
        }

        public class Saga3 : Saga
        {
            public Saga3(Guid id) : base(id)
            {
            }

            public List<DomainEvent> HandledEvents { get; } = new List<DomainEvent>();
            

            [SagaEvent(EventKey = "Foo", SagaKey = "foo", IsStartingIfSagaNotFound = true)]
            private void Handle(Event1 ev)
            {
                HandledEvents.Add(ev);
            }
        }

        public class Event1 : DomainEvent
        {
            public int Foo { get; set; }
        }

        public class Event2 : DomainEvent
        {
        }
    }
}
