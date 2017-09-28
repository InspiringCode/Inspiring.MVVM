namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   
   public sealed class ValidationRequest {
      public ValidationRequest(
         ValidationStep step,
         Path targetPath
      ) {
         Check.NotNull(targetPath, nameof(targetPath));
         Step = step;
         TargetPath = targetPath;
      }

      public ValidationRequest(
         ValidationStep step,
         IViewModel vm
      )
         : this(step, Path.Empty.Prepend(vm)) {
         Check.NotNull(vm, nameof(vm));
      }

      public ValidationRequest(
         ValidationStep step,
         IViewModel vm,
         IVMPropertyDescriptor property
      )
         : this(step, Path.Empty.Prepend(property).Prepend(vm)) {
         Check.NotNull(vm, nameof(vm));
         Check.NotNull(property, nameof(property));
      }

      public ValidationStep Step { get; private set; }

      public Path TargetPath { get; private set; }

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
