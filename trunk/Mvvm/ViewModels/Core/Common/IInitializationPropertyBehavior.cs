namespace Inspiring.Mvvm.ViewModels.Core {
   /// <summary>
   ///   Implement this behavior if your property behavior needs to declare a 
   ///   dynamic field or needs to know its property.
   /// </summary>
   public interface IBehaviorInitializationBehavior : IBehavior {
      /// <summary>
      ///   Initializes the property behavior.
      /// </summary>
      void Initialize(InitializationContext context);
   }
}
