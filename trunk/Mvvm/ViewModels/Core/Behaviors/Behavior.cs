namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;

   public class Behavior : IBehavior {
      public IBehavior Successor { get; set; }

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
               ExceptionTexts.BehaviorNotFound.FormatWith(typeof(TBehavior).Name)
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

      void IBehavior.Initialize(BehaviorInitializationContext context) {
         Initialize(context);

         if (Successor != null) {
            Successor.Initialize(context);
         }
      }

      /// <summary>
      ///   Override this method if your behavior requires dynamic fields that 
      ///   whose values are stored with each <see cref="ViewModel"/> instance.
      ///   The value of fields defined in this way can be accessed via the 
      ///   <see cref="IBehaviorContext.FieldValues"/> property.
      /// </summary>
      protected virtual void Initialize(BehaviorInitializationContext context) {
      }
   }
}
