namespace Inspiring.Mvvm.ViewModels {
   using Inspiring.Mvvm.ViewModels.Core;

   public static class StandardBehaviors {
      public static void Disconnect<T>(this IVMBehaviorConfigurator builder, VMProperty<T> property) {
         builder.Custom(property, VMBehaviorKey.PropertyValueCache); // TODO: Refactor cache keys!!!
      }
   }
}
