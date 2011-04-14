namespace Inspiring.Mvvm.ViewModels.Core {
   public class CommandPropertyBehaviorKeys : PropertyBehaviorKeys {
      public static readonly BehaviorKey CommandExecutor = Key(() => CommandExecutor);
      public static readonly BehaviorKey WaitCursor = Key(() => WaitCursor);
   }
}
