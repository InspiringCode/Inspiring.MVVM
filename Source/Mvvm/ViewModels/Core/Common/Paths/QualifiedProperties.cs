namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Linq;

   internal class QualifiedProperties {
      public QualifiedProperties() {
         Path = PathDefinition.Empty;
         Properties = new IPropertySelector[0];
      }

      public PathDefinition Path { get; set; }
      public IPropertySelector[] Properties { get; set; }

      public void Refresh(IViewModel rootVM, bool executeRefreshDependencies) {
         IEnumerable<IViewModel> viewModels = Path.IsEmpty ?
            new IViewModel[] { rootVM } :
            Path.GetDescendants(rootVM);

         foreach (IViewModel vm in viewModels) {
            if (Properties.Any()) {
               foreach (var selector in Properties) {
                  var property = selector.GetProperty(vm.Descriptor);
                  vm.Kernel.RefreshInternal(property, executeRefreshDependencies);
               }
            } else {
               vm.Kernel.RefreshInternal(executeRefreshDependencies);
            }
         }
      }

      public void Revalidate(IViewModel rootVM) {
         IEnumerable<IViewModel> viewModels = Path.IsEmpty ?
            new IViewModel[] { rootVM } :
            Path.GetDescendants(rootVM);

         foreach (IViewModel vm in viewModels) {
            if (Properties.Any()) {
               foreach (var selector in Properties) {
                  var property = selector.GetProperty(vm.Descriptor);
                  vm.Kernel.Revalidate(property);
               }
            } else {
               vm.Kernel.Revalidate(ValidationScope.SelfAndLoadedDescendants);
            }
         }
      }

      public override string ToString() {
         return String.Format(
            "{0}.[{1}]",
            Path,
            String.Join(", ", Properties.Select(x => x.PropertyName))
         );
      }
   }
}
