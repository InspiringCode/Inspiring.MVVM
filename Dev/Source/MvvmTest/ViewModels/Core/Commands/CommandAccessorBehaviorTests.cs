namespace Inspiring.MvvmTest.ViewModels.Core.Commands {
   using System.Windows.Input;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class CommandAccessorBehaviorTests : TestBase {
      private CommandAccessorMock Behavior { get; set; }
      private PropertyStub<object> OwnerProperty { get; set; }
      private ViewModelStub OwnerVM { get; set; }

      [TestInitialize]
      public void Setup() {
         Behavior = new CommandAccessorMock();

         OwnerProperty = PropertyStub
            .WithBehaviors(Behavior)
            .Build();

         OwnerVM = ViewModelStub
            .WithProperties(OwnerProperty)
            .Build();
      }

      [TestMethod]
      public void GetValue_ReturnsCommandReturnedByCreateCommand() {
         Behavior.CommandToReturn = Mock<ICommand>();
         ICommand returnedCommand = Behavior.GetValue(OwnerVM.GetContext());
         Assert.AreEqual(Behavior.CommandToReturn, returnedCommand);
      }

      [TestMethod]
      public void GetValue_CallsCreateCommandWithCorrectArgs() {
         Behavior.GetValue(OwnerVM.GetContext());
         Assert.AreEqual(OwnerVM, Behavior.LastOwnerVM);
         Assert.AreEqual(OwnerProperty, Behavior.LastOwnerProperty);
      }

      [TestMethod]
      public void GetValue_ReturnsViewModelCommand() {
         Behavior.GetValue(OwnerVM.GetContext());
         Assert.IsInstanceOfType(Behavior.CommandReturnedByBaseMethod, typeof(ViewModelCommand));
      }

      [TestMethod]
      public void GetValue_SecondTime_ReturnsSameCommandInstance() {
         var first = Behavior.GetValue(OwnerVM.GetContext());
         var second = Behavior.GetValue(OwnerVM.GetContext());
         Assert.AreEqual(first, second);
      }

      private class CommandAccessorMock : CommandAccessorBehavior {
         public CommandAccessorMock() {
            CommandToReturn = Mock<ICommand>();
         }

         public IViewModel LastOwnerVM { get; set; }
         public IVMPropertyDescriptor LastOwnerProperty { get; set; }
         public ICommand CommandReturnedByBaseMethod { get; set; }

         public ICommand CommandToReturn { get; set; }

         protected override ICommand CreateCommand(IViewModel ownerVM, IVMPropertyDescriptor ownerProperty) {
            LastOwnerVM = ownerVM;
            LastOwnerProperty = ownerProperty;
            CommandReturnedByBaseMethod = base.CreateCommand(ownerVM, ownerProperty);
            return CommandToReturn;
         }
      }
   }
}