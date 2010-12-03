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

         AppendWithDefaultFactory(t, BehaviorKeys.TypeDescriptor);

         RegisterTemplate(BehaviorChainTemplateKeys.ViewModel, t);
      }

      private static void RegisterPropertyTemplate() {
         var t = new BehaviorChainTemplate();

         t.Append(BehaviorKeys.InvalidDisplayValueCache, disabled: true);
         t.Append(BehaviorKeys.DisplayValueAccessor);
         t.Append(BehaviorKeys.Validator, disabled: true);
         t.Append(BehaviorKeys.PropertyChangedTrigger);
         t.Append(BehaviorKeys.PropertyValueCache, disabled: true);
         t.Append(BehaviorKeys.PropertyValueAcessor);
         t.Append(BehaviorKeys.ManualUpdateBehavior);
         t.Append(BehaviorKeys.SourceValueAccessor, disabled: true);
         t.Append(BehaviorKeys.TypeDescriptor);

         RegisterTemplate(BehaviorChainTemplateKeys.ViewModel, t);
      }

      private static void AppendWithDefaultFactory(
         BehaviorChainTemplate template,
         BehaviorKey key,
         bool isEnabledByDefault = true
      ) {
         template.Append(key, new DefaultBehaviorFactory(key), isEnabledByDefault);
      }
   }
}
