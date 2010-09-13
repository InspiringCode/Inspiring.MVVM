namespace Inspiring.Mvvm.ViewModels.Core {

   public static class BehaviorConfigurationFactory {
      private static BehaviorConfiguration _propertyDefault;
      private static BehaviorConfiguration _collectionDefault;
      private static BehaviorConfiguration _viewModelPropertyDefault;

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

      public static void OverrideDefaultConfiguration(BehaviorConfiguration configuration) {
         _propertyDefault = configuration.Clone();
      }

      public static void OverrideDefaultCollectionConfiguration(BehaviorConfiguration configuration) {
         _collectionDefault = configuration.Clone();
      }

      public static void OverrideDefaultViewModelPropertyConfiguration(BehaviorConfiguration configuration) {
         _viewModelPropertyDefault = configuration.Clone();
      }

      public static void Reset() {
         _propertyDefault = CreatePropertyDefault();
         _collectionDefault = CreateCollectionDefault();
         _viewModelPropertyDefault = CreateViewModelPropertyDefault();
      }

      private static BehaviorConfiguration CreatePropertyDefault() {
         var config = new BehaviorConfiguration();

         config.Add(
            VMBehaviorKey.DisplayValueAccessor,
            new DefaultBehaviorFactory(VMBehaviorKey.DisplayValueAccessor),
            BehaviorOrderModifier.After,
            VMBehaviorKey.Last
         );

         config.Add(
            VMBehaviorKey.PropertyValueAcessor,
            new DefaultBehaviorFactory(VMBehaviorKey.PropertyValueAcessor),
            BehaviorOrderModifier.After,
            VMBehaviorKey.Last
         );

         config.Add(
            VMBehaviorKey.SourceValueAccessor,
            new DefaultBehaviorFactory(VMBehaviorKey.SourceValueAccessor),
            BehaviorOrderModifier.After,
            VMBehaviorKey.Last,
            addLazily: true
         );

         return config;
      }

      private static BehaviorConfiguration CreateCollectionDefault() {
         var config = new BehaviorConfiguration();

         config.Add(
            VMBehaviorKey.CollectionValueCache,
            new DefaultBehaviorFactory(VMBehaviorKey.CollectionValueCache),
            BehaviorOrderModifier.After,
            VMBehaviorKey.Last
         );

         config.Add(
            VMBehaviorKey.CollectionPopulator,
            new DefaultBehaviorFactory(VMBehaviorKey.CollectionPopulator),
            BehaviorOrderModifier.After,
            VMBehaviorKey.Last
         );

         config.Add(
            VMBehaviorKey.SourceValueAccessor,
            new DefaultBehaviorFactory(VMBehaviorKey.SourceValueAccessor),
            BehaviorOrderModifier.After,
            VMBehaviorKey.Last
         );

         config.Add(
            VMBehaviorKey.CollectionInstanceCache,
            new DefaultBehaviorFactory(VMBehaviorKey.CollectionInstanceCache),
            BehaviorOrderModifier.After,
            VMBehaviorKey.Last
         );

         config.Add(
            VMBehaviorKey.CollectionFactory,
            new DefaultBehaviorFactory(VMBehaviorKey.CollectionFactory),
            BehaviorOrderModifier.After,
            VMBehaviorKey.Last
         );


         return config;
      }

      private static BehaviorConfiguration CreateViewModelPropertyDefault() {
         var config = new BehaviorConfiguration();

         config.Add(
            VMBehaviorKey.ViewModelValueCache,
            new DefaultBehaviorFactory(VMBehaviorKey.ViewModelValueCache),
            BehaviorOrderModifier.After,
            VMBehaviorKey.Last
         );

         config.Add(
            VMBehaviorKey.ViewModelFactory,
            new DefaultBehaviorFactory(VMBehaviorKey.ViewModelFactory),
            BehaviorOrderModifier.After,
            VMBehaviorKey.Last
         );

         config.Add(
            VMBehaviorKey.SourceValueAccessor,
            new DefaultBehaviorFactory(VMBehaviorKey.SourceValueAccessor),
            BehaviorOrderModifier.After,
            VMBehaviorKey.Last
         );

         return config;
      }
   }
}
