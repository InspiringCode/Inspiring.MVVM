namespace Inspiring.MvvmTest.ApiTests.ViewModels.DeclarativeDeendencies {
   using System;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public sealed class DependencyTests {

      [TestMethod]
      public void DependencyOnDescendentPropertiesWithRefreshAction_WhenDescendentIsNull_DoesntThrowException() {

         var parentVM = new ParentVM(
             b => b
              .OnChangeOf
              .Properties(x => x.ChangeTrigger)
              .Refresh
              .Descendant(x => x.ChildVM)
              .Properties(x => x.Name)
         );

         parentVM.ChangeTrigger = true;
      }

      private class ParentVM : TestViewModel<ParentVMDescriptor> {

         public ParentVM(
            Action<IVMDependencyBuilder<ParentVM, ParentVMDescriptor>> dependencyConfigurationAction
         )
            : base(CreateDescriptor(dependencyConfigurationAction)) {

            SetValue(Descriptor.ChildVM, null);
         }

         private static ParentVMDescriptor CreateDescriptor(
               Action<IVMDependencyBuilder<ParentVM, ParentVMDescriptor>> dependencyConfigurationAction
         ) {
            return VMDescriptorBuilder
                  .OfType<ParentVMDescriptor>()
                  .For<ParentVM>()
                  .WithProperties((d, c) => {
                     var v = c.GetPropertyBuilder();

                     d.ChangeTrigger = v.Property.Of<bool>();
                     d.ChildVM = v.Property.Of<ChildVM>();
                  })
                  .WithDependencies(
                     dependencyConfigurationAction
                  )
                  .Build();
         }

         internal bool ChangeTrigger {
            set { SetValue(Descriptor.ChangeTrigger, value); }
         }
      }

      private class ParentVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<bool> ChangeTrigger { get; set; }
         public IVMPropertyDescriptor<ChildVM> ChildVM { get; set; }
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
