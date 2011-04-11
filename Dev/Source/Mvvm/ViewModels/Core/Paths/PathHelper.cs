namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Diagnostics.Contracts;

   internal struct PathHelperResult {
      public bool Success { get; private set; }
      public IViewModel VM { get; private set; }
      public IVMPropertyDescriptor Property { get; private set; }

      internal static PathHelperResult Succeeded(
         IViewModel vm = null,
         IVMPropertyDescriptor property = null
      ) {
         return new PathHelperResult {
            Success = true,
            VM = vm,
            Property = property
         };
      }

      internal static PathHelperResult Failed() {
         return new PathHelperResult();
      }
   }

   internal static class PathHelper {
      public static PathHelperResult SelectsOnlyPropertyOf(this Path path, IViewModel owner) {
         Contract.Requires(path != null);
         Contract.Requires(owner != null);

         bool success =
            path.Length == 2 &&
            path[0].ViewModel == owner &&
            path[1].Type == PathStepType.Property;

         return success ?
            PathHelperResult.Succeeded(vm: owner, property: path[1].Property) :
            PathHelperResult.Failed();
      }

      public static PathHelperResult SelectsOnly(this Path path, IViewModel singleViewModel) {
         Contract.Requires(path != null);
         Contract.Requires(singleViewModel != null);

         bool success =
            path.Length == 1 &&
            path[0].ViewModel == singleViewModel;

         return success ?
            PathHelperResult.Succeeded(vm: singleViewModel) :
            PathHelperResult.Failed();
      }
   }
}
