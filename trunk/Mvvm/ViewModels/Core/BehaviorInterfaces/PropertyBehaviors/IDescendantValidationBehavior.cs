namespace Inspiring.Mvvm.ViewModels.Core {
   public interface IDescendantValidationBehavior : IBehavior {
      void RevalidateDescendants(
         IBehaviorContext context,
         ValidationScope scope,
         ValidationMode mode
      );
   }
}
