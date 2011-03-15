namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Diagnostics.Contracts;

   public sealed class ValidationRequest {
      public ValidationRequest(
         ValidationTrigger trigger,
         ValidationStep step,
         Path targetPath
      ) {
         Contract.Requires(targetPath != null);
         Trigger = trigger;
         Step = step;
         TargetPath = targetPath;
      }

      public ValidationRequest(
         ValidationTrigger trigger,
         ValidationStep step,
         IViewModel vm
      )
         : this(trigger, step, Path.Empty.Prepend(vm)) {
         Contract.Requires(vm != null);
         Target = vm;
      }

      public ValidationRequest(
         ValidationTrigger trigger,
         ValidationStep step,
         IViewModel vm,
         IVMPropertyDescriptor property
      )
         : this(trigger, step, Path.Empty.Prepend(property).Prepend(vm)) {
         Contract.Requires(vm != null);
         Contract.Requires(property != null);
         Target = vm;
         TargetProperty = property;
      }

      public ValidationTrigger Trigger { get; private set; }

      public ValidationStep Step { get; private set; }

      public Path TargetPath { get; private set; }

      public IViewModel Target { get; private set; }

      public IVMPropertyDescriptor TargetProperty { get; private set; }
   }
}
