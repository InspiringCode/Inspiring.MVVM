namespace Inspiring.MvvmContribTest.Views {
   using System.Windows.Controls;
   using System.Windows.Input;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.Views;
   using System.Linq;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using System.Windows.Data;

   [TestClass]
   public class KeyGestureViewBinderExtensionTests {
      [TestMethod]
      public void KeyGesture_IsAddedToInputBindingsOfTargetObject() {
         var target = new Button();
         var view = new TestView();
         ViewBinder.BindVM(view, b => {
            b.Property(x => x.ViewModelCommand).To(target).KeyGesture(Key.A, ModifierKeys.Control);
         });

         Assert.AreEqual(1, target.InputBindings.Count);

         var inputBinding = (KeyBinding)target.InputBindings[0];
         Assert.AreEqual(Key.A, inputBinding.Key);

         var binding = BindingOperations.GetBinding(inputBinding, KeyBinding.CommandProperty);
         Assert.AreEqual("ViewModelCommand", binding.Path.Path);
      }

      public class TestView : UserControl, IBindableView<TestVM> {
         public TestVM Model { get; set; }
      }

      public sealed class TestVM : ViewModel<TestVMDescriptor> {
         public static readonly TestVMDescriptor Descriptor = VMDescriptorBuilder
            .For<TestVM>()
            .CreateDescriptor(c => {
               var vm = c.GetPropertyFactory();

               return new TestVMDescriptor {
                  ViewModelCommand = vm.Local.Property<ICommand>()
               };
            })
            .Build();

         public TestVM()
            : base(Descriptor) {
         }
      }

      public sealed class TestVMDescriptor : VMDescriptor {
         public VMProperty<ICommand> ViewModelCommand { get; set; }
      }
   }
}