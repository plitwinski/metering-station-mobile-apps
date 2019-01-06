using MeteringStation.Mobile.Messaging;
using Moq;
using System;

namespace MeteringStation.Mobile.Tests.Messaging
{
    public class EventAggregatorTests
    {
        public class TestEvent { }
        public class TestEvent2 { }

        public void WhenSubscribedOnce_MessageReceivedOnce()
        {
            var e = new TestEvent();
            var actionMock = new Mock<Action<TestEvent>>();
            var target = new EventAggregator();
            target.Subscribe(actionMock.Object);
            target.Publish(e);

            actionMock.Verify(p => p(e), Times.Once);
        }

        public void WhenSubscribedTwice_MessageReceivedTwice()
        {
            var e = new TestEvent();
            var actionMock = new Mock<Action<TestEvent>>();
            var target = new EventAggregator();
            target.Subscribe(actionMock.Object);
            target.Subscribe(actionMock.Object);
            target.Publish(e);

            actionMock.Verify(p => p(e), Times.Exactly(2));
        }

        public void WhenSubscribedForTwoDifferentEvents_EachEventIsReceivedOnce()
        {
            var e1 = new TestEvent();
            var e2 = new TestEvent2();
            var actionMock1 = new Mock<Action<TestEvent>>();
            var actionMock2 = new Mock<Action<TestEvent2>>();
            var target = new EventAggregator();
            target.Subscribe(actionMock1.Object);
            target.Subscribe(actionMock2.Object);
            target.Publish(e1);
            target.Publish(e2);

            actionMock1.Verify(p => p(e1), Times.Once);
            actionMock2.Verify(p => p(e2), Times.Once);
        }
    }
}
