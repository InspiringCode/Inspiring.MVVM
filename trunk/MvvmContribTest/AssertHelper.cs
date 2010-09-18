namespace Inspiring.MvvmContribTest {
   using System;
   using System.Text.RegularExpressions;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   internal static class AssertHelper {
      /// <summary>
      /// Verifies that the exact exception is thrown (and not a derived exception type).
      /// </summary>
      public static ExceptionExpression<TException> Throws<TException>(Action testCode) where TException : Exception {
         try {
            testCode();
         } catch (TException ex) {
            if (typeof(TException) != ex.GetType()) {
               throw;
            }
            return new ExceptionExpression<TException>(ex);
         }

         Assert.Fail("Expected exception {0}.", typeof(TException).Name);
         throw new InvalidOperationException("This code should never be reached.");
      }
   }

   internal class ExceptionExpression<TException> where TException : Exception {
      private TException _exception;

      public ExceptionExpression(TException exception) {
         _exception = exception;
      }

      public TException Exception {
         get { return _exception; }
      }

      public ExceptionExpression<TException> Containing(string messageSubstring) {
         Assert.IsTrue(
            _exception.Message.ToUpper().Contains(messageSubstring.ToUpper()),
            String.Format(
               "Exception message did not contain case insensitive substring '{0}'. Message was: '{1}'.",
               messageSubstring,
               _exception.Message
            )
         );

         return this;
      }

      public ExceptionExpression<TException> ContainingCaseSensitive(string messageSubstring) {
         Assert.IsTrue(
            _exception.Message.Contains(messageSubstring),
            String.Format(
               "Exception message did not contain case sensitive substring '{0}'. Message was: '{1}'.",
               messageSubstring,
               _exception.Message
            )
         );

         return this;
      }

      public ExceptionExpression<TException> Matches(string messagePattern) {
         Assert.IsTrue(Regex.IsMatch(_exception.Message, messagePattern, RegexOptions.IgnoreCase));
         Assert.IsTrue(
            Regex.IsMatch(_exception.Message, messagePattern),
            String.Format(
               "Exception message did not match case insensitive pattern '{0}'. Message was: '{1}'.",
               messagePattern,
               _exception.Message
            )
         );

         return this;
      }

      public ExceptionExpression<TException> MatchesCaseSensitive(string messagePattern) {
         Assert.IsTrue(
            Regex.IsMatch(_exception.Message, messagePattern),
            String.Format(
               "Exception message did not match case sensitive pattern '{0}'. Message was: '{1}'.",
               messagePattern,
               _exception.Message
            )
         );

         return this;
      }


   }
}
