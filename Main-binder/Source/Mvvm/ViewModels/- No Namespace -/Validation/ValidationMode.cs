namespace Inspiring.Mvvm.ViewModels {
   /// <summary>
   ///   Determines how a revalidation is performed.
   /// </summary>
   public enum ValidationMode {
      /// <summary>
      ///   A property may have invalid pre-conversion and pre-validation values.
      ///   This option revalidates the invalid values and writes them to the 
      ///   source object it the have become valid.
      /// </summary>
      CommitValidValues,

      /// <summary>
      ///   With this option the pre-conversion and pre-validation values of the 
      ///   property are discarded and the post-validation value is revalidated.
      /// </summary>
      DiscardInvalidValues
   }
}
