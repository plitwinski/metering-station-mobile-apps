using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MeteringStation.Mobile.Messaging
{
    public class EventAggregator : IEventAggregator
    {
        readonly ConcurrentDictionary<Type, List<Action<object>>> _subscribers 
            = new ConcurrentDictionary<Type, List<Action<object>>>();

        public void Subscribe<T>(Action<T> action)
        {
            _subscribers.AddOrUpdate(typeof(T),
                k => new List<Action<object>>() { e => action((T)e) },
                (_, events) => {
                    events.Clear();
                    events.Add(e => action((T)e));
                    return events;
                });
        }

        public void Publish<T>(T sampleEvent)
        {
            if (_subscribers.ContainsKey(typeof(T)))
            {
                var callbacks = _subscribers[typeof(T)];
                callbacks.ForEach(c => c(sampleEvent));
            }
        }

        public void Dispose()
        {
            _subscribers.Clear();
        }

    }
}
