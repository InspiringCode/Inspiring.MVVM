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
               .Append(PropertyBehaviorKeys.ChangeNotifier)
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
               .Append(PropertyBehaviorKeys.ChangeNotifier)
               .Append(PropertyBehaviorKeys.ValueAccessor, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(PropertyBehaviorKeys.PropertyDescriptorProvider)
         );
      }

      private static void RegisterViewModelPropertyTemplate() {
         BehaviorChainTemplateRegistry.RegisterTemplate(
            BehaviorChainTemplateKeys.ViewModelProperty,
            new BehaviorChainTemplate(DefaultProviders.ViewModelProperty)
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
            BehaviorChainTemplateKeys.ViewModelProperty,
            new BehaviorChainTemplate(DefaultProviders.ViewModelProperty)
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
            BehaviorChainTemplateKeys.CollectionProperty,
            new BehaviorChainTemplate(null)
               .Append(PropertyBehaviorKeys.DisplayValueAccessor)
               .Append(PropertyBehaviorKeys.UntypedValueAccessor)
               .Append(PropertyBehaviorKeys.ValueInitializer)
               .Append(PropertyBehaviorKeys.Undo, DefaultBehaviorState.Disabled)
               .Append(CollectionBehaviorKeys.SourceSynchronizer)
               .Append(PropertyBehaviorKeys.ValueValidationSource, DefaultBehaviorState.Disabled)
               .Append(PropertyBehaviorKeys.ChangeNotifier)
               .Append(PropertyBehaviorKeys.ValueAccessor, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(PropertyBehaviorKeys.SourceAccessor, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(PropertyBehaviorKeys.PropertyDescriptorProvider)
         );
      }

      private static void RegisterCollectionPropertyWithSourceTemplate() {

      }

      /*
       * 
       *                .Append(PropertyBehaviorKeys.DisplayValueAccessor)
               .Append(PropertyBehaviorKeys.UntypedValueAccessor)
               .Append(PropertyBehaviorKeys.ValueCache)
               .Append(PropertyBehaviorKeys.CommandFactory, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(PropertyBehaviorKeys.PropertyDescriptorProvider)
       * 
       *                .Append(PropertyBehaviorKeys.WaitCursor)
               .Append(PropertyBehaviorKeys.CommandExecutor, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(PropertyBehaviorKeys.SourceAccessor, DefaultBehaviorState.DisabledWithoutFactory)
       * 
       * 
       * 
       */


      private static void RegisterCommandPropertyTemplate() {
         BehaviorChainTemplateRegistry.RegisterTemplate(
            BehaviorChainTemplateKeys.CommandProperty,
            new BehaviorChainTemplate(DefaultProviders.SimpleProperty)
               .Append(PropertyBehaviorKeys.DisplayValueAccessor)
               .Append(PropertyBehaviorKeys.UntypedValueAccessor)
               .Append(PropertyBehaviorKeys.ValueAccessor, DefaultBehaviorState.DisabledWithoutFactory)
               // cursor
               // executor
               .Append(PropertyBehaviorKeys.SourceAccessor)
               .Append(PropertyBehaviorKeys.PropertyDescriptorProvider)
         );
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
