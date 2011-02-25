namespace Inspiring.Mvvm.ViewModels.Core {
   public interface IDescendantValidationBehavior : IBehavior {
      void RevalidateDescendants(
         IBehaviorContext context,
         ValidationContext validationContext,
         ValidationScope scope,
         ValidationMode mode
      );
   }
}
