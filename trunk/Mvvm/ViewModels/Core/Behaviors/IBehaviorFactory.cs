namespace Inspiring.Mvvm.ViewModels.Core {

   /// <summary>
   ///   A factory that creates instances of a certain behavior class with a
   ///   certain <see cref="BehaviorKey"/>. This may be a view model behavior
   ///   or a property behavior.
   /// </summary>
   public interface IBehaviorFactory {
      /// <summary>
      ///   Creates a new instance of the behavior.
      /// </summary>
      /// <typeparam name="T">
      ///   If the behavior is created for a VM property chain, T has the type
      ///   of the property and if the behavior is created for a view model
      ///   behavior chain, T has the type of the view model.
      /// </typeparam>
      IBehavior Create<T>();
   }
}
