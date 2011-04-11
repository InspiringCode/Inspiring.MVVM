namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   public class CommandBehaviorFactory : IBehaviorFactory {
      public static readonly CommandBehaviorFactory Instance = new CommandBehaviorFactory();

      public static BehaviorFactoryInvoker CreateInvoker<TVM, TSourceObject>()
         where TVM : IViewModel {

         return new CommandBehaviorFactoryInvoker<TVM, TSourceObject>();
      }

      public virtual IBehavior Create<TVM, TSourceObject>(BehaviorKey key) where TVM : IViewModel {
         if (key == PropertyBehaviorKeys.WaitCursor) {
            return new WaitCursorBehavior();
         }

         throw new NotSupportedException(
            ExceptionTexts.BehaviorNotSupportedByFactory.FormatWith(key)
         );
      }

      private class CommandBehaviorFactoryInvoker<TVM, TSourceObject> :
         BehaviorFactoryInvoker
         where TVM : IViewModel {

         public override IBehavior Invoke(IBehaviorFactory factory, BehaviorKey behaviorToCreate) {
            var f = CastFactory<CommandBehaviorFactory>(factory, behaviorToCreate);
            return f.Create<TVM, TSourceObject>(behaviorToCreate);
         }
      }
   }
}
