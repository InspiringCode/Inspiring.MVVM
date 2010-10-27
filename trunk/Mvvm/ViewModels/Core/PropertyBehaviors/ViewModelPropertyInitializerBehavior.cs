namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   internal sealed class ViewModelPropertyInitializerBehavior<TVM, TSource> :
      Behavior, IAccessPropertyBehavior<TVM>
      where TVM : ViewModel, ICanInitializeFrom<TSource> {

      public TVM GetValue(IBehaviorContext vm) {
         TVM instance = GetNextBehavior<IViewModelFactoryBehavior<TVM>>().CreateInstance(vm);
         TSource source = GetSourceObject(vm);
         instance.InitializeFrom(source);

         instance.Parent = vm.VM;

         return instance;
      }

      public void SetValue(IBehaviorContext vm, TVM value) {
         throw new NotSupportedException();
      }

      private TSource GetSourceObject(IBehaviorContext vm) {
         return GetNextBehavior<IAccessPropertyBehavior<TSource>>().GetValue(vm);
      }
   }
}
