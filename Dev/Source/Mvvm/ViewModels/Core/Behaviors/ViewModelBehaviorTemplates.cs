namespace Inspiring.Mvvm.ViewModels.Core.Behaviors {

   internal class ViewModelBehaviorTemplates {
      public static void RegisterDefaultTemplates() {
         RegisterPropertyTemplate();

         RegisterViewModelTemplate();
      }

      private static void RegisterPropertyTemplate() {
         BehaviorChainTemplateRegistry.RegisterTemplate(
            BehaviorChainTemplateKeys.Property,
            new BehaviorChainTemplate(PropertyBehaviorFactoryProvider.Default)
               .Append(PropertyBehaviorKeys.DisplayValueAccessor)
               .Append(PropertyBehaviorKeys.UntypedValueAccessor)
               .Append(PropertyBehaviorKeys.Undo, DefaultBehaviorState.Disabled)
               .Append(PropertyBehaviorKeys.ValueValidationSource, DefaultBehaviorState.Disabled)
               .Append(PropertyBehaviorKeys.PropertyChangedNotifier)
               .Append(PropertyBehaviorKeys.SourceAccessor, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(PropertyBehaviorKeys.TypeDescriptor)
         );
      }

      private static void RegisterViewModelTemplate() {
         BehaviorChainTemplateRegistry.RegisterTemplate(
            BehaviorChainTemplateKeys.ViewModel,
            new BehaviorChainTemplate(ViewModelBehaviorFactoryProvider.Default)
            //.Append(PropertyBehaviorKeys.ManualUpdateCoordinator)
            //.Append(PropertyBehaviorKeys.Validator, DefaultBehaviorState.Disabled)
            //.Append(ViewModelBehaviorKeys.ViewModelValidationSource, DefaultBehaviorState.Disabled)
            //.Append(ViewModelBehaviorKeys.ValidationExecutor, DefaultBehaviorState.Disabled)
            //.Append(PropertyBehaviorKeys.TypeDescriptor)
            //.Append(ViewModelBehaviorKeys.UndoRoot, DefaultBehaviorState.Disabled)
         );
      }
   }
}
