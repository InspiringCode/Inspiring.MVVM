namespace Inspiring.MvvmTest.ApiTests.ViewModels.DeclarativeDeendencies {
   using System;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class LazyLoadTests {
      [TestMethod]
      public void DependencyOnDescendant_WhenDescendantIsNotLoaded_DoesNotTriggerLoad() {
         var parentVM = new ParentVM(
             b => b
              .OnChangeOf
              .Descendant(x => x.ChildVM)
              .Execute((vm, args) => { })
         );

         parentVM.GetValue(x => x.ChangeTrigger).SetValue(x => x.Name, "ChangeTrigger");

         Assert.IsFalse(parentVM.IsLoaded(x => x.ChildVM));
      }

      private class ParentVM : TestViewModel<ParentVMDescriptor> {

         public ParentVM(
            Action<IVMDependencyBuilder<ParentVM, ParentVMDescriptor>> dependencyConfigurationAction
         )
            : base(CreateDescriptor(dependencyConfigurationAction)) {

         }

         private static ParentVMDescriptor CreateDescriptor(
               Action<IVMDependencyBuilder<ParentVM, ParentVMDescriptor>> dependencyConfigurationAction
         ) {
            return VMDescriptorBuilder
                  .OfType<ParentVMDescriptor>()
                  .For<ParentVM>()
                  .WithProperties((d, c) => {
                     var v = c.GetPropertyBuilder();

                     d.Name = v.Property.Of<string>();
                     d.ChildVM = v.VM.DelegatesTo(x => {
                        return new ChildVM();
                     });
                     d.ChangeTrigger = v.VM.DelegatesTo(x => {
                        return new ChildVM();
                     });
                  })
                  .WithDependencies(
                     dependencyConfigurationAction
                  )
                  .Build();
         }
      }

      private class ParentVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> Name { get; set; }
         public IVMPropertyDescriptor<ChildVM> ChildVM { get; set; }
         public IVMPropertyDescriptor<ChildVM> ChangeTrigger { get; set; }
      }

      private class ChildVM : TestViewModel<ChildVMDescriptor> {
         public static readonly ChildVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<ChildVMDescriptor>()
            .For<ChildVM>()
            .WithProperties((d, b) => {
               var vm = b.GetPropertyBuilder();

               d.Name = vm.Property.Of<string>();
            })
            .Build();

         public ChildVM()
            : base(ClassDescriptor) {

         }
      }

      private class ChildVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> Name { get; set; }
      }
   }
}