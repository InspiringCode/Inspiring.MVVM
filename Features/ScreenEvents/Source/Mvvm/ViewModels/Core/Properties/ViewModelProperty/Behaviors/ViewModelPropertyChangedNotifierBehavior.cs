namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class ViewModelPropertyChangedNotifierBehavior<TValue> :
      PropertyChangedNotifierBehavior<TValue>
      where TValue : IViewModel {

      public override void NotifyPropertyChanged(IBehaviorContext context, ValueStage stage, TValue oldValue, TValue newValue) {
         var args = ChangeArgs.ViewModelPropertyChanged(
            _property,
            stage,
            oldValue,
            newValue,
            reason: null
         );

         context.NotifyChange(args);
      }
   }
}
