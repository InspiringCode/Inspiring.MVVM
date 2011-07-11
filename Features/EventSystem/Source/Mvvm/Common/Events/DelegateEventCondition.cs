namespace Inspiring.Mvvm.Common.Events {
   using System;
   using System.Diagnostics.Contracts;

   internal sealed class DelegateEventCondition<TPayload> : IEventCondition<TPayload> {
      private readonly Func<TPayload, bool> _condition;

      public DelegateEventCondition(Func<TPayload, bool> condition) {
         Contract.Requires(condition != null);
         _condition = condition;
      }

      public bool IsTrue(TPayload payload) {
         return _condition(payload);
      }
   }
}
