namespace Inspiring.Mvvm.ViewModels.Core {

   //internal sealed class ViewModelFactoryBehavior<TVM, TSource> :
   //   Behavior, IAccessPropertyBehavior<TVM>
   //   where TVM : ViewModel, ICanInitializeFrom<TSource> {

   //   public TVM GetValue(IBehaviorContext vm) {
   //      TVM instance = ServiceLocator.Current.GetInstance<TVM>();
   //      TSource source = GetSourceObject(vm);
   //      instance.InitializeFrom(source);
   //      return instance;
   //   }

   //   public void SetValue(IBehaviorContext vm, TVM value) {
   //      throw new NotSupportedException();
   //   }

   //   private TSource GetSourceObject(IBehaviorContext vm) {
   //      return GetNextBehavior<IAccessPropertyBehavior<TSource>>().GetValue(vm);
   //   }
   //}
}
