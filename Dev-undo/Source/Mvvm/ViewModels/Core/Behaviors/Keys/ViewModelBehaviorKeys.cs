namespace Inspiring.Mvvm.ViewModels.Core {

   internal class ViewModelBehaviorKeys : BehaviorKeys {
      public static readonly BehaviorKey ViewModelValidationSource = Key(() => ViewModelValidationSource);
      public static readonly BehaviorKey ValidationExecutor = Key(() => ValidationExecutor);
      public static readonly BehaviorKey UndoRoot = Key(() => UndoRoot);
   }
}
