namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;

   /// <summary>
   ///   The validation context holds context that is shared during a single 
   ///   validation process. It holds various validator contexts and the 
   ///   revalidation queue.
   /// </summary>
   // TODO: Clean me up please!
   public sealed class ValidationContext {
      private static ValidationContext _current = null;
      private static int _level = 0;

      private ValidationContext() {
         RevalidationQueue = new RevalidationQueue();
      }

      public static ValidationContext Current {
         get { return _current; }
      }

      public static void BeginValidation() {
         Contract.Assert(_level == 0 || Current != null);
         _level++;

         if (_current == null) {
            _current = new ValidationContext();
         }
      }

      public RevalidationQueue RevalidationQueue { get; set; }

      public static void CompleteValidation(ValidationMode validationMode) {
         Contract.Requires<InvalidOperationException>(Current != null);
         
         if ((_level - 1) == 0) {
            Current.RevalidationQueue.Revalidate(Current, validationMode);
            _current = null;
         }

         _level--;
      }
   }
}
