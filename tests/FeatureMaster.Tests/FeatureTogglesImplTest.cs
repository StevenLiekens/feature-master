using FeatureMaster.Infrastructure;
using Microsoft.Extensions.Options;
using Moq;
using System;
using Xunit;

namespace FeatureMaster.Tests
{
    public class SimpleFeature : Feature
    {
        public bool UseSimpleFeature { get; set; }
    }

    public class SimpleContext
    {
        public bool Production { get; set; }
    }

    public class FeatureTogglesImplTest
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void FeatureTogglesImpl_ShouldReturnConfiguredToggles_WhenContextIsUnspecified(bool featureToggle)
        {
            var fakeConfiguration = new Mock<IOptionsMonitor<SimpleFeature>>(MockBehavior.Strict);
            fakeConfiguration.SetupGet(mock => mock.CurrentValue).Returns(new SimpleFeature
            {
                UseSimpleFeature = featureToggle
            });

            var fakeRouterFactory = new Mock<IServiceProvider>(MockBehavior.Strict);

            var sut = new FeatureTogglesImpl<SimpleFeature>(fakeConfiguration.Object, fakeRouterFactory.Object);

            var result = sut.GetFeatureToggles();
            Assert.Equal(featureToggle, result.UseSimpleFeature);
        }

        [Fact]
        public void FeatureTogglesImpl_ShouldInvokeToggleRouter_WhenContextIsSpecified()
        {
            var fakeConfiguration = new Mock<IOptionsMonitor<SimpleFeature>>(MockBehavior.Strict);
            var fakeToggleRouter = new Mock<IFeatureToggleRouter<SimpleFeature, SimpleContext>>(MockBehavior.Strict);
            var fakeRouterFactory = new Mock<IServiceProvider>(MockBehavior.Strict);


            fakeRouterFactory.Setup(sp => sp.GetService(typeof(IFeatureToggleRouter<SimpleFeature, SimpleContext>)))
                .Returns(fakeToggleRouter.Object);

            var sut = new FeatureTogglesImpl<SimpleFeature>(fakeConfiguration.Object, fakeRouterFactory.Object);

            var featureConfiguration = new SimpleFeature
            {
                UseSimpleFeature = true
            };

            var simpleContext = new SimpleContext
            {
                Production = true
            };

            fakeConfiguration.SetupGet(mock => mock.CurrentValue).Returns(featureConfiguration);
            fakeToggleRouter.Setup(router => router.Toggle(It.Is<SimpleFeature>(feature => feature.UseSimpleFeature == featureConfiguration.UseSimpleFeature), simpleContext))
                            .Callback<SimpleFeature, SimpleContext>((feature, context) =>
                            {
                                if (context.Production)
                                {
                                    feature.UseSimpleFeature = false;
                                }
                            })
                            .Verifiable();

            var result = sut.GetFeatureToggles(simpleContext);
            fakeToggleRouter.VerifyAll();
            Assert.False(result.UseSimpleFeature);
        }
    }
}
