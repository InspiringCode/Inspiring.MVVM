namespace Inspiring.Mvvm.Views {
   using System;
   using System.Collections.Generic;
   using System.Reflection;
   using System.Resources;
   using System.Windows.Input;

   /// <summary>
   ///   A factory for enhanced <see cref="KeyGesture"/>s that do not validate
   ///   its modifiers/key and that allow easy localization.
   /// </summary>
   public static class CustomKeyGesture {
      private const string DefaultKeySeparator = "+";

      private static readonly ModifierKeysConverter ModifierKeysConverter = new ModifierKeysConverter();
      private static readonly KeyConverter KeyConverter = new KeyConverter();

      private static readonly Type[] KeyGestureConstructorSignature = new Type[] {
         typeof(Key),
         typeof(ModifierKeys),
         typeof(string),
         typeof(bool)
      };

      private static readonly ConstructorInfo KeyGestureConstructorInfo = typeof(KeyGesture).GetConstructor(
         BindingFlags.NonPublic | BindingFlags.Instance,
         binder: null,
         types: KeyGestureConstructorSignature,
         modifiers: null
      );

      private static ResourceManager _localizationResourceManager = null;

      /// <summary>
      ///   Returns a new <see cref="KeyGesture"/> for the given <paramref
      ///   name="key"/> and <paramref name="modifiers"/>.
      /// </summary>
      public static KeyGesture Create(Key key, ModifierKeys modifiers) {

         // The KeyGesture constructor allows only certain keys and modifiers but
         // it provides a private constructor, which allows to disable this 
         // validation. This is certainly a hack but it was the only practical way 
         // I found to circumvent the validation. An alternative would be to
         // derive from InputGesture and implement a custom KeyGesture, but this
         // is problematic as some properties require the gesture to be of type
         // KeyGesture and not InputGesture.

         bool validateGesture = false;

         return (KeyGesture)KeyGestureConstructorInfo.Invoke(new object[] {
            key,
            modifiers,
            GetDisplayString(key, modifiers),
            validateGesture
         });
      }

      /// <summary>
      ///   Allows the localization of <see cref="KeyGesture.DisplayString"/> of 
      ///   key gestures created by <see cref="CustomKeyGesture.Create"/>.
      /// </summary>
      /// <param name="resourceFile">
      ///   A <see cref="ResourceManager"/> (for example 'Localized.ResourceManager')
      ///   that may contain the following keys: "KeySeparator" which is used to
      ///   join the modifies/keys ("+" by default), an entry of the form
      ///   "ModifierKeys_EnumMember" to localize modifier keys and "Key_EnumMember"
      ///   to localize keys. If a resource key is not found, the WPF default
      ///   texts are used.
      /// </param>
      public static void SupplyLocalization(ResourceManager resourceFile) {
         Check.NotNull(resourceFile, nameof(resourceFile));

         _localizationResourceManager = resourceFile;
      }

      private static string GetDisplayString(Key key, ModifierKeys modifiers) {

         var elements = new List<string>();

         if ((modifiers & ModifierKeys.Control) == ModifierKeys.Control) {
            elements.Add(GetModifierString(ModifierKeys.Control));
         }

         if ((modifiers & ModifierKeys.Alt) == ModifierKeys.Alt) {
            elements.Add(GetModifierString(ModifierKeys.Alt));
         }

         if ((modifiers & ModifierKeys.Windows) == ModifierKeys.Windows) {
            elements.Add(GetModifierString(ModifierKeys.Windows));
         }

         if ((modifiers & ModifierKeys.Shift) == ModifierKeys.Shift) {
            elements.Add(GetModifierString(ModifierKeys.Shift));
         }

         elements.Add(GetKeyString(key));

         return String.Join(GetKeySeparatorString(), elements);
      }

      private static string GetKeySeparatorString() {
         string separator = null;

         if (_localizationResourceManager != null) {
            separator = _localizationResourceManager.GetString("KeySeparator");
         }

         return separator ?? DefaultKeySeparator;
      }

      private static string GetKeyString(Key key) {

         string keyStr = null;

         if (_localizationResourceManager != null) {
            keyStr = _localizationResourceManager.GetString("Key_" + key);
         }

         return keyStr ?? KeyConverter.ConvertToString(key);
      }

      private static string GetModifierString(ModifierKeys modifier) {

         string modifierStr = null;

         if (_localizationResourceManager != null) {
            modifierStr = _localizationResourceManager.GetString("ModifierKeys_" + modifier);
         }

         return modifierStr ?? ModifierKeysConverter.ConvertToString(modifier);
      }
   }
}
