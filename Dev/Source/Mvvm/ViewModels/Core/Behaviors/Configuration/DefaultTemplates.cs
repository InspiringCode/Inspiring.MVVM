namespace Inspiring.Mvvm.ViewModels.Core {

   internal class DefaultTemplates {
      public static void RegisterDefaultTemplates() {
         RegisterPropertyTemplate();
         RegisterPropertyWithSourceTemplate();
         RegisterViewModelPropertyTemplate();
         RegisterViewModelWithSourcePropertyTemplate();
         RegisterCollectionPropertyTemplate();
         RegisterCollectionPropertyWithSourceTemplate();
         RegisterCommandPropertyTemplate();

         RegisterViewModelTemplate();
      }

      private static void RegisterPropertyTemplate() {
         BehaviorChainTemplateRegistry.RegisterTemplate(
            DefaultBehaviorChainTemplateKeys.Property,
            new BehaviorChainTemplate(BehaviorFactoryProviders.SimpleProperty)
               .Append(PropertyBehaviorKeys.DisplayValueAccessor)
               .Append(PropertyBehaviorKeys.UntypedValueAccessor)
               .Append(PropertyBehaviorKeys.Undo, DefaultBehaviorState.Disabled)
               .Append(PropertyBehaviorKeys.ValueValidationSource, DefaultBehaviorState.Disabled)
               .Append(PropertyBehaviorKeys.ChangeNotifier)
               .Append(PropertyBehaviorKeys.ValueAccessor, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(PropertyBehaviorKeys.PropertyDescriptorProvider)
         );
      }

      private static void RegisterPropertyWithSourceTemplate() {
         BehaviorChainTemplateRegistry.RegisterTemplate(
            DefaultBehaviorChainTemplateKeys.PropertyWithSource,
            new BehaviorChainTemplate(BehaviorFactoryProviders.SimpleProperty)
               .Append(PropertyBehaviorKeys.DisplayValueAccessor)
               .Append(PropertyBehaviorKeys.UntypedValueAccessor)
               .Append(PropertyBehaviorKeys.Undo, DefaultBehaviorState.Disabled)
               .Append(PropertyBehaviorKeys.ValueValidationSource, DefaultBehaviorState.Disabled)
               .Append(PropertyBehaviorKeys.ChangeNotifier)
               .Append(PropertyBehaviorKeys.ValueAccessor, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(PropertyBehaviorKeys.PropertyDescriptorProvider)
         );
      }

      private static void RegisterViewModelPropertyTemplate() {
         BehaviorChainTemplateRegistry.RegisterTemplate(
            DefaultBehaviorChainTemplateKeys.ViewModelProperty,
            new BehaviorChainTemplate(BehaviorFactoryProviders.ViewModelProperty)
               .Append(PropertyBehaviorKeys.DisplayValueAccessor)
               .Append(PropertyBehaviorKeys.UntypedValueAccessor)
               .Append(PropertyBehaviorKeys.ValueInitializer)
               .Append(PropertyBehaviorKeys.Undo, DefaultBehaviorState.Disabled)
               .Append(PropertyBehaviorKeys.LazyRefresh)
               .Append(PropertyBehaviorKeys.ValueValidationSource, DefaultBehaviorState.Disabled)
               .Append(PropertyBehaviorKeys.DescendantsValidator)
               .Append(PropertyBehaviorKeys.ChangeNotifier)
               .Append(PropertyBehaviorKeys.ValueAccessor, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(PropertyBehaviorKeys.SourceAccessor, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(PropertyBehaviorKeys.PropertyDescriptorProvider)
         );
      }

      private static void RegisterViewModelWithSourcePropertyTemplate() {
         BehaviorChainTemplateRegistry.RegisterTemplate(
            DefaultBehaviorChainTemplateKeys.ViewModelPropertyWithSource,
            new BehaviorChainTemplate(BehaviorFactoryProviders.ViewModelProperty)
               .Append(PropertyBehaviorKeys.DisplayValueAccessor)
               .Append(PropertyBehaviorKeys.UntypedValueAccessor)
               .Append(PropertyBehaviorKeys.ValueInitializer)
               .Append(PropertyBehaviorKeys.Undo, DefaultBehaviorState.Disabled)
               .Append(PropertyBehaviorKeys.LazyRefresh)
               .Append(PropertyBehaviorKeys.ValueValidationSource, DefaultBehaviorState.Disabled)
               .Append(PropertyBehaviorKeys.DescendantsValidator)
               .Append(PropertyBehaviorKeys.ChangeNotifier)
               .Append(PropertyBehaviorKeys.ValueAccessor, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(PropertyBehaviorKeys.SourceAccessor, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(PropertyBehaviorKeys.ValueFactory)
               .Append(PropertyBehaviorKeys.PropertyDescriptorProvider)
         );
      }

      private static void RegisterCollectionPropertyTemplate() {
         BehaviorChainTemplateRegistry.RegisterTemplate(
            DefaultBehaviorChainTemplateKeys.CollectionProperty,
            new BehaviorChainTemplate(BehaviorFactoryProviders.CollectionProperty)
               .Append(PropertyBehaviorKeys.DisplayValueAccessor)
               .Append(PropertyBehaviorKeys.UntypedValueAccessor)
               .Append(PropertyBehaviorKeys.ValueInitializer)
               .Append(PropertyBehaviorKeys.Undo, DefaultBehaviorState.Disabled)
               .Append(PropertyBehaviorKeys.LazyRefresh)
               .Append(PropertyBehaviorKeys.ValueValidationSource)
               .Append(PropertyBehaviorKeys.DescendantsValidator)
               .Append(PropertyBehaviorKeys.ChangeNotifier)
               .Append(PropertyBehaviorKeys.ValueAccessor, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(PropertyBehaviorKeys.SourceAccessor, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(PropertyBehaviorKeys.ValueFactory)
               .Append(PropertyBehaviorKeys.PropertyDescriptorProvider)
               .Append(CollectionPropertyBehaviorKeys.ItemDescriptorProvider, DefaultBehaviorState.DisabledWithoutFactory)
         );
      }

      private static void RegisterCollectionPropertyWithSourceTemplate() {
         BehaviorChainTemplateRegistry.RegisterTemplate(
            DefaultBehaviorChainTemplateKeys.CollectionPropertyWithSource,
            new BehaviorChainTemplate(BehaviorFactoryProviders.CollectionProperty)
               .Append(PropertyBehaviorKeys.DisplayValueAccessor)
               .Append(PropertyBehaviorKeys.UntypedValueAccessor)
               .Append(PropertyBehaviorKeys.ValueInitializer)
               .Append(PropertyBehaviorKeys.Undo, DefaultBehaviorState.Disabled)
               .Append(PropertyBehaviorKeys.LazyRefresh)
               .Append(CollectionPropertyBehaviorKeys.Synchronizer)
               .Append(PropertyBehaviorKeys.ValueValidationSource, DefaultBehaviorState.Disabled)
               .Append(PropertyBehaviorKeys.DescendantsValidator)
               .Append(PropertyBehaviorKeys.ChangeNotifier)
               .Append(PropertyBehaviorKeys.ValueAccessor, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(PropertyBehaviorKeys.SourceAccessor, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(PropertyBehaviorKeys.ValueFactory)
               .Append(CollectionPropertyBehaviorKeys.ItemFactory)
               .Append(PropertyBehaviorKeys.PropertyDescriptorProvider)
               .Append(CollectionPropertyBehaviorKeys.ItemDescriptorProvider, DefaultBehaviorState.DisabledWithoutFactory)
         );
      }

      private static void RegisterCommandPropertyTemplate() {
         BehaviorChainTemplateRegistry.RegisterTemplate(
            DefaultBehaviorChainTemplateKeys.CommandProperty,
            new BehaviorChainTemplate(BehaviorFactoryProviders.CommandProperty)
               .Append(PropertyBehaviorKeys.DisplayValueAccessor)
               .Append(PropertyBehaviorKeys.UntypedValueAccessor)
               .Append(PropertyBehaviorKeys.ValueAccessor)
               .Append(CommandPropertyBehaviorKeys.WaitCursor)
               .Append(CommandPropertyBehaviorKeys.CommandExecutor, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(PropertyBehaviorKeys.SourceAccessor, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(PropertyBehaviorKeys.PropertyDescriptorProvider)
         );
      }

      private static void RegisterViewModelTemplate() {
         BehaviorChainTemplateRegistry.RegisterTemplate(
            DefaultBehaviorChainTemplateKeys.ViewModel,
            new BehaviorChainTemplate(BehaviorFactoryProviders.ViewModel)
               .Append(ViewModelBehaviorKeys.LoadOrderController)
               .Append(ViewModelBehaviorKeys.ViewModelValidationSource, DefaultBehaviorState.Disabled)
               .Append(ViewModelBehaviorKeys.ValidationExecutor, DefaultBehaviorState.Disabled)
               .Append(ViewModelBehaviorKeys.ValidationResultAggregatorCache)
               .Append(ViewModelBehaviorKeys.ValidationResultAggregator)
               .Append(ViewModelBehaviorKeys.TypeDescriptorProvider)
               .Append(ViewModelBehaviorKeys.UndoRoot, DefaultBehaviorState.Disabled)
         );
      }
   }
}
