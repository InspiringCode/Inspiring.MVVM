namespace Inspiring.Mvvm.Views.Binder {
   using System.Windows.Controls;

   public sealed class DataGridBinderBuildStep : IBinderBuildStep {
      public void Execute(BinderContext context) {
         DataGridBoundColumn boundColumn = context.TargetObject as DataGridBoundColumn;
         if (boundColumn != null) {
            //context.PrepareBinding();
            boundColumn.Binding = context.Binding;
            context.Complete = true;
         }
      }
   }
}
