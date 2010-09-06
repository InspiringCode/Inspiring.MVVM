namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Linq.Expressions;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.ViewModels.Behaviors;

   /// <inheritdoc/>
   internal class VMPropertyFactory<TVM, TSource> : IVMPropertyFactory<TSource>, IRootVMPropertyFactory<TSource> where TVM : ViewModel {
      private PropertyPath<TVM, TSource> _sourceObjectPath;

      /// <param name="sourceObjectPath">
      ///   Pass 'PropertyPath.Empty&lt;TVM&gt;' if you want to create a root
      ///   factory for the VM.
      /// </param>
      public VMPropertyFactory(
         PropertyPath<TVM, TSource> sourceObjectPath
      ) {
         _sourceObjectPath = sourceObjectPath;
      }

      public VMProperty<T> Mapped<T>(Expression<Func<TSource, T>> sourcePropertySelector) {
         PropertyPath<TSource, T> sourcePropertyPath = PropertyPath.Create(sourcePropertySelector);
         PropertyPath<TVM, T> path = PropertyPath.Concat(_sourceObjectPath, sourcePropertyPath);

         IAccessPropertyBehavior<T> accessBehavior = new MappedPropertyBehavior<TVM, T>(path);
         return new VMProperty<T>(accessBehavior);
      }

      public VMProperty<T> Calculated<T>(Func<TSource, T> getter, Action<TSource, T> setter = null) {
         IAccessPropertyBehavior<TSource> sourceAccessBehavior = _sourceObjectPath.IsEmpty ?
            null :
            new MappedPropertyBehavior<TVM, TSource>(_sourceObjectPath);

         IAccessPropertyBehavior<T> accessBehavior = new CalculatedPropertyBehavior<TSource, T>(
            getter,
            setter,
            sourceAccessBehavior
         );

         return new VMProperty<T>(accessBehavior);
      }

      public VMProperty<TValue> Simple<TValue>() {
         IAccessPropertyBehavior<TValue> accessBehavior = new InstancePropertyBehavior<TValue>();
         return new VMProperty<TValue>(accessBehavior);
      }
   }
}
