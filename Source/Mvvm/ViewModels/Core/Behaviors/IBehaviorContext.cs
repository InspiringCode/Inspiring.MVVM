namespace Inspiring.Mvvm.ViewModels.Core {
   /// <summary>
   ///   Holds that state needed by operations of a <see cref="ViewModelBehavior"/>.
   /// </summary>
   public interface IBehaviorContext {
      IViewModel VM { get; }

      FieldValueHolder FieldValues { get; }

      IServiceLocator ServiceLocator { get; }

      void NotifyChange(ChangeArgs args);
   }
}