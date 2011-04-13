namespace Inspiring.Mvvm.ViewModels.Core.Behaviors {

   internal class DefaultTemplates {
      public static void RegisterDefaultTemplates() {
         RegisterPropertyTemplate();

         RegisterViewModelTemplate();
      }

      private static void RegisterPropertyTemplate() {
         BehaviorChainTemplateRegistry.RegisterTemplate(
            BehaviorChainTemplateKeys.Property,
            new BehaviorChainTemplate(DefaultProviders.SimpleProperty)
               .Append(PropertyBehaviorKeys.DisplayValueAccessor)
               .Append(PropertyBehaviorKeys.UntypedValueAccessor)
               .Append(PropertyBehaviorKeys.Undo, DefaultBehaviorState.Disabled)
               .Append(PropertyBehaviorKeys.ValueValidationSource, DefaultBehaviorState.Disabled)
               .Append(PropertyBehaviorKeys.PropertyChangedNotifier)
               .Append(PropertyBehaviorKeys.ValueAccessor, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(PropertyBehaviorKeys.PropertyDescriptorProvider)
         );
      }

      private static void RegisterPropertyWithSourceTemplate() {
         BehaviorChainTemplateRegistry.RegisterTemplate(
            BehaviorChainTemplateKeys.Property,
            new BehaviorChainTemplate(DefaultProviders.SimpleProperty)
               .Append(PropertyBehaviorKeys.DisplayValueAccessor)
               .Append(PropertyBehaviorKeys.UntypedValueAccessor)
               .Append(PropertyBehaviorKeys.Undo, DefaultBehaviorState.Disabled)
               .Append(PropertyBehaviorKeys.ValueValidationSource, DefaultBehaviorState.Disabled)
               .Append(PropertyBehaviorKeys.PropertyChangedNotifier)
               .Append(PropertyBehaviorKeys.SourceAccessor, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(PropertyBehaviorKeys.PropertyDescriptorProvider)
         );
      }

      private static readonly BehaviorChainTemplate ViewModelPropertyTemplate = new BehaviorChainTemplate(null)
         .Append(PropertyBehaviorKeys.DisplayValueAccessor)
         .Append(PropertyBehaviorKeys.UntypedValueAccessor)
         .Append(PropertyBehaviorKeys.Undo, DefaultBehaviorState.Disabled)
         .Append(PropertyBehaviorKeys.ValueValidationSource, DefaultBehaviorState.Disabled)
         .Append(PropertyBehaviorKeys.PropertyChangedNotifier)
         .Append(PropertyBehaviorKeys.SourceAccessor, DefaultBehaviorState.DisabledWithoutFactory)
         .Append(PropertyBehaviorKeys.PropertyDescriptorProvider);

      private static void RegisterViewModelPropertyTemplate() {
         //RegisterTemplate(
         //   BehaviorChainTemplateKeys.ViewModelProperty,
         //   new BehaviorChainTemplate(ViewModelPropertyBehaviorFactory.Instance)
         //      .Append(PropertyBehaviorKeys.DisplayValueAccessor)
         //      .Append(PropertyBehaviorKeys.UntypedValueAccessor)
         //      .Append(PropertyBehaviorKeys.LazyRefresh)
         //      .Append(PropertyBehaviorKeys.Undo, DefaultBehaviorState.Disabled)
         //            // TODO: Rethink.
         //      .Append(PropertyBehaviorKeys.RefreshBehavior, DefaultBehaviorState.DisabledWithoutFactory)

         //      .Append(PropertyBehaviorKeys.DescendantsValidator)

         //      //.Append(PropertyBehaviorKeys.ValueInitializer)

         //      .Append(PropertyBehaviorKeys.ParentSetter)

         //      .Append(PropertyBehaviorKeys.ValueValidationSource, DefaultBehaviorState.Disabled)
         //      .Append(PropertyBehaviorKeys.Validator, DefaultBehaviorState.Disabled)

         //      .Append(PropertyBehaviorKeys.PropertyChangedNotifier)


         //      //.Append(PropertyBehaviorKeys.ViewModelSourceSetter, DefaultBehaviorState.DisabledWithoutFactory)
         //            //.Append(PropertyBehaviorKeys.ValueCache)
         //      .Append(PropertyBehaviorKeys.ViewModelAccessor, DefaultBehaviorState.DisabledWithoutFactory)
         //      .Append(PropertyBehaviorKeys.SourceAccessor, DefaultBehaviorState.DisabledWithoutFactory)
         //      .Append(PropertyBehaviorKeys.ViewModelFactory)
         //      .Append(PropertyBehaviorKeys.PropertyDescriptorProvider)
         //);

         BehaviorChainTemplateRegistry.RegisterTemplate(
            BehaviorChainTemplateKeys.ViewModelProperty,
            new BehaviorChainTemplate(DefaultProviders.ViewModelProperty)
               .Append(PropertyBehaviorKeys.DisplayValueAccessor)
               .Append(PropertyBehaviorKeys.UntypedValueAccessor)
               .Append(PropertyBehaviorKeys.Undo, DefaultBehaviorState.Disabled)
               .Append(PropertyBehaviorKeys.LazyRefresh)
               .Append(PropertyBehaviorKeys.ValueValidationSource, DefaultBehaviorState.Disabled)
               .Append(PropertyBehaviorKeys.DescendantsValidator)
               .Append(PropertyBehaviorKeys.PropertyChangedNotifier)
               .Append(PropertyBehaviorKeys.SourceAccessor, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(PropertyBehaviorKeys.PropertyDescriptorProvider)
         );
      }

      private static void RegisterViewModelWithSourcePropertyTemplate() {
         BehaviorChainTemplateRegistry.RegisterTemplate(
            BehaviorChainTemplateKeys.ViewModelProperty,
            new BehaviorChainTemplate(DefaultProviders.ViewModelProperty)
               .Append(PropertyBehaviorKeys.DisplayValueAccessor)
               .Append(PropertyBehaviorKeys.UntypedValueAccessor)
               .Append(PropertyBehaviorKeys.Undo, DefaultBehaviorState.Disabled)
               .Append(PropertyBehaviorKeys.ValueValidationSource, DefaultBehaviorState.Disabled)
               .Append(PropertyBehaviorKeys.PropertyChangedNotifier)
               .Append(PropertyBehaviorKeys.SourceAccessor, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(PropertyBehaviorKeys.PropertyDescriptorProvider)
         );
      }

      private static void RegisterCollectionPropertyTemplate() {

      }

      private static void RegisterCollectionPropertyWithSourceTemplate() {

      }

      private static void RegisterCommandPropertyTemplate() {

      }



      private static void RegisterViewModelTemplate() {
         BehaviorChainTemplateRegistry.RegisterTemplate(
            BehaviorChainTemplateKeys.ViewModel,
            new BehaviorChainTemplate(DefaultProviders.ViewModel)
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
