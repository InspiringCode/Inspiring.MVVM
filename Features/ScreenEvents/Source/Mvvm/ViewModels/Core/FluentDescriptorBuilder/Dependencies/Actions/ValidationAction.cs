namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Collections.Generic;

   internal sealed class ValidationAction : DependencyAction {
      private readonly QualifiedProperties _target;
      
      public ValidationAction(
         QualifiedProperties target
      ) {
         _target = target;
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
         _target.Revalidate(ownerVM);
         //if (TargetPath.IsEmpty) {
         //   RevalidateProperties(ownerVM);
         //} else {
         //   var viewModels = TargetPath.GetDescendants(ownerVM);

         //   foreach (var viewModel in viewModels) {
         //      if (_targetProperties.Count > 0) {
         //         RevalidateProperties(viewModel);
         //      } else {
         //         viewModel.Kernel.Revalidate(ValidationScope.SelfAndLoadedDescendants);
         //      }
         //   }
         //}
      }

      //private void RevalidateProperties(IViewModel ownerVM) {
      //   foreach (var propertySelector in _targetProperties) {
      //      var property = propertySelector.GetProperty(ownerVM.Descriptor);
      //      ownerVM.Kernel.Revalidate(property);
      //   }
      //}
   }
}
