using System;

namespace MeteringStation.Mobile.Messaging
{
    public interface IEventAggregator : IDisposable
    {
        void Publish<T>(T sampleEvent);
        void Subscribe<T>(Action<T> action);
    }
}