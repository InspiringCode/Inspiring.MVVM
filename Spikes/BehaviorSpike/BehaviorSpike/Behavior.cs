namespace BehaviorSpike {
   using System;

   internal class Behavior : IBehavior {
      private IBehavior _successor;

      public IBehavior Successor {
         get { return _successor; }
         set { _successor = value; }
      }

      public void TryCall<TBehavior>(Action<TBehavior> callAction) {
         TBehavior b;
         if (TryGetBehavior(out b)) {
            callAction(b);
         }
      }

      /// <summary>
      ///   Tries to get the next behavior in the stack and returns whether a
      ///   behavior that implements 'TBehavior' was found.
      /// </summary>
      public bool TryGetBehavior<TBehavior>(out TBehavior result) {
         for (IBehavior b = this.Successor; b != null; b = b.Successor) {
            if (b is TBehavior) {
               result = (TBehavior)b;
               return true;
            }
         }

         result = default(TBehavior);
         return false;
      }
   }
}
