namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.Linq.Expressions;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.ViewModels.Core;

   /// <inheritdoc/>
   internal class VMPropertyFactory<TVM, TSource> : IVMPropertyFactory<TSource>, IRootVMPropertyFactory<TSource> where TVM : ViewModel {
      private PropertyPath<TVM, TSource> _sourceObjectPropertyPath;
      private BehaviorConfigurationDictionary _configurations;

      /// <param name="sourceObjectPath">
      ///   Pass 'PropertyPath.Empty{TVM}' if you want to create a root factory
      ///   for the VM.
      /// </param>
      public VMPropertyFactory(
         PropertyPath<TVM, TSource> sourceObjectPath,
         BehaviorConfigurationDictionary configurations
      ) {
         _sourceObjectPropertyPath = sourceObjectPath;
         _configurations = configurations;
      }

      /// <inheritdoc/>
      public VMProperty<T> Mapped<T>(Expression<Func<TSource, T>> sourcePropertySelector) {
         var property = new VMProperty<T>();
         var config = BehaviorConfigurationFactory.CreateConfiguration();

         var propertyPath = PropertyPath.Concat(
            _sourceObjectPropertyPath,
            PropertyPath.Create(sourcePropertySelector)
         );

         config.OverridePermanently(
            behavior: VMBehaviorKey.PropertyValueAcessor,
            withBehavior: new ConstantBehaviorFactory(
               new MappedPropertyBehavior<TVM, T>(propertyPath)
            )
         );

         _configurations.Add(property, config);
         return property;
      }

      /// <inheritdoc/>
      public VMProperty<T> Calculated<T>(Func<TSource, T> getter, Action<TSource, T> setter = null) {
         var property = new VMProperty<T>();
         var config = BehaviorConfigurationFactory.CreateConfiguration();

         if (!_sourceObjectPropertyPath.IsEmpty) {
            config.OverridePermanently(
               behavior: VMBehaviorKey.SourceValueAccessor,
               withBehavior: new ConstantBehaviorFactory(
                  new MappedPropertyBehavior<TVM, TSource>(_sourceObjectPropertyPath)
               )
            );
         }

         config.OverridePermanently(
            behavior: VMBehaviorKey.PropertyValueAcessor,
            withBehavior: new ConstantBehaviorFactory(
               new CalculatedPropertyBehavior<TSource, T>(getter, setter)
            )
         );

         _configurations.Add(property, config);
         return property;
      }

      /// <inheritdoc/>
      public VMProperty<TValue> Local<TValue>() {
         VMProperty<TValue> property = new VMProperty<TValue>();
         BehaviorConfiguration config = BehaviorConfigurationFactory.CreateConfiguration();

         config.OverridePermanently(
            behavior: VMBehaviorKey.PropertyValueAcessor,
            withBehavior: new ConstantBehaviorFactory(
               new InstancePropertyBehavior<TValue>()
            )
         );

         _configurations.Add(property, config);
         return property;
      }

      /// <inheritdoc/>
      public IVMCollectionPropertyFactoryExpression<TItem> MappedCollection<TItem>(
         Expression<Func<TSource, IEnumerable<TItem>>> sourceCollectionSelector
      ) {
         var config = BehaviorConfigurationFactory.CreateCollectionConfiguration();

         PropertyPath<TVM, IEnumerable<TItem>> sourceCollectionPropertyPath = PropertyPath.Concat(
            _sourceObjectPropertyPath,
            PropertyPath.Create(sourceCollectionSelector)
         );

         config.OverridePermanently(
            behavior: VMBehaviorKey.SourceValueAccessor,
            withBehavior: new ConstantBehaviorFactory(
               new MappedPropertyBehavior<TVM, IEnumerable<TItem>>(sourceCollectionPropertyPath)
            )
         );

         return new VMCollectionPropertyFactoryExpression<TItem>(config, _configurations);
      }

      private class VMCollectionPropertyFactoryExpression<TItem> : IVMCollectionPropertyFactoryExpression<TItem> {
         private BehaviorConfiguration _config;
         private BehaviorConfigurationDictionary _configurations;

         public VMCollectionPropertyFactoryExpression(
            BehaviorConfiguration config,
            BehaviorConfigurationDictionary configurations
         ) {
            _config = config;
            _configurations = configurations;
         }

         public VMCollectionProperty<TVM> Of<TVM>() where TVM : ICanInitializeFrom<TItem> {
            var property = new VMCollectionProperty<TVM>();

            _config.OverridePermanently(
               behavior: VMBehaviorKey.CollectionPopulator,
               withBehavior: new ConstantBehaviorFactory(
                  new CollectionPopulatorBehavior<TVM, TItem>()
               )
            );

            _configurations.Add(property, _config);
            return property;
         }
      }
   }
}
