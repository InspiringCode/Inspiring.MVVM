namespace Inspiring.Mvvm.ViewModels {
   using System;
   using Inspiring.Mvvm.Testability;

   public static class ViewModelAssert {
      public static void IsLoaded<TDescriptor>(
         IViewModel<TDescriptor> vm,
         Func<TDescriptor, IVMPropertyDescriptor> propertySelector
      ) where TDescriptor : IVMDescriptor {
         IsLoaded(vm, propertySelector, expectedLoadedResult: true);
      }

      public static void IsNotLoaded<TDescriptor>(
         IViewModel<TDescriptor> vm,
         Func<TDescriptor, IVMPropertyDescriptor> propertySelector
      ) where TDescriptor : IVMDescriptor {
         IsLoaded(vm, propertySelector, expectedLoadedResult: false);
      }

      private static void IsLoaded<TDescriptor>(
         IViewModel<TDescriptor> vm,
         Func<TDescriptor, IVMPropertyDescriptor> propertySelector,
         bool expectedLoadedResult
      ) where TDescriptor : IVMDescriptor {
         IVMPropertyDescriptor property = propertySelector((TDescriptor)vm.Descriptor);

         if (vm.Kernel.IsLoaded(property) != expectedLoadedResult) {
            string message = String.Format(
               "Expected property {0} of {1} {2}to be loaded.",
               property.PropertyName,
               vm,
               expectedLoadedResult ?
                  String.Empty :
                  "not "
            );

            TestFrameworkAdapter.Current.Fail(message);
         }
      }

   }
}
