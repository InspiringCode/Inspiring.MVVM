namespace Inspiring.Mvvm.Views {
   using System;
   using System.Windows.Input;
   using System.Windows.Markup;

   /// <summary>
   ///   A <see cref="MarkupExtension"/> that creates improved <see 
   ///   cref="KeyGestures"/> using <see cref="CustomKeyGesture.Create"/>.
   /// </summary>
   public class CustomKeyGestureExtension : MarkupExtension {
      private static readonly KeyGestureConverter KeyGestureConverter = new KeyGestureConverter();
      private KeyGesture _keyGesture;

      public CustomKeyGestureExtension(string gestureString) {
         // TODO: Implement custom parsing logic, because Converter also invokes 
         // validating KeyGesture contructor.
         var original = (KeyGesture)KeyGestureConverter.ConvertFromString(gestureString);
         _keyGesture = CustomKeyGesture.Create(original.Key, original.Modifiers);
      }

      public override object ProvideValue(IServiceProvider serviceProvider) {
         return _keyGesture;
      }
   }
}
