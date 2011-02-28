namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using System;
   using System.Windows.Input;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class CommandHandlerTests : TestBase {
      private static readonly BehaviorKey TestBehaviorKey = new BehaviorKey("TestBehavior");

      private EmployeeVM VM { get; set; }

      [TestInitialize]
      public void Setup() {
         RegisterCustomCommandTemplate();
         VM = new EmployeeVM();
      }

      [TestMethod]
      public void Execute_SetsCursorToWaitCursor() {
         Cursor cursorWhileExecuting = null;
         VM.PaySalaryCallback = () => cursorWhileExecuting = Mouse.OverrideCursor;

         VM.ExecuteCommand(x => x.PaySalary);

         Assert.AreEqual(Cursors.Wait, cursorWhileExecuting);
      }

      [TestMethod]
      public void Execute_CustomCommandHandler_IsInvoked() {
         VM.ExecuteCommand(x => x.PaySalary);
         Assert.IsTrue(VM.CustomExecuteWasInvoked);
      }

      [TestMethod]
      public void CanExecute_CustomCommandHandler_IsInvoked() {
         VM.CanExecuteCommand(x => x.PaySalary);
         Assert.IsTrue(VM.CustomCanExecuteWasInvoked);
      }

      [TestCleanup]
      public void Cleanup() {
         BehaviorChainTemplateRegistry.ResetToDefaults();
      }

      private static void RegisterCustomCommandTemplate() {
         BehaviorChainTemplateRegistry.RegisterTemplate(
            BehaviorChainTemplateKeys.CommandBehaviors,
            new BehaviorChainTemplate(CustomPropertyBehaviorFactory.Instance)
               .Append(BehaviorKeys.WaitCursor)
               .Append(TestBehaviorKey)
               .Append(BehaviorKeys.CommandExecutor, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(BehaviorKeys.SourceAccessor, DefaultBehaviorState.DisabledWithoutFactory)
         );
      }

      private class CustomPropertyBehaviorFactory : CommandBehaviorFactory {
         public static readonly CommandBehaviorFactory Instance = new CustomPropertyBehaviorFactory();

         public override IBehavior Create<TVM, TValue>(BehaviorKey key) {
            if (key == TestBehaviorKey) {
               return new CustomCommandHandler();
            }

            return base.Create<TVM, TValue>(key);
         }
      }

      private class CustomCommandHandler :
         Behavior,
         ICommandCanExecuteBehavior,
         ICommandExecuteBehavior {

         public bool CanExecute(IBehaviorContext context, object parameter) {
            var vm = (EmployeeVM)context.VM;
            vm.CustomCanExecuteWasInvoked = true;

            return this.CanExecuteNext(context, parameter);
         }

         public void Execute(IBehaviorContext context, object parameter) {
            var vm = (EmployeeVM)context.VM;
            vm.CustomExecuteWasInvoked = true;

            this.ExecuteNext(context, parameter);
         }
      }

      private sealed class EmployeeVM : ViewModel<EmployeeVMDescriptor> {
         public EmployeeVM()
            : base(CreateDescriptor()) {
            PaySalaryCallback = () => { };
         }

         public Action PaySalaryCallback { get; set; }

         public bool CustomExecuteWasInvoked { get; set; }

         public bool CustomCanExecuteWasInvoked { get; set; }

         private static EmployeeVMDescriptor CreateDescriptor() {
            // The only reason that we do not use a static Descriptor field is 
            // that we can change the property behaviors between unit tests!
            return VMDescriptorBuilder
               .OfType<EmployeeVMDescriptor>()
               .For<EmployeeVM>()
               .WithProperties((d, b) => {
                  var v = b.GetPropertyBuilder();
                  d.PaySalary = v.Command(x => x.PaySalaryCallback());
               })
               .Build();
         }
      }

      private sealed class EmployeeVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<ICommand> PaySalary { get; set; }
      }
   }
}