namespace Inspiring.MvvmTest.ViewModels.Core.Behaviors.PropertyBehaviors {
   using System;
   using System.Linq;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
using Inspiring.Mvvm.ViewModels.Core;
   using Moq;
   using Inspiring.Mvvm.ViewModels;

   [TestClass]
   public class RefreshableValueCacheBehaviorTests : TestBase {
      private IBehaviorContext Context { get; set; }
      private RefreshableValueCacheBehavior<string> Behavior { get; set; }
      private Mock<IValueAccessorBehavior<string>> SourceAccessorMock { get; set; }

      [TestInitialize]
      public void Setup() {
         SourceAccessorMock = new Mock<IValueAccessorBehavior<string>>();

         Behavior = new RefreshableValueCacheBehavior<string>();
         Behavior.Successor = SourceAccessorMock.Object;

         var helper = new ContextTestHelper();
         helper.InitializeBehaviors(Behavior);

         Context = helper.Context;
      }

      [TestMethod]
      public void UpdateFromSource_UpdatesCache() {
         SetSourceValue(ArbitraryString);
         Behavior.GetValue(Context, ValueStage.None);
         SetSourceValue(AnotherArbitraryString);
         Behavior.UpdatePropertyFromSource(Context);
         var actualValue = Behavior.GetValue(Context, ValueStage.None);

         Assert.AreEqual(AnotherArbitraryString, actualValue);
      }

      [TestMethod]
      public void UpdateSource_UpdatesSource() {
         Behavior.SetValue(Context, ArbitraryString);
         Behavior.UpdatePropertySource(Context);

         SourceAccessorMock.Verify(
            x => x.SetValue(It.IsAny<IBehaviorContext>(), ArbitraryString),
            Times.Once()
         );
      }

      private void SetSourceValue(string value) {
         SourceAccessorMock.Setup(
            x => x.GetValue(It.IsAny<IBehaviorContext>(), It.IsAny<ValueStage>())
         )
         .Returns(value);
      }
   }
}