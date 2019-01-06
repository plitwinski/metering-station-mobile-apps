using Moq;

namespace MeteringStation.Mobile.Tests.Builders
{
    abstract class BaseMockBuilder<T> where T : class
    {
        public BaseMockBuilder()
        {
            Mock = new Mock<T>();
        }

        public Mock<T> Mock { get; }

        public T Build()
            => Mock.Object;
    }
}
