namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Diagnostics.Contracts;

   internal sealed class ConstantBehaviorFactory : IBehaviorFactory {
      private IBehavior _result;

      public ConstantBehaviorFactory(IBehavior result) {
         Contract.Requires(result != null);
         _result = result;
      }

      public IBehavior Create<TValue>() {
         Contract.Assert(
            _result != null,
            "Only one instance can be created by a 'ConstantBehaviorFactory'."
         );
         return _result;
      }
   }
}
