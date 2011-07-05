namespace Inspiring.Mvvm.ViewModels.Core {

   public interface IUntypedValueGetterBehavior : IBehavior {
      object GetValue(IBehaviorContext context);
   }
}
