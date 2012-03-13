namespace Inspiring.Mvvm.Common {
   using System;
   using System.Diagnostics.Contracts;

   internal sealed class DelegateEventCondition<TPayload> : IEventCondition {
      private readonly Func<TPayload, bool> _condition;

      public DelegateEventCondition(Func<TPayload, bool> condition) {
         Contract.Requires(condition != null);
         _condition = condition;
      }

      public bool IsTrue(object payload) {
         return _condition((TPayload)payload);
      }
   }
}
