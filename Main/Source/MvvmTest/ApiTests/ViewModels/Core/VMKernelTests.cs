namespace Inspiring.MvvmTest.ApiTests.ViewModels.Core {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class VMKernelTests : TestBase {

      /// <summary>
      ///   ParentVM has a child VM. When a certain property of the child VM changes the childVM is replaced with a new one.
      ///   When the change notification is forwarded from the child to the parent, the parents declarative dependency change handler
      ///   replaces the child VM. This replace removes the parent from the old child, i.e the parent collection changes during iterating
      ///   (InvalidOperationException with message 'Collection was modified; enumeration operation may not execute.' was thrown).
      /// </summary>
      [TestMethod]
      public void ForwardChangeNotificationToParents_WhenParentsChangingDuringNotification_DoesntThrowException() {
         var parent = new ParentVM();
         var child = new ChildVM();

         parent.SetValue(x => x.Child, child);

         child.TriggerChange();
      }

      private sealed class ParentVM : ViewModel<ParentVMDescriptor> {
         public static readonly ParentVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<ParentVMDescriptor>()
            .For<ParentVM>()
            .WithProperties((d, c) => {
               var vm = c.GetPropertyBuilder();

               d.Child = vm.VM.Of<ChildVM>();
            })
            .WithDependencies(d => {
               d.OnChangeOf
                  .Descendant(x => x.Child)
                  .Properties(x => x.ChangeTrigger)
                  .Execute((vm, args) => {
                     vm.Child = new ChildVM();
                  });
            })
            .Build();

         public ParentVM()
            : base(ClassDescriptor) {

         }

         private ChildVM Child {
            set { SetValue(Descriptor.Child, value); }
         }
      }

      private sealed class ParentVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<ChildVM> Child { get; set; }
      }

      private sealed class ChildVM : ViewModel<ChildVMDescriptor> {
         public static readonly ChildVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<ChildVMDescriptor>()
            .For<ChildVM>()
            .WithProperties((d, c) => {
               var vm = c.GetPropertyBuilder();

               d.ChangeTrigger = vm.Property.Of<bool>();
            })
            .Build();

         public ChildVM()
            : base(ClassDescriptor) {

         }

         private bool ChangeTrigger {
            get { return GetValue(Descriptor.ChangeTrigger); }
            set { SetValue(Descriptor.ChangeTrigger, value); }
         }

         public void TriggerChange() {
            ChangeTrigger = !ChangeTrigger;
         }
      }

      private sealed class ChildVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<bool> ChangeTrigger { get; set; }
      }
   }
}
