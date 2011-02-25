namespace Inspiring.Mvvm.ViewModels.Core.BehaviorInterfaces.PropertyBehaviors {
   using System.Windows.Input;

   public interface ICommandFactoryBehavior : IBehavior {
      ICommand CreateCommand(IBehaviorContext context);
   }
}
