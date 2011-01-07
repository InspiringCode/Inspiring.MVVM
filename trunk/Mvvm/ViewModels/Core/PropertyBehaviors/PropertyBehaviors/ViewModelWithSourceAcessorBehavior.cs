namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class ViewModelWithSourceAcessorBehavior<TVM, TSource> :
      Behavior, IValueAccessorBehavior<TVM>
      where TVM : IViewModel, ICanInitializeFrom<TSource> {

      public TVM GetValue(IBehaviorContext context) {
         var factory = GetNextBehavior<IViewModelFactoryBehavior<TVM>>();

         TVM instance = factory.CreateInstance(context);

         TSource sourceValue = this.GetValueNext<TSource>(context);
         instance.InitializeFrom(sourceValue);

         return instance;
      }

      public void SetValue(IBehaviorContext context, TVM value) {
         this.SetValueNext(context, value);
      }
   }
}
