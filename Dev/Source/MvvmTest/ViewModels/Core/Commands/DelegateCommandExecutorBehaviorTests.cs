namespace Inspiring.MvvmTest.ViewModels.Core.Commands {
   using System;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class DelegateCommandExecutorBehaviorTests : TestBase {
      private DelegateCommandExecutorBehavior<SourceObject> Behavior { get; set; }
      private NextBehavior Next { get; set; }

      private DelegateInvocation LastExecuteInvocation { get; set; }
      private DelegateInvocation LastCanExecuteInvocation { get; set; }

      private IBehaviorContext Context { get; set; }

      [TestMethod]
      public void Execute_CallsActionWithSourceObject() {
         Setup();
         Next.SourceObject = new SourceObject();
         Behavior.Execute(Context, null);
         Assert.AreEqual(Next.SourceObject, LastExecuteInvocation.SourceObject);
      }

      [TestMethod]
      public void CanExecute_CallsCanExecutePredicateWithSourceObject() {
         Setup(withCanExecutePredicate: true);
         Next.SourceObject = new SourceObject();
         Behavior.CanExecute(Context, null);
         Assert.AreEqual(Next.SourceObject, LastCanExecuteInvocation.SourceObject);
      }

      [TestMethod]
      public void CanExecute_ReturnsTrueWithoutCanExecuteFunction() {
         Setup(withCanExecutePredicate: false);
         Assert.IsTrue(Behavior.CanExecute(Context, null));
      }

      [TestMethod]
      public void CanExceute_ReturnsCanExecuteFunctionResult() {
         Setup(withCanExecutePredicate: true, canExecuteResult: false);
         Assert.IsFalse(Behavior.CanExecute(Context, null));
         Setup(withCanExecutePredicate: true, canExecuteResult: true);
         Assert.IsTrue(Behavior.CanExecute(Context, null));
      }

      private void Setup(bool withCanExecutePredicate = false, bool canExecuteResult = false) {
         Func<SourceObject, bool> canExecuteFunction = null;

         if (withCanExecutePredicate) {
            canExecuteFunction = (so) => {
               LastCanExecuteInvocation = new DelegateInvocation { SourceObject = so };
               return canExecuteResult;
            };
         }

         Action<SourceObject> executeAction = (so) => {
            LastExecuteInvocation = new DelegateInvocation { SourceObject = so };
         };

         Behavior = new DelegateCommandExecutorBehavior<SourceObject>(executeAction, canExecuteFunction);
         Next = new NextBehavior();

         Context = PropertyStub
            .WithBehaviors(Behavior, Next)
            .GetContext();
      }

      private class DelegateInvocation {
         public SourceObject SourceObject { get; set; }
      }

      private class NextBehavior :
         Behavior,
         IValueAccessorBehavior<SourceObject> {

         public SourceObject SourceObject { get; set; }

         public SourceObject GetValue(IBehaviorContext context) {
            return SourceObject;
         }

         public void SetValue(IBehaviorContext context, SourceObject value) {
            SourceObject = value;
         }
      }

      private class SourceObject {
      }
   }
}