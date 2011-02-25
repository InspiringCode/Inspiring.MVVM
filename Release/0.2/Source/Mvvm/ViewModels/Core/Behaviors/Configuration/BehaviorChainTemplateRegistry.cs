namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;

   /// <summary>
   ///   A registry that holds all available <see cref="BehaviorChainTemplate"/> 
   ///   objects.
   /// </summary>
   public static partial class BehaviorChainTemplateRegistry {
      private static Dictionary<BehaviorChainTemplateKey, BehaviorChainTemplate> _templates
         = new Dictionary<BehaviorChainTemplateKey, BehaviorChainTemplate>();

      /// <summary>
      ///   Registers or overrides the given <see cref="BehaviorChainTemplateKey"/>
      ///   with the given <see cref="BehaviorChainTemplate"/>.
      /// </summary>
      public static void RegisterTemplate(BehaviorChainTemplateKey key, BehaviorChainTemplate template) {
         Contract.Requires<ArgumentNullException>(key != null);
         Contract.Requires<ArgumentNullException>(template != null);

         _templates[key] = template;
      }

      /// <summary>
      ///   Returns a template previously register with <see cref="RegisterTemplate"/>.
      /// </summary>
      /// <exception cref="KeyNotFoundException">
      ///   No template with the given <paramref name="key"/> was registered.
      /// </exception>
      public static BehaviorChainTemplate GetTemplate(BehaviorChainTemplateKey key) {
         Contract.Requires<ArgumentNullException>(key != null);

         return _templates[key];
      }
   }
}
