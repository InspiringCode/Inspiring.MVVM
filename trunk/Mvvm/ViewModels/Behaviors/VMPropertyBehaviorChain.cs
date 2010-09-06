namespace Inspiring.Mvvm.ViewModels.Behaviors {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;

   public class VMPropertyBehaviorChain : VMPropertyBehavior {
      protected TBehavior EnsureBehavior<TBehavior>()
         where TBehavior : IBehavior, new() {
         TBehavior behavior;
         if (!TryGetBehavior<TBehavior>(out behavior)) {
            behavior = new TBehavior();
            EnableBehavior(behavior, VMBehaviors.BehaviorStack);
         }
         return behavior;
      }

      //protected void EnsureBehavior<TBehavior>(out TBehavior behavior)
      //   where TBehavior : IBehavior, new() {
      //   if (!TryGetBehavior<TBehavior>(out behavior)) {
      //      behavior = new TBehavior();
      //      EnableBehavior(behavior, VMBehaviors.BehaviorStack);
      //   }
      //}

      protected TBehavior EnsureBehavior<TBehavior>(Func<TBehavior> behaviorFactory)
         where TBehavior : IBehavior {
         TBehavior behavior;
         if (!TryGetBehavior<TBehavior>(out behavior)) {
            behavior = behaviorFactory();
            EnableBehavior(behavior, VMBehaviors.BehaviorStack);
         }
         return behavior;
      }

      protected void EnableBehavior(IBehavior behavior, IEnumerable<Type> behaviorStack) {
         IBehavior x;
         Contract.Requires<ArgumentException>(
            !TryGetBehavior(behavior.GetType(), out x),
            "Behavior is already registered!"
         );

         if (this.Successor == null) {
            Insert(behavior, after: this);
         } else {
            IBehavior insertAfter = this;
            IBehavior b = insertAfter.Successor;

            foreach (Type t in behaviorStack) {
               if (GetRegisteredType(b.GetType()) == t) {
                  insertAfter = b;
                  b = insertAfter.Successor;
               }

               if (t == GetRegisteredType(behavior.GetType())) {
                  Insert(behavior, after: insertAfter);
                  break;
               }
            }
         }
      }

      [Pure]
      protected bool TryGetBehavior(Type behaviorType, out IBehavior behavior) {
         behaviorType = GetRegisteredType(behaviorType);

         for (IBehavior b = this.Successor; b != null; b = b.Successor) {
            if (b.GetType() == behaviorType) {
               behavior = b;
               return true;
            }
         }

         behavior = null;
         return false;
      }

      private Type GetRegisteredType(Type behaviorType) {
         return behaviorType.IsGenericType ?
            behaviorType.GetGenericTypeDefinition() :
            behaviorType;
      }


      private void Insert(IBehavior insert, IBehavior after) {
         insert.Successor = after.Successor;
         after.Successor = insert;
      }
   }
}
