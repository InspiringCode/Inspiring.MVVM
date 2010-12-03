namespace Inspiring.Mvvm.ViewModels.Core.Behaviors {
   using System;
   using System.Diagnostics.Contracts;

   internal sealed class DefaultViewModelBehaviorFactory : IBehaviorFactory {
      private BehaviorKey _behavior;

      public DefaultViewModelBehaviorFactory(BehaviorKey behaviorToCreate) {
         Contract.Requires<ArgumentNullException>(behaviorToCreate != null);

         _behavior = behaviorToCreate;
      }

      public IBehavior Create<T>() {
         if (_behavior == BehaviorKeys.TypeDescriptor) {
            return new TypeDescriptorBehavior();
         }


         throw new NotSupportedException(
            ExceptionTexts.BehaviorNotSupportedByFactory.FormatWith(_behavior)
         );
      }
   }
}
