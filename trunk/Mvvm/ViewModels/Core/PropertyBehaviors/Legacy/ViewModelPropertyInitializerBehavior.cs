namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   internal sealed class ViewModelPropertyInitializerBehavior<TVM, TSource> :
      Behavior, IPropertyAccessorBehavior<TVM>
      where TVM : IViewModel, ICanInitializeFrom<TSource> {

      public TVM GetValue(IBehaviorContext vm, ValueStage stage) {
         TVM instance = GetNextBehavior<IViewModelFactoryBehavior<TVM>>().CreateInstance(vm);
         TSource source = GetSourceObject(vm);
         instance.InitializeFrom(source);

         instance.Parent = (IViewModel)vm.VM;

         return instance;
      }

      public void SetValue(IBehaviorContext vm, TVM value) {
         throw new NotSupportedException();
      }

      private TSource GetSourceObject(IBehaviorContext vm) {
         return GetNextBehavior<IPropertyAccessorBehavior<TSource>>().GetValue(vm, ValueStage.PostValidation);
      }
   }
}
