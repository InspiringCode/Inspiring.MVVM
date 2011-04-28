namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Collections.Generic;

   internal sealed class Revalidator {
      public static void Revalidate(IViewModel viewModel, ValidationScope scope) {
         foreach (var property in viewModel.Descriptor.Properties) {
            RevalidatePropertyValidations(viewModel, property, scope);
         }

         RevalidateViewModelValidations(viewModel);
      }

      public static void RevalidatePropertyValidations(
         IViewModel viewModel,
         IVMPropertyDescriptor property,
         ValidationScope scope
      ) {
         PerformDescendantValidations(viewModel, property, scope);

         var controller = new ValidationController();
         controller.RequestPropertyRevalidation(viewModel, property);
         controller.ProcessPendingValidations();
      }

      public static void RevalidateViewModelValidations(IViewModel viewModel) {
         var controller = new ValidationController();
         controller.RequestViewModelRevalidation(viewModel);
         controller.ProcessPendingValidations();
      }

      public static void RevalidateItems(IEnumerable<IViewModel> items, ValidationScope scope) {
         if (scope != ValidationScope.Self) {
            foreach (var item in items) {
               foreach (var property in item.Descriptor.Properties) {
                  PerformDescendantValidations(item, property, scope);
               }
            }
         }

         var controller = new ValidationController();

         foreach (var item in items) {
            foreach (var property in item.Descriptor.Properties) {
               controller.RequestPropertyRevalidation(item, property);
            }
         }

         foreach (var item in items) {
            controller.RequestViewModelRevalidation(item);
         }

         controller.ProcessPendingValidations();
      }

      private static void PerformDescendantValidations(
         IViewModel target,
         IVMPropertyDescriptor property,
         ValidationScope scope
      ) {
         if (scope != ValidationScope.Self) {
            property.Behaviors.RevalidateDescendantsNext(
               target.GetContext(),
               scope
            );
         }
      }
   }
}
