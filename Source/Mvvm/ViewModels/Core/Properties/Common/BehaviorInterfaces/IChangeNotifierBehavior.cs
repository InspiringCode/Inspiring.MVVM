namespace Inspiring.Mvvm.ViewModels.Core {
   // TODO: Is this the best solution? Alternatives: Make ChangeArgs factory method so smart,
   //       that it detects OldItems and NewItems, extract logic to helper class, ...
   public interface IChangeNotifierBehavior<TValue> {
      void NotifyPropertyChanged(
         IBehaviorContext context,
         ValueStage stage,
         TValue oldValue,
         TValue newValue
      );
   }
}
