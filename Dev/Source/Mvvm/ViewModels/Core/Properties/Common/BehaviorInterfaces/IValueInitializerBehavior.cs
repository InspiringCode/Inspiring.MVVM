namespace Inspiring.Mvvm.ViewModels.Core {

   public interface IValueInitializerBehavior : IBehavior {
      void InitializeValue(IBehaviorContext context);
   }
}
