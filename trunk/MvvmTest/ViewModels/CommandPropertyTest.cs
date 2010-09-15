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
      public void GetValue() {
         ICommand command = _vm.GetValue(CommandVM.Descriptor.ViewModelCommand);
         Assert.IsNotNull(command);

         command = _vm.GetValue(CommandVM.Descriptor.ModelCommand);
         Assert.IsNotNull(command);
      }

      [TestMethod]
      public void Execute() {
         ICommand command = _vm.GetValue(CommandVM.Descriptor.ViewModelCommand);
         Assert.AreEqual(0, _vm.ViewModelActionInvocationCount);
         command.Execute(null);
         Assert.AreEqual(1, _vm.ViewModelActionInvocationCount);

         command = _vm.GetValue(CommandVM.Descriptor.ModelCommand);
         Assert.AreEqual(0, _vm.Model.ModelActionInvocationCount);
         command.Execute(null);
         Assert.AreEqual(1, _vm.Model.ModelActionInvocationCount);
      }

      [TestMethod]
      public void CanExecute() {
         ICommand command = _vm.GetValue(CommandVM.Descriptor.ViewModelCommand);
         Assert.AreEqual(0, _vm.ViewModelCanActionInvocationCount);
         Assert.IsTrue(command.CanExecute(null));
         Assert.AreEqual(1, _vm.ViewModelCanActionInvocationCount);

         command = _vm.GetValue(CommandVM.Descriptor.ModelCommand);
         Assert.AreEqual(0, _vm.Model.ModelActionInvocationCount);
         Assert.IsTrue(command.CanExecute(null));
         Assert.AreEqual(0, _vm.Model.ModelActionInvocationCount);
      }



      private class CommandVM : ViewModel<CommandVMDescriptor> {
         public static readonly CommandVMDescriptor Descriptor = VMDescriptorBuilder
            .For<CommandVM>()
            .CreateDescriptor(c => {
               var v = c.GetPropertyFactory();
               var m = c.GetPropertyFactory(x => x.Model);
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
