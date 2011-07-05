namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;

   public sealed class ValidationRequest {
      public ValidationRequest(
         ValidationStep step,
         Path targetPath
      ) {
         Contract.Requires(targetPath != null);
         Step = step;
         TargetPath = targetPath;
      }

      public ValidationRequest(
         ValidationStep step,
         IViewModel vm
      )
         : this(step, Path.Empty.Prepend(vm)) {
         Contract.Requires(vm != null);
         Target = vm;
      }

      public ValidationRequest(
         ValidationStep step,
         IViewModel vm,
         IVMPropertyDescriptor property
      )
         : this(step, Path.Empty.Prepend(property).Prepend(vm)) {
         Contract.Requires(vm != null);
         Contract.Requires(property != null);
         Target = vm;
         TargetProperty = property;
      }

      public ValidationStep Step { get; private set; }

      public Path TargetPath { get; private set; }

      public IViewModel Target { get; private set; }

      public IVMPropertyDescriptor TargetProperty { get; private set; }

      public override string ToString() {
         return String.Format("{{Step = {0}, Path = {1}}}", Step, TargetPath);
      }

      internal ValidationRequest PrependAncestor(IViewModel ancestor) {
         return new ValidationRequest(
            Step,
            TargetPath.Prepend(ancestor)
         );
      }
   }
}
