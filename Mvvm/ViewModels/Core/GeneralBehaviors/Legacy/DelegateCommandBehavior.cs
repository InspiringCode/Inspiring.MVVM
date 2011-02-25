namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Windows.Input;
   using Inspiring.Mvvm.Screens;

   internal sealed class DelegateCommandBehavior<TSource> : Behavior, IValueAccessorBehavior<ICommand> {
      private Action<TSource> _execute;
      private Func<TSource, bool> _canExecute;

      public DelegateCommandBehavior(
         Action<TSource> execute,
         Func<TSource, bool> canExecute
      ) {
         _execute = execute;
         _canExecute = canExecute;
      }

      public ICommand GetValue(IBehaviorContext vm) {
         // TODO: Is there a better way that has less overhead?
         TSource source = GetSourceObject(vm);

         Func<bool> canExecute = null;
         if (_canExecute != null) {
            canExecute = () => _canExecute(source);
         }

         return DelegateCommand.For(() => _execute(source), canExecute);
      }

      public void SetValue(IBehaviorContext vm, ICommand value) {
         throw new NotSupportedException();
      }

      private TSource GetSourceObject(IBehaviorContext context) {
         // TODO: Why is there a specail case?
         IValueAccessorBehavior<TSource> sourceAccessor;
         return TryGetBehavior(out sourceAccessor) ?
            sourceAccessor.GetValue(context) :
            (TSource)context;
      }
   }
}
