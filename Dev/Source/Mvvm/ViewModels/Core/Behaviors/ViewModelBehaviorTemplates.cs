namespace Inspiring.Mvvm.ViewModels.Core.Behaviors {
   using System;
   using System.Collections.Generic;
   using System.Linq;

   internal class ViewModelBehaviorTemplates {
      public static void RegisterDefaultTemplates() {
         
      }

      private static void RegisterPropertyTemplate() {
         BehaviorChainTemplateRegistry.RegisterTemplate(
            BehaviorChainTemplateKeys.Property,
            new BehaviorChainTemplate(PropertyBehaviorFactoryProvider.Default)
               .Append(PropertyBehaviorKeys.DisplayValueAccessor)
               .Append(PropertyBehaviorKeys.UntypedValueAccessor)
               .Append(PropertyBehaviorKeys.Undo, DefaultBehaviorState.Disabled)
               //.Append(PropertyBehaviorKeys.RefreshBehavior, DefaultBehaviorState.DisabledWithoutFactory)
               //.Append(PropertyBehaviorKeys.ManualUpdateBehavior, DefaultBehaviorState.Disabled)
               //.Append(PropertyBehaviorKeys.ValueValidationSource, DefaultBehaviorState.Disabled)
               //.Append(PropertyBehaviorKeys.Validator, DefaultBehaviorState.Disabled)
               //.Append(PropertyBehaviorKeys.PropertyChangedTrigger)
               //.Append(PropertyBehaviorKeys.SourceAccessor, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(PropertyBehaviorKeys.TypeDescriptor)
         );
      }
   }
}
