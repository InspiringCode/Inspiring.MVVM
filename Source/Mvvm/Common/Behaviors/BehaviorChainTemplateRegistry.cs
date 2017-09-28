namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Collections.Generic;

   /// <summary>
   ///   A registry that holds all available <see cref="BehaviorChainTemplate"/> 
   ///   objects.
   /// </summary>
   public static partial class BehaviorChainTemplateRegistry {
      private static Dictionary<BehaviorChainTemplateKey, BehaviorChainTemplate> _templates
         = new Dictionary<BehaviorChainTemplateKey, BehaviorChainTemplate>();

      static BehaviorChainTemplateRegistry() {
         ResetToDefaults();
      }

      /// <summary>
      ///   Registers or overrides the given <see cref="BehaviorChainTemplateKey"/>
      ///   with the given <see cref="BehaviorChainTemplate"/>.
      /// </summary>
      public static void RegisterTemplate(BehaviorChainTemplateKey key, BehaviorChainTemplate template) {
         Check.NotNull(key, nameof(key));
         Check.NotNull(template, nameof(template));

         _templates[key] = template;
      }

      /// <summary>
      ///   Returns a template previously register with <see cref="RegisterTemplate"/>.
      /// </summary>
      /// <exception cref="KeyNotFoundException">
      ///   No template with the given <paramref name="key"/> was registered.
      /// </exception>
      public static BehaviorChainTemplate GetTemplate(BehaviorChainTemplateKey key) {
         Check.NotNull(key, nameof(key));

         return _templates[key];
      }

      public static void ResetToDefaults() {
         DefaultTemplates.RegisterDefaultTemplates();
      }
   }
}
