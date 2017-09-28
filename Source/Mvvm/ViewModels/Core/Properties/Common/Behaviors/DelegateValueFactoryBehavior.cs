namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using Inspiring.Mvvm.Common;

   // TODO: Throw this away? Currently not used...
   internal sealed class DelegateValueFactoryBehavior<TVM, TSource, TValue> :
      Behavior,
      IValueFactoryBehavior<TValue> {

      private readonly Func<IBehaviorContext, TVM, TSource, TValue> _factoryMethod;
      private readonly PropertyPath<TVM, TSource> _sourceObjectPath;

      public DelegateValueFactoryBehavior(
         PropertyPath<TVM, TSource> sourceObjectPath,
         Func<IBehaviorContext, TVM, TSource, TValue> factoryMethod
      ) {
         Check.NotNull(sourceObjectPath, nameof(sourceObjectPath));
         Check.NotNull(factoryMethod, nameof(factoryMethod));

         _sourceObjectPath = sourceObjectPath;
         _factoryMethod = factoryMethod;
      }

      public TValue CreateValue(IBehaviorContext context) {
         var sourceObject = _sourceObjectPath.GetValue((TVM)context.VM);
         return _factoryMethod(context, (TVM)context.VM, sourceObject);
      }
   }
}
