namespace Inspiring.Mvvm.ViewModels.Core {

   public interface IMutabilityCheckerBehavior : IBehavior {
      bool IsMutable(IBehaviorContext vm);
   }
}
