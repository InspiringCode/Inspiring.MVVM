namespace Inspiring.MvvmTest.ViewModels.Core.Behaviors.PropertyBehaviors {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Moq;

   [TestClass]
   public class ValueCacheBehaviorTests : TestBase {
      private IBehaviorContext Context { get; set; }
      private ValueCacheBehavior<string> Behavior { get; set; }
      private Mock<IValueAccessorBehavior<string>> SourceAccessorMock { get; set; }

      [TestInitialize]
      public void Setup() {
         SourceAccessorMock = new Mock<IValueAccessorBehavior<string>>();

         Behavior = new ValueCacheBehavior<string>();
         Behavior.Successor = SourceAccessorMock.Object;

         var helper = new ContextTestHelper();
         helper.InitializeBehaviors(Behavior);

         Context = helper.Context;
      }

      [TestMethod]
      public void GetValue_Initially_ReturnsSourceValue() {
         SetSourceValue(ArbitraryString);
         var actualValue = Behavior.GetValue(Context);

         Assert.AreEqual(ArbitraryString, actualValue);
      }

      [TestMethod]
      public void GetValue_SecondTime_ReturnsCachedValue() {
         SetSourceValue(ArbitraryString);
         Behavior.GetValue(Context);
         SetSourceValue(AnotherArbitraryString);
         
         var actualValue = Behavior.GetValue(Context);

         Assert.AreEqual(ArbitraryString, actualValue);
      }

      [TestMethod]
      public void SetValue_DoesNotUpdateSource() {
         Behavior.SetValue(Context, ArbitraryString);

         SourceAccessorMock.Verify(
            x => x.SetValue(It.IsAny<IBehaviorContext>(), It.IsAny<string>()),
            Times.Never(),
            "SetValue should not update the source value."
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