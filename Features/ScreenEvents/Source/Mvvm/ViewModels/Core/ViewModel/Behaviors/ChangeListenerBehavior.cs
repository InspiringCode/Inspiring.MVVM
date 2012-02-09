namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   internal sealed class ChangeListenerBehavior<TVM> : Behavior, IChangeHandlerBehavior {
      private Action<TVM, ChangeArgs> _changeHandler;

      public ChangeListenerBehavior(Action<TVM, ChangeArgs> changeHandler) {
         _changeHandler = changeHandler;
      }

      public void HandleChange(IBehaviorContext context, ChangeArgs args) {
         _changeHandler((TVM)context.VM, args);
         this.HandleChangedNext(context, args);
      }
   }
}
