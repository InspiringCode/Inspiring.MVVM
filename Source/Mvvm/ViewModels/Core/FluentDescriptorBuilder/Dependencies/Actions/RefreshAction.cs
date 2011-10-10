namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels.Tracing;

   internal sealed class RefreshAction : DependencyAction {
      private readonly PathDefinition _targetPath;
      private readonly IList<IPropertySelector> _targetProperties;
      private readonly bool _executeRefreshDependencies;

      public RefreshAction(
         PathDefinition targetPath,
         IList<IPropertySelector> targetProperties,
         bool executeRefreshDependencies
      ) {
         _targetPath = targetPath;
         _targetProperties = targetProperties;
         _executeRefreshDependencies = executeRefreshDependencies;
      }

      internal PathDefinition TargetPath { get { return _targetPath; } }

      internal IList<IPropertySelector> TargetProperties {
         get { return _targetProperties; }
      }

      public override void Execute(
         IViewModel ownerVM,
         ChangeArgs args,
         DeclarativeDependency dependency
      ) {
         RefreshReason reason = args.Reason as RefreshReason;

         if (reason != null && !reason.ExecuteRefreshDependencies) {
            return;
         }

         RefreshTrace.BeginRefresh(dependency);

         if (TargetPath.IsEmpty) {
            if (_targetProperties.Count > 0) {
               RefreshProperties(ownerVM);
            } else {
               ownerVM.Kernel.RefreshInternal(_executeRefreshDependencies);
            }
         } else {
            var viewModels = TargetPath.GetDescendants(ownerVM);

            foreach (var viewModel in viewModels) {
               if (_targetProperties.Count > 0) {
                  RefreshProperties(viewModel);
               } else {
                  viewModel.Kernel.RefreshInternal(_executeRefreshDependencies);
               }
            }
         }

         RefreshTrace.EndLastRefresh();
      }

      public override string ToString() {
         return String.Format(
            "refresh '{0}.[{1}]'",
            TargetPath,
            String.Join(" AND ", TargetProperties.Select(x => x.PropertyName))
         );
      }

      private void RefreshProperties(IViewModel ownerVM) {
         foreach (var propertySelector in _targetProperties) {
            var property = propertySelector.GetProperty(ownerVM.Descriptor);
            ownerVM.Kernel.RefreshInternal(property, _executeRefreshDependencies);
         }
      }
   }
}
