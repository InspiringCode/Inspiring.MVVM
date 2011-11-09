namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels.Tracing;

   internal sealed class RefreshAction : DependencyAction {
      private readonly QualifiedProperties _target;
      private readonly bool _executeRefreshDependencies;

      public RefreshAction(
         QualifiedProperties target,
         bool executeRefreshDependencies
      ) {
         _target = target;
         _executeRefreshDependencies = executeRefreshDependencies;
      }

      internal QualifiedProperties Target { get { return _target; } }

      //internal IList<IPropertySelector> TargetProperties {
      //   get { return _targetProperties; }
      //}

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

         _target.ForeachLoadedDescendant(ownerVM, (vm, props) => {
            if (props.Any()) {
               foreach (var prop in props) {
                  bool wouldRefreshChangeSource = 
                     vm == args.ChangedVM &&
                     prop == args.ChangedProperty;

                  if (!wouldRefreshChangeSource) {
                     vm.Kernel.RefreshInternal(prop, new RefreshOptions(_executeRefreshDependencies));
                  }
               }
            } else {
               bool wouldRefreshChangeSource = vm == args.ChangedVM;

               if (!wouldRefreshChangeSource) {
                  vm.Kernel.RefreshInternal(_executeRefreshDependencies);
               }
            }
         });

         //_target.Refresh(ownerVM, _executeRefreshDependencies);

         //ownerVM.Kernel.RefreshInternal(_target, _executeRefreshDependencies);

         //if (TargetPath.IsEmpty) {
         //   if (_targetProperties.Count > 0) {
         //      RefreshProperties(ownerVM);
         //   } else {
         //      ownerVM.Kernel.RefreshInternal(_executeRefreshDependencies);
         //   }
         //} else {
         //   var viewModels = TargetPath.GetDescendants(ownerVM);

         //   foreach (var viewModel in viewModels) {
         //      if (_targetProperties.Count > 0) {
         //         RefreshProperties(viewModel);
         //      } else {
         //         viewModel.Kernel.RefreshInternal(_executeRefreshDependencies);
         //      }
         //   }
         //}

         RefreshTrace.EndLastRefresh();
      }

      public override string ToString() {
         return String.Format("refresh '{0}'", _target);
      }

      //private void RefreshProperties(IViewModel ownerVM) {
      //   foreach (var propertySelector in _targetProperties) {
      //      var property = propertySelector.GetProperty(ownerVM.Descriptor);
      //      ownerVM.Kernel.RefreshInternal(property, _executeRefreshDependencies);
      //   }
      //}
   }
}
