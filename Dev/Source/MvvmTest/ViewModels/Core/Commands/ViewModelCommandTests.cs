namespace Inspiring.MvvmTest.ViewModels.Core.Commands {
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

   [TestClass]
   public class ViewModelCommandTests : TestBase {
      private ViewModelCommand Command { get; set; }
      private HandlerMock Handler { get; set; }
      
      private PropertyStub<object> OwnerProperty { get; set; }
      private ViewModelStub OwnerVM { get; set; }

      [TestInitialize]
      public void Setup() {
         Handler = new HandlerMock();

         OwnerProperty = PropertyStub
            .WithBehaviors(Handler)
            .Build();

         OwnerVM = ViewModelStub
            .WithProperties(OwnerProperty)
            .Build();

         Command = new ViewModelCommand(OwnerVM, OwnerProperty);
      }

      [TestMethod]
      public void Execute_InvokesExecuteHandlerOfCommandPropertyBehaviorChain() {
         var expectedParameter = new Object();
         var expectedContext = OwnerVM.GetContext();
         
         Command.Execute(expectedParameter);

         Assert.IsNotNull(Handler.LastExecuteInvocation);
         Assert.AreEqual(expectedContext, Handler.LastExecuteInvocation.Context);
         Assert.AreEqual(expectedParameter, Handler.LastExecuteInvocation.Parameter);
      }

      [TestMethod]
      public void CanExecute_InvokesCanExecuteHandlerOfCommandPropertyBehaviorChain() {
         var expectedParameter = new Object();
         var expectedContext = OwnerVM.GetContext();

         Command.CanExecute(expectedParameter);

         Assert.IsNotNull(Handler.LastCanExecuteInvocation);
         Assert.AreEqual(expectedContext, Handler.LastCanExecuteInvocation.Context);
         Assert.AreEqual(expectedParameter, Handler.LastCanExecuteInvocation.Parameter);
      }

      [TestMethod]
      public void CanExecute_ReturnsResultOfCanExecuteBehavior() {
         Handler.ResultToReturn = true;
         Assert.AreEqual(Handler.ResultToReturn, Command.CanExecute(null));
         Handler.ResultToReturn = false;
         Assert.AreEqual(Handler.ResultToReturn, Command.CanExecute(null));        
      }

      private class HandlerMock :
         Behavior,
         ICommandCanExecuteBehavior,
         ICommandExecuteBehavior {

         public bool ResultToReturn { get; set; }

         public HandlerInvocation LastExecuteInvocation { get; set; }
         public HandlerInvocation LastCanExecuteInvocation { get; set; }

         public bool CanExecute(IBehaviorContext context, object parameter) {
            LastCanExecuteInvocation = new HandlerInvocation { Context = context, Parameter = parameter };
            return ResultToReturn;
         }

         public void Execute(IBehaviorContext context, object parameter) {
            LastExecuteInvocation = new HandlerInvocation { Context = context, Parameter = parameter };
         }
      }

      private class HandlerInvocation {
         public IBehaviorContext Context { get; set; }
         public object Parameter { get; set; }
      }
   }
}