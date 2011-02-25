namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;
   using Contracts;

   [ContractClass(typeof(BehaviorFactoryInvokerContract))]
   public abstract class BehaviorFactoryInvoker {
      public abstract IBehavior Invoke(IBehaviorFactory factory, BehaviorKey behaviorToCreate);

      protected TFactory CastFactory<TFactory>(
         IBehaviorFactory factory,
         BehaviorKey behaviorToCreate
      ) where TFactory : IBehaviorFactory {
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
         public override IBehavior Invoke(IBehaviorFactory factory, BehaviorKey behaviorToCreate) {
            Contract.Requires(factory != null);
            Contract.Requires(behaviorToCreate != null);

            return null;
         }
      }
   }
}
