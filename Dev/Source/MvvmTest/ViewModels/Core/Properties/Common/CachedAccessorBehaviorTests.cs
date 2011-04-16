namespace Inspiring.MvvmTest.ViewModels.Core.Properties.Common {
   using System;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class CachedAccessorBehaviorTests {
      [TestMethod]
      public void GetValue_CallingGetValueWithinValueInitializer_DoesNotCallValueInitializerAgain() {
         PropertyStub<object> property = null;

         Func<IBehaviorContext, object> provideValueFunction = ctx => {
            return new Object();
         };

         var behavior = new TestBehavior(provideValueFunction);
         var valueInitializerWasAlreadyInvoked = false;

         Action<IBehaviorContext> initializeValueAction = ctx => {
            Assert.IsFalse(valueInitializerWasAlreadyInvoked);
            valueInitializerWasAlreadyInvoked = true;
            property.Behaviors.GetValueNext<object>(ctx);
         };

         property = PropertyStub
            .WithBehaviors(behavior, new TestValueInitializerBehavior(initializeValueAction))
            .Build();

         var context = ViewModelStub
            .WithProperties(property)
            .BuildContext();

         behavior.GetValue(context);

         Assert.IsTrue(valueInitializerWasAlreadyInvoked);
      }

      [TestMethod]
      public void GetValue_CallingGetValueWithinProvideValue_ThrowsException() {
         PropertyStub<object> property = null;
         var provideValueWasAlreadyCalled = false;

         Func<IBehaviorContext, object> provideValueFunction = ctx => {
            Assert.IsFalse(provideValueWasAlreadyCalled);
            provideValueWasAlreadyCalled = true;
            property.Behaviors.GetValueNext<object>(ctx);
            return new Object();
         };

         var behavior = new TestBehavior(provideValueFunction);

         property = PropertyStub
            .WithBehaviors(behavior)
            .Build();

         var context = ViewModelStub
            .WithProperties(property)
            .BuildContext();

         AssertHelper.Throws<InvalidOperationException>(() =>
            behavior.GetValue(context)
         ).WithMessage(EViewModels.ValueAccessedWithinProvideValue);

         Assert.IsTrue(provideValueWasAlreadyCalled);
      }

      private class TestBehavior : CachedAccessorBehavior<object> {
         Func<IBehaviorContext, object> _valueProvider;

         public TestBehavior(Func<IBehaviorContext, object> provideValueFunction) {
            _valueProvider = provideValueFunction;
         }

         protected override object ProvideValue(IBehaviorContext context) {
            return _valueProvider(context);
         }
      }

      private class TestValueInitializerBehavior : Behavior, IValueInitializerBehavior {
         private Action<IBehaviorContext> _initializeValueAction;

         public TestValueInitializerBehavior(Action<IBehaviorContext> initializeValueAction) {
            _initializeValueAction = initializeValueAction;
         }

         public void InitializeValue(IBehaviorContext context) {
            _initializeValueAction(context);
         }
      }

   }
}