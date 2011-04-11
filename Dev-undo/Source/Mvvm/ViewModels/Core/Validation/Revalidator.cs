namespace Inspiring.Mvvm.ViewModels.Core.Validation {
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;

   internal sealed class Revalidator {
      private readonly IViewModel _viewModel;
      private readonly CollectionResultCache _cache;

      private Revalidator(IViewModel viewModel)
         : this(viewModel, new CollectionResultCache()) {
      }

      private Revalidator(IViewModel viewModel, CollectionResultCache cache) {
         Contract.Requires(viewModel != null);
         Contract.Requires(cache != null);

         _viewModel = viewModel;
         _cache = cache;
      }

      public static void Revalidate(IViewModel viewModel) {
         new Revalidator(viewModel).PerformAllValidations();
      }

      public static void RevalidatePropertyValidations(
         IViewModel viewModel,
         IVMPropertyDescriptor property
      ) {
         new Revalidator(viewModel).PerformPropertyValidations(property);
      }

      public static void RevalidateViewModelValidations(IViewModel viewModel) {
         new Revalidator(viewModel).PerformViewModelValidations();
      }

      public static void RevalidateItems(IEnumerable<IViewModel> items) {
         var cache = new CollectionResultCache();

         foreach (IViewModel item in items) {
            new Revalidator(item, cache).PerformAllValidations();
         }
      }

      private void PerformAllValidations() {
         var properties = _viewModel.Descriptor.Properties;
         properties.ForEach(PerformPropertyValidations);

         PerformViewModelValidations();
      }

      private void PerformViewModelValidations() {
         IRevalidationBehavior behavior;

         bool hasRevalidationBehavior = _viewModel
            .Descriptor
            .Behaviors
            .TryGetBehavior(out behavior);

         if (hasRevalidationBehavior) {
            behavior.Revalidate(_viewModel.GetContext(), _cache);
         }
      }

      private void PerformPropertyValidations(IVMPropertyDescriptor property) {
         IRevalidationBehavior behavior;

         bool hasRevalidationBehavior = property
            .Behaviors
            .TryGetBehavior(out behavior);

         if (hasRevalidationBehavior) {
            behavior.Revalidate(_viewModel.GetContext(), _cache);
         }
      }
   }
}
