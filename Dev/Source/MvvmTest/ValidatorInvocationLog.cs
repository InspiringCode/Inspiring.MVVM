namespace Inspiring.MvvmTest {
   using System.Collections.Generic;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using ValidationArgs = Inspiring.Mvvm.ViewModels.Core.ValidationArgs;

   public class ValidatorInvocationLog {
      private List<object> _expectedCalls = new List<object>();
      private Dictionary<object, ValidationArgs> _actualCalls = new Dictionary<object, ValidationArgs>();

      public ValidatorInvocationLog() {
         IsEnabled = false;
      }

      public bool IsEnabled { get; set; }

      public void ExpectCalls(params object[] toValidators) {
         IsEnabled = true;
         _expectedCalls.AddRange(toValidators);
      }

      public void AddCall(object validator, ValidationArgs args) {
         if (!IsEnabled) {
            return;
         }

         Assert.IsTrue(
            _expectedCalls.Contains(validator),
            "Did not expect a call to validator {0}.",
            validator
         );

         _actualCalls.Add(validator, args);
      }

      public void VerifyCalls() {
         CollectionAssert.AreEquivalent(
            _expectedCalls,
            _actualCalls.Keys,
            "Not all expected validators were called."
         );
      }

      public ValidationArgs GetArgs(object forValidator) {
         return _actualCalls[forValidator];
      }
   }
}