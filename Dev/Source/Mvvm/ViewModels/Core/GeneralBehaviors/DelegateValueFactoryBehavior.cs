namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.Common;

   internal sealed class DelegateValueFactoryBehavior<TVM, TSource, TValue> :
      Behavior,
      IValueFactoryBehavior<TValue> {

      private readonly Func<IBehaviorContext, TVM, TSource, TValue> _factoryMethod;
      private readonly PropertyPath<TVM, TSource> _sourceObjectPath;

      public DelegateValueFactoryBehavior(
         PropertyPath<TVM, TSource> sourceObjectPath,
         Func<IBehaviorContext, TVM, TSource, TValue> factoryMethod
      ) {
         Contract.Requires(sourceObjectPath != null);
         Contract.Requires(factoryMethod != null);

         _sourceObjectPath = sourceObjectPath;
         _factoryMethod = factoryMethod;
      }

      public TValue CreateValue(IBehaviorContext context) {
         var sourceObject = _sourceObjectPath.GetValue((TVM)context.VM);
         return _factoryMethod(context, (TVM)context.VM, sourceObject);
      }
   }
}
