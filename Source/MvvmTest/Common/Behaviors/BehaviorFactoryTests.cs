namespace Inspiring.MvvmTest.Common.Behaviors {
   using System;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class BehaviorFactoryTests : TestBase {
      private BehaviorFactory Factory { get; set; }
      private BehaviorKey TestKey { get; set; }

      [TestInitialize]
      public void Setup() {
         Factory = new BehaviorFactory();
         TestKey = new BehaviorKey("Test key");
      }

      [TestMethod]
      public void Create_ForBehaviorRegisteredByType_CreatesAndReturnsNewInstance() {
         Factory.RegisterBehavior<TestBehavior>(TestKey);
         Assert.IsInstanceOfType(Factory.Create(TestKey), typeof(TestBehavior));
      }

      [TestMethod]
      public void Create_ForBehaviorRegisteredWithCreationFunction_ReturnsInstanceReturnedByFunction() {
         var expectedBehavior = Mock<IBehavior>();
         Factory.RegisterBehavior(TestKey, () => expectedBehavior);
         Assert.AreEqual(expectedBehavior, Factory.Create(TestKey));
      }

      [TestMethod]
      public void Create_ForUnregisteredKey_ThrowsException() {
         var expectedMessage = ECommon
            .BehaviorForKeyCannotBeCreated
            .FormatWith(TestKey);

         AssertHelper.Throws<ArgumentException>(() =>
            Factory.Create(TestKey)
         ).WithMessage(expectedMessage);
      }

      private class TestBehavior : Behavior { }
   }
}