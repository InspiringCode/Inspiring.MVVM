namespace Inspiring.Mvvm.ViewModels.Core {

   public interface IValueFactoryBehavior<TValue> : IBehavior {
      TValue CreateValue(IBehaviorContext context);
   }
}
