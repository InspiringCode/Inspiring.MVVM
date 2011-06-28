namespace Inspiring.MvvmTest.Common.Behaviors {
   using System;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.Common.Behaviors;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class BehaviorFactoryConfigurationTests : TestBase {
      private TestBehaviorFactoryConfiguration Configuration { get; set; }
      private IBehaviorFactoryConfiguration ConfigurationInterface { get { return Configuration; } }

      [TestInitialize]
      public void Setup() {
         Configuration = new TestBehaviorFactoryConfiguration();
      }

      [TestMethod]
      public void GetFactory_WithCorrectProvider_ReturnsResultOfAbstractGetFactory() {
         Configuration.FactoryToReturn = Mock<IBehaviorFactory>();

         var expectedProvider = new TestBehaviorFactoryProvider();
         var actualFactory = ConfigurationInterface.GetFactory(expectedProvider);

         Assert.AreEqual(expectedProvider, Configuration.LastGetFactoryArg);
         Assert.AreEqual(Configuration.FactoryToReturn, actualFactory);
      }

      [TestMethod]
      public void GetFactory_WithWrongProvider_ThrowsException() {
         var expectedMessage = ECommon
            .WrongBehaviorFactoryProviderType
            .FormatWith(
               typeof(TestBehaviorFactoryProvider).Name,
               typeof(WrongBehaviorFactoryProvider).Name
            );

         AssertHelper.Throws<ArgumentException>(() =>
            ConfigurationInterface.GetFactory(new WrongBehaviorFactoryProvider())
         ).WithMessage(expectedMessage);
      }

      private class TestBehaviorFactoryConfiguration :
         BehaviorFactoryConfiguration<TestBehaviorFactoryProvider> {

         public IBehaviorFactory FactoryToReturn { get; set; }
         public TestBehaviorFactoryProvider LastGetFactoryArg { get; set; }

         protected override IBehaviorFactory GetFactory(TestBehaviorFactoryProvider factoryProvider) {
            LastGetFactoryArg = factoryProvider;
            return FactoryToReturn;
         }
      }

      private class TestBehaviorFactoryProvider : IBehaviorFactoryProvider { }

      private class WrongBehaviorFactoryProvider : IBehaviorFactoryProvider { }
   }
}