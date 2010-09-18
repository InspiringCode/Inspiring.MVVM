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
         var config = new BehaviorConfiguration();

         config.Add(
            VMBehaviorKey.DisplayValueAccessor,
            new DefaultBehaviorFactory(VMBehaviorKey.DisplayValueAccessor),
            RelativePosition.After,
            VMBehaviorKey.Last
         );

         config.Add(
            VMBehaviorKey.PropertyChangedTrigger,
            new DefaultBehaviorFactory(VMBehaviorKey.PropertyChangedTrigger),
            RelativePosition.After,
            VMBehaviorKey.Last
         );

         config.Add(
            VMBehaviorKey.PropertyValueAcessor,
            new DefaultBehaviorFactory(VMBehaviorKey.PropertyValueAcessor),
            RelativePosition.After,
            VMBehaviorKey.Last
         );

         config.Add(
            VMBehaviorKey.SourceValueAccessor,
            new DefaultBehaviorFactory(VMBehaviorKey.SourceValueAccessor),
            RelativePosition.After,
            VMBehaviorKey.Last,
            addLazily: true
         );

         return config;
      }

      private static BehaviorConfiguration CreateCollectionDefault() {
         var config = new BehaviorConfiguration();

         config.Add(
            VMBehaviorKey.DisplayValueAccessor,
            new DefaultBehaviorFactory(VMBehaviorKey.DisplayValueAccessor),
            RelativePosition.After,
            VMBehaviorKey.Last
         );

         config.Add(
            VMBehaviorKey.CollectionValueCache,
            new DefaultBehaviorFactory(VMBehaviorKey.CollectionValueCache),
            RelativePosition.After,
            VMBehaviorKey.Last
         );

         config.Add(
            VMBehaviorKey.CollectionPopulator,
            new DefaultBehaviorFactory(VMBehaviorKey.CollectionPopulator),
            RelativePosition.After,
            VMBehaviorKey.Last
         );

         config.Add(
            VMBehaviorKey.ViewModelFactory,
            new DefaultBehaviorFactory(VMBehaviorKey.ViewModelFactory),
            RelativePosition.After,
            VMBehaviorKey.Last
         );

         config.Add(
            VMBehaviorKey.SourceValueAccessor,
            new DefaultBehaviorFactory(VMBehaviorKey.SourceValueAccessor),
            RelativePosition.After,
            VMBehaviorKey.Last
         );

         config.Add(
            VMBehaviorKey.CollectionInstanceCache,
            new DefaultBehaviorFactory(VMBehaviorKey.CollectionInstanceCache),
            RelativePosition.After,
            VMBehaviorKey.Last
         );

         config.Add(
            VMBehaviorKey.CollectionFactory,
            new DefaultBehaviorFactory(VMBehaviorKey.CollectionFactory),
            RelativePosition.After,
            VMBehaviorKey.Last
         );


         return config;
      }

      private static BehaviorConfiguration CreateViewModelPropertyDefault() {
         var config = new BehaviorConfiguration();

         config.Add(
            VMBehaviorKey.ViewModelValueCache,
            new DefaultBehaviorFactory(VMBehaviorKey.ViewModelValueCache),
            RelativePosition.After,
            VMBehaviorKey.Last
         );

         config.Add(
            VMBehaviorKey.ViewModelPropertyInitializer,
            new DefaultBehaviorFactory(VMBehaviorKey.ViewModelPropertyInitializer),
            RelativePosition.After,
            VMBehaviorKey.Last
         );

         config.Add(
            VMBehaviorKey.ViewModelFactory,
            new DefaultBehaviorFactory(VMBehaviorKey.ViewModelFactory),
            RelativePosition.After,
            VMBehaviorKey.Last
         );

         config.Add(
            VMBehaviorKey.SourceValueAccessor,
            new DefaultBehaviorFactory(VMBehaviorKey.SourceValueAccessor),
            RelativePosition.After,
            VMBehaviorKey.Last
         );

         return config;
      }

      private static BehaviorConfiguration CreateCommandPropertyDefault() {
         var config = new BehaviorConfiguration();

         config.Add(
            VMBehaviorKey.CommandValueCache,
            new DefaultBehaviorFactory(VMBehaviorKey.CommandValueCache),
            RelativePosition.After,
            VMBehaviorKey.Last
         );

         config.Add(
            VMBehaviorKey.PropertyValueAcessor,
            new DefaultBehaviorFactory(VMBehaviorKey.PropertyValueAcessor),
            RelativePosition.After,
            VMBehaviorKey.Last
         );

         config.Add(
            VMBehaviorKey.SourceValueAccessor,
            new DefaultBehaviorFactory(VMBehaviorKey.SourceValueAccessor),
            RelativePosition.After,
            VMBehaviorKey.Last,
            addLazily: true
         );

         return config;
      }
   }
}
