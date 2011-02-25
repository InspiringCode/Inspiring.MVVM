namespace Inspiring.Mvvm.ViewModels.Core {
   /// <summary>
   ///   Implement this behavior if your property behavior needs to declare a 
   ///   dynamic field or needs to know its property. The values of dynamic 
   ///   fields are stored with each <see cref="ViewModel"/> instance.
   ///   The value of fields defined in this way can be accessed via the 
   ///   <see cref="IBehaviorContext.FieldValues"/> property.
   /// </summary>
   public interface IBehaviorInitializationBehavior : IBehavior {
      /// <summary>
      ///   Initializes the property behavior.
      /// </summary>
      void Initialize(BehaviorInitializationContext context);
   }
}
