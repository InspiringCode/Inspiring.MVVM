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
         ICommand vmCommand = _vm.GetValue(CommandVM.Descriptor.ViewModelCommand);
         ICommand modelCommand = _vm.GetValue(CommandVM.Descriptor.ModelCommand);

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
         ICommand command = _vm.GetValue(CommandVM.Descriptor.ViewModelCommand);

         // Act
         command.Execute(null);

         // Assert
         Assert.AreEqual(1, _vm.ViewModelActionInvocationCount);
      }

      [TestMethod]
      public void ExecuteCommand_ModelMethodInvoked() {
         ICommand command = _vm.GetValue(CommandVM.Descriptor.ModelCommand);

         command.Execute(null);

         Assert.AreEqual(1, _vm.Model.ModelActionInvocationCount);
      }


      [TestMethod]
      public void CanExecuteCommand_ViewModelMethodInvoked() {
         // Arrange
         ICommand command = _vm.GetValue(CommandVM.Descriptor.ViewModelCommand);

         // Act
         bool result = command.CanExecute(null);

         // Assert
         Assert.IsTrue(result);
      }

      [TestMethod]
      public void CanExecuteCommand_ModelMethodInvoked() {
         ICommand command = _vm.GetValue(CommandVM.Descriptor.ModelCommand);

         bool result = command.CanExecute(null);

         Assert.IsTrue(result);
         Assert.AreEqual(0, _vm.Model.ModelActionInvocationCount);
      }


      private class CommandVM : ViewModel<CommandVMDescriptor> {
         public static readonly CommandVMDescriptor Descriptor = VMDescriptorBuilder
            .For<CommandVM>()
            .CreateDescriptor(c => {
               var v = c.GetPropertyBuilder();
               var m = c.GetPropertyBuilder(x => x.Model);
               return new CommandVMDescriptor {
                  ViewModelCommand = v.Command(vm => vm.ViewModelAction(), vm => vm.ViewModelCanAction()),
                  ModelCommand = m.Command(model => model.ModelActionMethod())
               };
            })
            .Build();

         public CommandVM()
            : base(Descriptor) {
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
         public VMProperty<ICommand> ViewModelCommand { get; set; }
         public VMProperty<ICommand> ModelCommand { get; set; }
      }

      private class Model {
         public int ModelActionInvocationCount { get; set; }
         public void ModelActionMethod() {
            ModelActionInvocationCount++;
         }
      }
   }
}
