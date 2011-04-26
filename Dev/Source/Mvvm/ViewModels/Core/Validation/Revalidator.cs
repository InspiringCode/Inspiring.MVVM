namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;

   internal sealed class Revalidator {
      private readonly IViewModel _viewModel;
      private readonly ValidationScope _scope;
      private readonly CollectionResultCache _cache;

      private Revalidator(IViewModel viewModel, ValidationScope scope)
         : this(viewModel, scope, new CollectionResultCache()) {
      }

      private Revalidator(IViewModel viewModel, ValidationScope scope, CollectionResultCache cache) {
         Contract.Requires(viewModel != null);
         Contract.Requires(cache != null);

         _viewModel = viewModel;
         _scope = scope;
         _cache = cache;
      }

      public static void Revalidate(IViewModel viewModel, ValidationScope scope) {
         new Revalidator(viewModel, scope).PerformAllValidations();
      }

      public static void RevalidatePropertyValidations(
         IViewModel viewModel,
         IVMPropertyDescriptor property,
         ValidationScope scope
      ) {
         new Revalidator(viewModel, scope).PerformHierarchicalPropertyValidation(property);
      }

      public static void RevalidateViewModelValidations(IViewModel viewModel) {
         new Revalidator(viewModel, ValidationScope.SelfOnly).PerformViewModelValidations();
      }

      public static void RevalidateItems(IEnumerable<IViewModel> items, ValidationScope scope) {
         var cache = new CollectionResultCache();

         foreach (IViewModel item in items) {
            new Revalidator(item, scope, cache).PerformAllValidations();
         }
      }

      private void PerformAllValidations() {
         var properties = _viewModel.Descriptor.Properties;
         properties.ForEach(PerformHierarchicalPropertyValidation);

         PerformViewModelValidations();
      }

      private void PerformHierarchicalPropertyValidation(IVMPropertyDescriptor property) {
         if (_scope != ValidationScope.SelfOnly) {
            PerformDescendantValidations(property);
         }

         PerformPropertyValidations(property);
      }

      private void PerformPropertyValidations(IVMPropertyDescriptor property) {
         IPropertyRevalidationBehavior behavior;

         bool hasRevalidationBehavior = property
            .Behaviors
            .TryGetBehavior(out behavior);

         if (hasRevalidationBehavior) {
            behavior.Revalidate(_viewModel.GetContext(), _cache);
         }
      }

      private void PerformViewModelValidations() {
         IPropertyRevalidationBehavior behavior;

         bool hasRevalidationBehavior = _viewModel
            .Descriptor
            .Behaviors
            .TryGetBehavior(out behavior);

         if (hasRevalidationBehavior) {
            behavior.Revalidate(_viewModel.GetContext(), _cache);
         }
      }

      private void PerformDescendantValidations(IVMPropertyDescriptor property) {
         property.Behaviors.RevalidateDescendantsNext(
            _viewModel.GetContext(),
            _scope
         );
      }
   }
}
