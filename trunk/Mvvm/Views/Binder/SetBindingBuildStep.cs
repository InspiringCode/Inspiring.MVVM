namespace Inspiring.Mvvm.Views.Binder {
   using System;
   using System.Diagnostics.Contracts;
   using System.Windows.Data;

   public sealed class SetBindingBuildStep : IBinderBuildStep {
      public void Execute(BinderContext context) {
         Contract.Requires<ArgumentException>(
            context.TargetObject != null &&
            context.TargetProperty != null &&
            context.Binding != null &&
            !String.IsNullOrWhiteSpace(context.PropertyPath)
         );

         BindingOperations.SetBinding(
            context.TargetObject,
            context.TargetProperty,
            context.Binding
         );
      }
   }
}
