using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;

namespace AutoFilterer.Dynamics.Tests.Environment.Statics
{
    public class AutoMoqDataAttribute : AutoDataAttribute
    {
        public AutoMoqDataAttribute(int count = 3)
            : base(() => new Fixture { RepeatCount = count,  }.Customize(new AutoMoqCustomization()))
        {
        }
    }
}
