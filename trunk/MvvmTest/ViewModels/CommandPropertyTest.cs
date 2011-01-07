using System.Windows.Input;
using Inspiring.Mvvm.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inspiring.MvvmTest.ViewModels {
   [TestClass]
   public class CommandPropertyTest {
      private CommandVM _vm;

      [TestInitialize]
      public void Setup() {
         _vm = new CommandVM();
      }

      [TestMethod]
      public void GetValue_FirstAccess_CommandIsCreated() {
         ICommand vmCommand = _vm.GetValue(CommandVM.ClassDescriptor.ViewModelCommand);
         ICommand modelCommand = _vm.GetValue(CommandVM.ClassDescriptor.ModelCommand);

         Assert.IsNotNull(vmCommand);
         Assert.IsNotNull(modelCommand);
         Assert.AreEqual(0, _vm.ViewModelCanActionInvocationCount);
         Assert.AreEqual(0, _vm.Model.ModelActionInvocationCount);
         Assert.AreEqual(0, _vm.ViewModelActionInvocationCount);
         Assert.AreEqual(0, _vm.Model.ModelActionInvocationCount);
      }


      [TestMethod]
      public void ExecuteCommand_ViewModelMethodInvoked() {
         // Arrange
         ICommand command = _vm.GetValue(CommandVM.ClassDescriptor.ViewModelCommand);

         // Act
         command.Execute(null);

         // Assert
         Assert.AreEqual(1, _vm.ViewModelActionInvocationCount);
      }

      [TestMethod]
      public void ExecuteCommand_ModelMethodInvoked() {
         ICommand command = _vm.GetValue(CommandVM.ClassDescriptor.ModelCommand);

         command.Execute(null);

         Assert.AreEqual(1, _vm.Model.ModelActionInvocationCount);
      }


      [TestMethod]
      public void CanExecuteCommand_ViewModelMethodInvoked() {
         // Arrange
         ICommand command = _vm.GetValue(CommandVM.ClassDescriptor.ViewModelCommand);

         // Act
         bool result = command.CanExecute(null);

         // Assert
         Assert.IsTrue(result);
      }

      [TestMethod]
      public void CanExecuteCommand_ModelMethodInvoked() {
         ICommand command = _vm.GetValue(CommandVM.ClassDescriptor.ModelCommand);

         bool result = command.CanExecute(null);

         Assert.IsTrue(result);
         Assert.AreEqual(0, _vm.Model.ModelActionInvocationCount);
      }


      private class CommandVM : ViewModel<CommandVMDescriptor> {
         public static readonly CommandVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<CommandVMDescriptor>()
            .For<CommandVM>()
            .WithProperties((d, c) => {
               var v = c.GetPropertyBuilder();
               var m = c.GetPropertyBuilder(x => x.Model);

               d.ViewModelCommand = v.Command(vm => vm.ViewModelAction(), vm => vm.ViewModelCanAction());
               d.ModelCommand = m.Command(model => model.ModelActionMethod());
            })
            .Build();

         public CommandVM()
            : base(ClassDescriptor) {
            Model = new Model();
         }
         public Model Model { get; set; }

         public int ViewModelActionInvocationCount { get; set; }

         public void ViewModelAction() {
            ViewModelActionInvocationCount++;
         }


         public int ViewModelCanActionInvocationCount { get; set; }

         public bool ViewModelCanAction() {
            ViewModelCanActionInvocationCount++;
            return true;
         }
      }

      private class CommandVMDescriptor : VMDescriptor {
         public IVMProperty<ICommand> ViewModelCommand { get; set; }
         public IVMProperty<ICommand> ModelCommand { get; set; }
      }

      private class Model {
         public int ModelActionInvocationCount { get; set; }
         public void ModelActionMethod() {
            ModelActionInvocationCount++;
         }
      }
   }
}
