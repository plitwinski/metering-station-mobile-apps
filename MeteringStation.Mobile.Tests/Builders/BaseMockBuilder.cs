using Moq;

namespace MeteringStation.Mobile.Tests.Builders
{
    abstract class BaseMockBuilder<T> where T : class
    {
        public BaseMockBuilder(bool callBase = false)
        {
            Mock = new Mock<T>() { CallBase = callBase };
        }

        public Mock<T> Mock { get; }

        public T Build()
            => Mock.Object;
    }
}
