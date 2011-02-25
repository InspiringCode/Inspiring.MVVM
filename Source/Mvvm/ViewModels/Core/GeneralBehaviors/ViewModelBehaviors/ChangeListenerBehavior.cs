namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   internal sealed class ChangeListenerBehavior<TVM> : ViewModelBehavior {
      private Action<TVM, ChangeArgs, InstancePath> _changeHandler;

      public ChangeListenerBehavior(Action<TVM, ChangeArgs, InstancePath> changeHandler) {
         _changeHandler = changeHandler;
      }

      protected internal override void OnChanged(IBehaviorContext context, ChangeArgs args, InstancePath changedPath) {
         base.OnChanged(context, args, changedPath);

         _changeHandler((TVM)context.VM, args, changedPath);
      }
   }
}
