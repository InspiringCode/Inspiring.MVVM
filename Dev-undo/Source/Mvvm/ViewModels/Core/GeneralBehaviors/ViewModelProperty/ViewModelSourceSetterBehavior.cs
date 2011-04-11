namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class ViewModelSourceSetterBehavior<TValue, TSource> :
      Behavior,
      IValueInitializerBehavior,
      IValueAccessorBehavior<TValue>,
      IRefreshBehavior
      where TValue : IViewModel, IHasSourceObject<TSource> {

      public void InitializeValue(IBehaviorContext context) {
         SetSourceObject(context);
         this.InitializeValueNext(context);
      }

      public TValue GetValue(IBehaviorContext context) {
         return this.GetValueNext<TValue>(context);
      }

      public void SetValue(IBehaviorContext context, TValue value) {
         UpdateSourceObject(context, value.Source);
         this.SetValueNext(context, value);
      }

      public void Refresh(IBehaviorContext context) {
         SetSourceObject(context);
         this.RefreshNext(context);
      }

      private void SetSourceObject(IBehaviorContext context) {
         TValue viewModel = this.GetValueNext<TValue>(context);
         TSource source = this.GetValueNext<TSource>(context);
         viewModel.Source = source;
      }

      private void UpdateSourceObject(IBehaviorContext context, TSource sourceValue) {
         this.SetValueNext<TSource>(context, sourceValue);
      }
   }
}
