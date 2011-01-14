namespace Inspiring.Mvvm.ViewModels.Core {
   public static partial class BehaviorChainTemplateRegistry {
      static BehaviorChainTemplateRegistry() {
         RegisterDefaultTemplates();
      }

      private static void RegisterDefaultTemplates() {
         RegisterViewModelTemplate();
         RegisterPropertyTemplate();
         RegisterCollectionPropertyTemplate();
         RegisterCommandPropertyTemplate();
         RegisterViewModelPropertyTemplate();
         RegisterCollectionBehaviorsTemplate();
      }

      private static void RegisterViewModelTemplate() {
         RegisterTemplate(
            BehaviorChainTemplateKeys.ViewModel,
            new BehaviorChainTemplate(ViewModelBehaviorFactory.Instance)
               .Append(BehaviorKeys.ManualUpdateCoordinator)
               .Append(BehaviorKeys.Validator, DefaultBehaviorState.Disabled)
               .Append(BehaviorKeys.TypeDescriptor)
         );
      }

      private static void RegisterPropertyTemplate() {
         RegisterTemplate(
            BehaviorChainTemplateKeys.Property,
            new BehaviorChainTemplate(PropertyBehaviorFactory.Instance)
               .Append(BehaviorKeys.InvalidDisplayValueCache, DefaultBehaviorState.Disabled)
               .Append(BehaviorKeys.DisplayValueAccessor)
               .Append(BehaviorKeys.ManualUpdateBehavior, DefaultBehaviorState.Disabled)
               .Append(BehaviorKeys.PreValidationValueCache, DefaultBehaviorState.Disabled)
               .Append(BehaviorKeys.Validator, DefaultBehaviorState.Disabled)
               .Append(BehaviorKeys.PropertyChangedTrigger)
               .Append(BehaviorKeys.PropertyValueCache, DefaultBehaviorState.DisabledWithoutFactory) // TODO!
            //.Append(BehaviorKeys.ManualUpdateBehavior) // TODO: Is this correct?
               .Append(BehaviorKeys.SourceAccessor, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(BehaviorKeys.TypeDescriptor)
         );
      }

      private static void RegisterCollectionPropertyTemplate() {
         RegisterTemplate(
            BehaviorChainTemplateKeys.CollectionProperty,
            new BehaviorChainTemplate(PropertyBehaviorFactory.Instance)
               .Append(BehaviorKeys.DisplayValueAccessor)
               .Append(BehaviorKeys.ManualUpdateBehavior, DefaultBehaviorState.DisabledWithoutFactory)

               // TODO: Rethink these two behaviors.
               .Append(BehaviorKeys.PreValidationValueCache, DefaultBehaviorState.Disabled)
               .Append(BehaviorKeys.Validator, DefaultBehaviorState.Disabled)

               .Append(BehaviorKeys.DescendantValidator)

               .Append(BehaviorKeys.IsLoadedIndicator)
               .Append(BehaviorKeys.CollectionPopulator, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(BehaviorKeys.ValueCache)
               .Append(BehaviorKeys.CollectionFactory, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(BehaviorKeys.TypeDescriptor)
         );
      }

      private static void RegisterCommandPropertyTemplate() {
         RegisterTemplate(
            BehaviorChainTemplateKeys.CommandProperty,
            new BehaviorChainTemplate(PropertyBehaviorFactory.Instance)
               .Append(BehaviorKeys.DisplayValueAccessor)
               .Append(BehaviorKeys.ValueCache)
               .Append(BehaviorKeys.SourceAccessor, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(BehaviorKeys.TypeDescriptor)
         );
      }

      private static void RegisterViewModelPropertyTemplate() {
         RegisterTemplate(
            BehaviorChainTemplateKeys.ViewModelProperty,
            new BehaviorChainTemplate(ViewModelPropertyBehaviorFactory.Instance)
               .Append(BehaviorKeys.DisplayValueAccessor)
            // TODO: Rethink.
               .Append(BehaviorKeys.ManualUpdateBehavior, DefaultBehaviorState.DisabledWithoutFactory)

               .Append(BehaviorKeys.ParentSetter)

               .Append(BehaviorKeys.PreValidationValueCache, DefaultBehaviorState.Disabled)
               .Append(BehaviorKeys.Validator, DefaultBehaviorState.Disabled)

               .Append(BehaviorKeys.DescendantValidator)

               .Append(BehaviorKeys.IsLoadedIndicator)
               .Append(BehaviorKeys.PropertyChangedTrigger)
               .Append(BehaviorKeys.ValueCache, DefaultBehaviorState.Disabled)

               .Append(BehaviorKeys.ParentInitializer)
               .Append(BehaviorKeys.ViewModelAccessor, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(BehaviorKeys.ViewModelFactory, DefaultBehaviorState.Disabled)
               .Append(BehaviorKeys.SourceAccessor, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(BehaviorKeys.TypeDescriptor)
         );
      }

      private static void RegisterCollectionBehaviorsTemplate() {
         RegisterTemplate(
            BehaviorChainTemplateKeys.DefaultCollectionBehaviors,
            new BehaviorChainTemplate(CollectionBehaviorFactory.Instance)
               .Append(CollectionBehaviorKeys.DescriptorSetter, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(CollectionBehaviorKeys.ParentSetter)
               .Append(CollectionBehaviorKeys.ChangeNotifier)
               .Append(CollectionBehaviorKeys.SourceSynchronizer, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(CollectionBehaviorKeys.Populator, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(CollectionBehaviorKeys.SourceAccessor, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(CollectionBehaviorKeys.ViewModelFactory, DefaultBehaviorState.Disabled)
         );
      }

      ///// <summary>
      /////   Appends the behavior key with the <see cref="PropertyBehaviorFactory"/>.
      ///// </summary>
      //private static void Append(
      //   BehaviorChainTemplate template,
      //   BehaviorKey key,
      //   bool disabled = false,
      //   bool withoutFactory = false
      //) {
      //   Contract.Requires(withoutFactory ? disabled : true);

      //   if (withoutFactory) {
      //      template.Append(key);
      //   } else {
      //      template.Append(key, new PropertyBehaviorFactory(key), disabled);
      //   }
      //}

      ///// <summary>
      /////   Appends the behavior key with the <see cref="ViewModelBehaviorFactory"/>.
      ///// </summary>
      //private static void AppendViewModel(
      //   BehaviorChainTemplate template,
      //   BehaviorKey key,
      //   bool disabled = false
      //) {
      //   template.Append(key, new ViewModelBehaviorFactory(key), disabled);
      //}
   }
}
