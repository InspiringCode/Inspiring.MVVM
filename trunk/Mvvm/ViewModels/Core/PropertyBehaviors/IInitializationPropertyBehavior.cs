namespace Inspiring.Mvvm.ViewModels.Core {
   /// <summary>
   ///   Implement this behavior if your property behavior needs to declare a 
   ///   dynamic field or needs to know its property.
   /// </summary>
   public interface IInitializationPropertyBehavior : IBehavior {
      /// <summary>
      ///   Initializes the property behavior.
      /// </summary>
      void Initialize(PropertyBehaviorInitializationContext context);
   }
}
