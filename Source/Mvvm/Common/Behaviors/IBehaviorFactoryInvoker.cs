namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using Inspiring.Mvvm.Common;

   public interface IBehaviorFactoryConfiguration {
      IBehaviorFactory GetFactory(IBehaviorFactoryProvider factoryProvider);
   }

   public abstract class BehaviorFactoryInvoker {
      public abstract IBehavior Invoke(IBehaviorFactoryProvider factory, BehaviorKey behaviorToCreate);

      protected TFactory CastFactory<TFactory>(
         IBehaviorFactoryProvider factory,
         BehaviorKey behaviorToCreate
      ) where TFactory : IBehaviorFactoryProvider {
         if (factory is TFactory) {
            return (TFactory)factory;
         }

         throw new ArgumentException(
            ExceptionTexts.WrongBehaviorFactoryType.FormatWith(
               behaviorToCreate,
               typeof(TFactory).Name,
               factory.GetType().Name
            )
         );
      }
   }
}
