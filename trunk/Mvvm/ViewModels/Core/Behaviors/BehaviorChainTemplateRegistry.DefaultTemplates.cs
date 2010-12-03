using System.Diagnostics.Contracts;
using Inspiring.Mvvm.ViewModels.Core.Behaviors;
namespace Inspiring.Mvvm.ViewModels.Core {
   public static partial class BehaviorChainTemplateRegistry {
      static BehaviorChainTemplateRegistry() {
         RegisterDefaultTemplates();
      }

      private static void RegisterDefaultTemplates() {
         RegisterViewModelTemplate();
         RegisterPropertyTemplate();
      }

      private static void RegisterViewModelTemplate() {
         var t = new BehaviorChainTemplate();

         AppendViewModel(t, BehaviorKeys.TypeDescriptor);

         RegisterTemplate(BehaviorChainTemplateKeys.ViewModel, t);
      }

      private static void RegisterPropertyTemplate() {
         var t = new BehaviorChainTemplate();

         Append(t, BehaviorKeys.InvalidDisplayValueCache, disabled: true);
         Append(t, BehaviorKeys.DisplayValueAccessor);
         Append(t, BehaviorKeys.Validator, disabled: true);
         Append(t, BehaviorKeys.PropertyChangedTrigger);
         Append(t, BehaviorKeys.PropertyValueCache, disabled: true);
         //Append(t, BehaviorKeys.ManualUpdateBehavior); // TODO: Is this correct?
         Append(t, BehaviorKeys.SourceValueAccessor, disabled: true, withoutFactory: true);
         Append(t, BehaviorKeys.TypeDescriptor);

         RegisterTemplate(BehaviorChainTemplateKeys.Property, t);
      }

      /// <summary>
      ///   Appends the behavior key with the <see cref="DefaultPropertyBehaviorFactory"/>.
      /// </summary>
      private static void Append(
         BehaviorChainTemplate template,
         BehaviorKey key,
         bool disabled = false,
         bool withoutFactory = false
      ) {
         Contract.Requires(withoutFactory ? disabled : true);

         if (withoutFactory) {
            template.Append(key);
         } else {
            template.Append(key, new DefaultPropertyBehaviorFactory(key), disabled);
         }
      }

      /// <summary>
      ///   Appends the behavior key with the <see cref="DefaultViewModelBehaviorFactory"/>.
      /// </summary>
      private static void AppendViewModel(
         BehaviorChainTemplate template,
         BehaviorKey key,
         bool disabled = false
      ) {
         template.Append(key, new DefaultViewModelBehaviorFactory(key), disabled);
      }
   }
}
