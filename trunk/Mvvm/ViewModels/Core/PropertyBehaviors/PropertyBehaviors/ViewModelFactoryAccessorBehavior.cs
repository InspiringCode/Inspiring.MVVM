namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   internal sealed class ViewModelFactoryAccessorBehavior<TValue> :
      Behavior,
      IValueAccessorBehavior<TValue>
      where TValue : IViewModel {

      public TValue GetValue(IBehaviorContext context) {
         var factory = GetNextBehavior<IViewModelFactoryBehavior<TValue>>();
         return factory.CreateInstance(context);
      }

      public void SetValue(IBehaviorContext context, TValue value) {
         throw new NotSupportedException(); // TODO: Better error message.
      }
   }
}
