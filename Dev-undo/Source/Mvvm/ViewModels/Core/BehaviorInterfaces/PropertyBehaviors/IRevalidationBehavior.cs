namespace Inspiring.Mvvm.ViewModels.Core {
   /// <summary>
   ///   A property behavior that can revalidate its property.
   /// </summary>
   public interface IRevalidationBehavior {
      void Revalidate(IBehaviorContext context, ValidationContext validationContext, ValidationMode mode);
      void Revalidate(IBehaviorContext context);
      void Revalidate(IBehaviorContext context, CollectionResultCache cache);
   }
}
