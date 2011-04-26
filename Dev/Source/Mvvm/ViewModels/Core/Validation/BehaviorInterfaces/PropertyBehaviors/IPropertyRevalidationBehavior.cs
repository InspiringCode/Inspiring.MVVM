namespace Inspiring.Mvvm.ViewModels.Core {
   /// <summary>
   ///   A property behavior that can revalidate its property.
   /// </summary>
   public interface IPropertyRevalidationBehavior {
      void BeginValidation(IBehaviorContext context, ValidationController controller);
      void Revalidate(IBehaviorContext context, CollectionResultCache cache);
      void EndValidation(IBehaviorContext context);
   }
}
