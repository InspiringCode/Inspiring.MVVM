namespace Inspiring.Mvvm.ViewModels.Core {
   /// <summary>
   ///   Specifies what is being validated.
   /// </summary>
   internal enum ValidationType {
      /// <summary>
      ///   A whole view model is being validated after something has changed.
      /// </summary>
      ViewModel,

      /// <summary>
      ///   The display value of a property before being converted to its target
      ///   type is being validated.
      /// </summary>
      PropertyDisplayValue,

      /// <summary>
      ///   The strongly typed value (after possibly being successfully 
      ///   converted to its target type) is being validated.
      /// </summary>
      PropertyValue
   }
}
