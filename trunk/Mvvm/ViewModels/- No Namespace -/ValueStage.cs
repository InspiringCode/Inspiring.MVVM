namespace Inspiring.Mvvm.ViewModels {
   /// <summary>
   ///   Sepecifies the type of property value that should be returned by the
   ///   'GetValue' method of the VM.
   /// </summary>
   public enum ValueStage {
      /// <summary>
      ///   The stage is not relevant for the current operation.
      /// </summary>
      None,

      /// <summary>
      ///   The value before it is converted to the type of the property. It is
      ///   usually set by the View and may be of a different type than the 
      ///   property.
      /// </summary>
      PreConversion,

      /// <summary>
      ///   The value after it is converted to the correct type but before the 
      ///   strongly typed validation is performed. 
      /// </summary>
      PreValidation,

      /// <summary>
      ///   The value after all validations has succeeded. This is usually the 
      ///   same value as in the source objects.
      /// </summary>
      PostValidation
   }
}
