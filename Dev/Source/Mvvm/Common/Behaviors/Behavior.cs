namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.Common;

   public class Behavior : SealableObject, IBehavior {
      private IBehavior _successor;

      public IBehavior Successor {
         get { return _successor; }
         set {
            RequireNotSealed();
            _successor = value;
         }
      }



      public void TryCall<TBehavior>(Action<TBehavior> callAction) {
         TBehavior b;
         if (TryGetBehavior(out b)) {
            callAction(b);
         }
      }

      /// <summary>
      ///   Gets the next behavior in the stack and throws an 'ArgumentException'
      ///   if no behavior that implements 'TBehavior' can be found.
      /// </summary>
      public TBehavior GetNextBehavior<TBehavior>() {
         Contract.Ensures(Contract.Result<TBehavior>() != null);

         TBehavior result;
         if (!TryGetBehavior<TBehavior>(out result)) {
            throw new ArgumentException(
               ExceptionTexts.BehaviorNotFound.FormatWith(TypeService.GetFriendlyName(typeof(TBehavior)))
            );
         }
         return result;
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
