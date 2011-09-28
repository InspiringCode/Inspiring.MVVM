namespace Inspiring.Mvvm.ViewModels {
   using Inspiring.Mvvm.ViewModels.Core;

   public static class BehaviorExtensions {
      public static void IsCached<TVM, TDescriptor, TValue>(
         this ISinglePropertyBehaviorBuilder<TVM, TDescriptor, TValue> builder
      ) {
         if (builder.ContainsBehavior(PropertyBehaviorKeys.ValueCache)) {
            builder.Enable(PropertyBehaviorKeys.ValueCache);
         }
      }
   }
}
