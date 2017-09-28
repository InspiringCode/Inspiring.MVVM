namespace Inspiring.Mvvm.ViewModels.Core {
   /// <summary>
   /// A behavior that intercepts or implements the strongly typed get/set
   /// operation of a property.
   /// </summary>
   /// <typeparam propertyName="TValue">The type of the property target.</typeparam>
   public interface IValueAccessorBehavior<TValue> : IBehavior {
      TValue GetValue(IBehaviorContext context);
      void SetValue(IBehaviorContext context, TValue value);
   }
}
