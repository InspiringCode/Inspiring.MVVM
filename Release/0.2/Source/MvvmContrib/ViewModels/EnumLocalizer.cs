namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using System.Resources;

   public static class EnumLocalizer {
      private static List<ResourceManager> _localizationResources = new List<ResourceManager>();

      public static string GetCaption<TEnum>(TEnum value) {
         Type type = typeof(TEnum);

         // Since generic constrains for Enum are not supported,
         // we'll check it at run-time.
         if (type.BaseType != typeof(Enum)) {
            throw new ArgumentException(ExceptionTexts.TypeNotEnum);
         }

         string name = Enum.GetName(type, value);

         string caption = _localizationResources
            .Select(rm => rm.GetString(type.Name + "_" + name))
            .FirstOrDefault(c => c != null);

         if (caption == null) {
            throw new ArgumentException(
               ExceptionTexts.EnumLocalizationNotFound.FormatWith(type.Name, name)
            );
         }

         return caption;
      }

      public static void AddLocalizationResource(ResourceManager resourceManager) {
         if (!_localizationResources.Contains(resourceManager)) {
            _localizationResources.Add(resourceManager);
         }
      }
   }
}
