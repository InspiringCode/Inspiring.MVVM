namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;
   using Contracts;
   using Inspiring.Mvvm.Common.Behaviors;

   public interface IBehaviorFactoryConfiguration {
      IBehaviorFactory GetFactory(IBehaviorFactoryProvider factoryProvider);
   }

   [ContractClass(typeof(BehaviorFactoryInvokerContract))]
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

   namespace Contracts {
      [ContractClassFor(typeof(BehaviorFactoryInvoker))]
      internal abstract class BehaviorFactoryInvokerContract : BehaviorFactoryInvoker {
         public override IBehavior Invoke(IBehaviorFactoryProvider factory, BehaviorKey behaviorToCreate) {
            Contract.Requires(factory != null);
            Contract.Requires(behaviorToCreate != null);

            return null;
         }
      }
   }
}
