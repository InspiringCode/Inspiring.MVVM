namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   internal sealed class ViewModelPropertyInitializerBehavior<TVM, TSource> :
      Behavior, IValueAccessorBehavior<TVM>
      where TVM : IViewModel, ICanInitializeFrom<TSource> {

      public TVM GetValue(IBehaviorContext vm, ValueStage stage) {
         TVM instance = GetNextBehavior<IViewModelFactoryBehavior<TVM>>().CreateInstance(vm);
         TSource source = GetSourceObject(vm);
         instance.InitializeFrom(source);

         instance.Kernel.Parent = (IViewModel)vm.VM;

         return instance;
      }

      public void SetValue(IBehaviorContext vm, TVM value) {
         throw new NotSupportedException();
      }

      private TSource GetSourceObject(IBehaviorContext vm) {
         return GetNextBehavior<IValueAccessorBehavior<TSource>>().GetValue(vm, ValueStage.PostValidation);
      }
   }
}
