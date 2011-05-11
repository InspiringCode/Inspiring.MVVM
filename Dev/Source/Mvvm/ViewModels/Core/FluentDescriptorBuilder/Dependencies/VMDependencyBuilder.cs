namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Collections.Generic;

   internal sealed class VMDependencyBuilder<TVM, TDescriptor>
      : IVMDependencyBuilder<TVM, TDescriptor>
      where TVM : IViewModel
      where TDescriptor : VMDescriptor {

      private BehaviorChainConfiguration _viewModelConfiguration;
      private List<DependencyBuilderOperation> _operations = new List<DependencyBuilderOperation>();

      public VMDependencyBuilder(BehaviorChainConfiguration viewModelConfiguration) {
         _viewModelConfiguration = viewModelConfiguration;
      }

      public IDependencySelfSourceBuilder<TVM, TVM, TDescriptor, TDescriptor> OnChangeOf {
         get {
            var operation = new DependencyBuilderOperation(_viewModelConfiguration);
            _operations.Add(operation);
            return new DependencySourceBuilder<TVM, TVM, TDescriptor, TDescriptor>(operation);
         }
      }

      public void Execute() {
         foreach (var operation in _operations) {
            operation.Perform();
         }
      }
   }
}