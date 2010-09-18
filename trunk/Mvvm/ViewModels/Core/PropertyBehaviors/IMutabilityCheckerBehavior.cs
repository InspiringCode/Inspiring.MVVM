namespace Inspiring.Mvvm.ViewModels.Core.PropertyBehaviors {

   public interface IMutabilityCheckerBehavior : IBehavior {
      bool IsMutable(IBehaviorContext vm);
   }
}
