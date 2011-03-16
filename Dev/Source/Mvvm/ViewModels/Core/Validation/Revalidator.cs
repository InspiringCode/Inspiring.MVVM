namespace Inspiring.Mvvm.ViewModels.Core.Validation {
   using System.Diagnostics.Contracts;

   internal sealed class Revalidator {
      private IViewModel _viewModel;

      public Revalidator(IViewModel viewModel) {
         Contract.Requires(viewModel != null);
         _viewModel = viewModel;
      }

      public void Revalidate() {
         _viewModel
            .Descriptor
            .Properties
            .ForEach(RevalidatePropertyValidations);

         RevalidateViewModelValidations();
      }

      public void RevalidateItems(IVMCollection collection) {
         var properties = collection.GetItemDescriptor().Properties;

         foreach (var p in properties) {
            var batch = new PropertyValidationBatch(p);

            foreach (var item in collection) {
               // call prop.Revalidate(batch);
            }
         }

         var viewModelBatch = new ViewModelValidationBatch();

         foreach (var item in collection) {
            // call viewModelBehavior.Revalidate(viewModelBatch);
         }
      }

      public void RevalidatePropertyValidations(IVMPropertyDescriptor property) {

      }

      public void RevalidateViewModelValidations() {

      }
   }
}
