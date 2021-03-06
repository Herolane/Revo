﻿using System;
using System.Diagnostics;
using Revo.Core.Events;
using Revo.Core.Types;
using Revo.Infrastructure.EventStores;
using Revo.Infrastructure.EventStores.Generic;

namespace Revo.Infrastructure.Events.Async.Generic
{
    public class AsyncEventQueueRecordAdapter : IAsyncEventQueueRecord
    {
        private readonly QueuedAsyncEvent queuedEvent;
        private readonly IEventSerializer eventSerializer;
        private IEventMessage eventMessage;

        public AsyncEventQueueRecordAdapter(QueuedAsyncEvent queuedEvent, IEventSerializer eventSerializer)
        {
            this.queuedEvent = queuedEvent;
            this.eventSerializer = eventSerializer;
        }

        public Guid Id => queuedEvent.Id;
        public Guid EventId => queuedEvent.EventStreamRowId ?? queuedEvent.ExternalEventRecordId ?? throw new InvalidOperationException("QueuedAsyncEvent has no event id");
        public string QueueName => queuedEvent.QueueId;
        public long? SequenceNumber => queuedEvent.SequenceNumber;

        public IEventMessage EventMessage
        {
            get
            {
                if (eventMessage == null)
                {
                    Debug.Assert(queuedEvent.EventStreamRow != null || queuedEvent.ExternalEventRecord != null);

                    if (queuedEvent.EventStreamRow != null)
                    {
                        var record = new EventStoreRecordAdapter(queuedEvent.EventStreamRow, eventSerializer);
                        eventMessage = EventStoreEventMessage.FromRecord(record);
                    }
                    else
                    {
                        var @event = eventSerializer.DeserializeEvent(queuedEvent.ExternalEventRecord.EventJson,
                            new VersionedTypeId(queuedEvent.ExternalEventRecord.EventName, queuedEvent.ExternalEventRecord.EventVersion));
                        var metadata =  eventSerializer.DeserializeEventMetadata(queuedEvent.ExternalEventRecord.MetadataJson);
                        eventMessage = Revo.Core.Events.EventMessage.FromEvent(@event, metadata);
                    }
                }

                return eventMessage;
            }
        }
    }
}
