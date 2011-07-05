namespace Inspiring.Mvvm.ViewModels.Core {
   internal sealed class ViewModelInitializerBehavior<TValue> :
      Behavior,
      IValueAccessorBehavior<TValue>,
      IValueInitializerBehavior
      where TValue : IViewModel {

      public TValue GetValue(IBehaviorContext context) {
         return this.GetValueNext<TValue>(context);
      }

      public void SetValue(IBehaviorContext context, TValue value) {
         TValue previousValue = this.GetValueNext<TValue>(context);

         HandleAddedToHierarchy(context, value);
         this.SetValueNext(context, value);
         HandleRemovedFromHierarchy(context, previousValue);
      }

      public void InitializeValue(IBehaviorContext context) {
         TValue initialValue = this.GetValueNext<TValue>(context);
         HandleAddedToHierarchy(context, initialValue);

         this.InitializeValueNext(context);
      }

      private void HandleAddedToHierarchy(IBehaviorContext context, TValue viewModel) {
         if (viewModel != null) {
            viewModel.Kernel.Parents.Add(context.VM);
         }
      }

      private void HandleRemovedFromHierarchy(IBehaviorContext context, TValue viewModel) {
         if (viewModel != null) {
            viewModel.Kernel.Parents.Remove(context.VM);
         }
      }
   }
}
