namespace Inspiring.Mvvm.Common {
   using System;

   internal sealed class DelegateEventCondition<TPayload> : IEventCondition {
      private readonly Func<TPayload, bool> _condition;

      public DelegateEventCondition(Func<TPayload, bool> condition) {
         Check.NotNull(condition, nameof(condition));
         _condition = condition;
      }

      public bool IsTrue(object payload) {
         return _condition((TPayload)payload);
      }
   }
}
