namespace Inspiring.Mvvm.ViewModels.Core {

   public static class BehaviorConfigurationFactory {
      private static BehaviorConfiguration _propertyDefault;
      private static BehaviorConfiguration _collectionDefault;
      private static BehaviorConfiguration _viewModelPropertyDefault;
      private static BehaviorConfiguration _commandPropertyDefault;

      static BehaviorConfigurationFactory() {
         Reset();
      }

      public static BehaviorConfiguration CreateConfiguration() {
         return _propertyDefault.Clone();
      }

      public static BehaviorConfiguration CreateCollectionConfiguration() {
         return _collectionDefault.Clone();
      }

      public static BehaviorConfiguration CreateViewModelPropertyConfiguration() {
         return _viewModelPropertyDefault.Clone();
      }

      public static BehaviorConfiguration CreateCommandPropertyConfiguration() {
         return _commandPropertyDefault.Clone();
      }

      public static void OverrideDefaultConfiguration(BehaviorConfiguration configuration) {
         _propertyDefault = configuration.Clone();
      }

      public static void OverrideDefaultCollectionConfiguration(BehaviorConfiguration configuration) {
         _collectionDefault = configuration.Clone();
      }

      public static void OverrideDefaultViewModelPropertyConfiguration(BehaviorConfiguration configuration) {
         _viewModelPropertyDefault = configuration.Clone();
      }

      public static void OverrideCommandPropertyConfiguration(BehaviorConfiguration configuration) {
         _commandPropertyDefault = configuration.Clone();
      }

      public static void Reset() {
         _propertyDefault = CreatePropertyDefault();
         _collectionDefault = CreateCollectionDefault();
         _viewModelPropertyDefault = CreateViewModelPropertyDefault();
         _commandPropertyDefault = CreateCommandPropertyDefault();
      }

      private static BehaviorConfiguration CreatePropertyDefault() {
         return new BehaviorConfiguration()
            .Append(VMBehaviorKey.InvalidDisplayValueCache, disabled: true)
            .Append(VMBehaviorKey.DisplayValueAccessor)
            .Append(VMBehaviorKey.Validator, disabled: true)
            .Append(VMBehaviorKey.PropertyChangedTrigger)
            .Append(VMBehaviorKey.PropertyValueCache, disabled: true)
            .Append(VMBehaviorKey.PropertyValueAcessor)
            .Append(VMBehaviorKey.SourceValueAccessor, disabled: true);
      }

      private static BehaviorConfiguration CreateCollectionDefault() {
         return new BehaviorConfiguration()
            .Append(VMBehaviorKey.DisplayValueAccessor)
            .Append(VMBehaviorKey.CollectionValueCache)
            .Append(VMBehaviorKey.CollectionPopulator)
            .Append(VMBehaviorKey.ViewModelFactory)
            .Append(VMBehaviorKey.SourceValueAccessor)
            .Append(VMBehaviorKey.CollectionInstanceCache)
            .Append(VMBehaviorKey.CollectionFactory);
      }

      private static BehaviorConfiguration CreateViewModelPropertyDefault() {
         return new BehaviorConfiguration()
            .Append(VMBehaviorKey.DisplayValueAccessor)
            .Append(VMBehaviorKey.ViewModelValueCache)
            .Append(VMBehaviorKey.ViewModelPropertyInitializer)
            .Append(VMBehaviorKey.ViewModelFactory)
            .Append(VMBehaviorKey.SourceValueAccessor);
      }

      private static BehaviorConfiguration CreateCommandPropertyDefault() {
         return new BehaviorConfiguration()
            .Append(VMBehaviorKey.DisplayValueAccessor)
            .Append(VMBehaviorKey.CommandValueCache)
            .Append(VMBehaviorKey.PropertyValueAcessor)
            .Append(VMBehaviorKey.SourceValueAccessor, disabled: true);
      }
   }
}
