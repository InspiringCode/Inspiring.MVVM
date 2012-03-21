﻿namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;

   internal struct PathHelperResult {
      public bool Success { get; private set; }
      public IViewModel VM { get; private set; }
      public IVMPropertyDescriptor Property { get; private set; }
      public IVMCollection Collection { get; private set; }

      internal static PathHelperResult Succeeded(
         IViewModel vm = null,
         IVMPropertyDescriptor property = null,
         IVMCollection collection = null
      ) {
         return new PathHelperResult {
            Success = true,
            VM = vm,
            Property = property,
            Collection = collection
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

      public static PathHelperResult SelectsOnlyCollectionOf(this Path path, IViewModel owner) {
         Contract.Requires(path != null);
         Contract.Requires(owner != null);

         bool success =
            path.Length == 2 &&
            path[0].ViewModel == owner &&
            path[1].Type == PathStepType.Collection;

         return success ?
            PathHelperResult.Succeeded(vm: owner, collection: path[1].Collection) :
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

      public static bool SelectsAncestor(this Path path) {
         return
            path.Length >= 2 &&
            path[0].Type == PathStepType.ViewModel &&
            path[1].Type == PathStepType.ViewModel;
      }

      public static IViewModel GetRootVM(this Path path) {
         Contract.Requires<ArgumentException>(!path.IsEmpty);
         Contract.Requires<ArgumentException>(path[0].Type == PathStepType.ViewModel); // TODO: Shouldn't this be an invariant?

         return path[0].ViewModel;
      }

      //public static IViewModel GetLastVM(this Path path) {
      //   Contract.Requires<ArgumentException>(!path.IsEmpty);

      //   for (int i = path.Length - 1; i >= 0; i--) {
      //      if (path[i].Type == PathStepType.ViewModel) {
      //         return path[i].ViewModel;
      //      }
      //   }

      //   throw new ArgumentException();
      //}
   }
}
