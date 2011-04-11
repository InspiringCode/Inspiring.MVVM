namespace Inspiring.MvvmTest {
   using System;
   using System.Collections;
   using System.Collections.Generic;
   using System.Diagnostics;
   using System.Linq;
   using System.Text.RegularExpressions;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [DebuggerStepThrough]
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


      /// <summary>
      ///   Compares two collections using the specified comparer delegate. The elements
      ///   must appear in the same order in both collections.
      /// </summary>
      public static void AreEqual<T>(
         IEnumerable<T> expected,
         IEnumerable<T> actual,
         Func<T, T, bool> comparer,
         string message = null,
         params object[] parameters
      ) where T : class {
         CollectionAssert.AreEqual(
            expected.ToArray(),
            actual.ToArray(),
            new DelegateEqualityComparer<T>(comparer),
            message,
            parameters
         );
      }

      /// <summary>
      ///   Compares two collections using the specified comparer delegate. If both 
      ///   collections contain the same elements but in different order, the 
      ///   collection are not considered equal.
      /// </summary>
      public static void AreNotEqual<T>(
         IEnumerable<T> expected,
         IEnumerable<T> actual,
         Func<T, T, bool> comparer,
         string message = null,
         params object[] parameters
      ) where T : class {
         CollectionAssert.AreNotEqual(
            expected.ToArray(),
            actual.ToArray(),
            new DelegateEqualityComparer<T>(comparer),
            message,
            parameters
         );
      }

      /// <summary>
      ///   Compares two collections ignoring the order of the elements using the specified
      ///   comparer delegate.
      /// </summary>
      public static void AreEquivalent<T>(
         IEnumerable<T> expected,
         IEnumerable<T> actual,
         Func<T, T, bool> comparer
      ) where T : class {
         if ((expected == null) != (actual == null)) {
            Assert.Fail("One of the collections is null, but the other is not.");
         }

         if (!Object.ReferenceEquals(expected, actual) && (expected != null)) {
            T[] expectedArray = expected.ToArray();
            T[] actualArray = actual.ToArray();
            int expectedCount;
            int actualCount;
            T mismatchedElement;

            if (expectedArray.Length != actualArray.Length) {
               Assert.Fail("The collections do not have the same number of elements.");
            }

            if ((expectedArray.Length != 0) && FindMismatchedElement(
               expectedArray,
               actualArray,
               new DelegateEqualityComparer<T>(comparer),
               out expectedCount,
               out actualCount,
               out mismatchedElement
            )) {
               string message = "The element '{0}' was expected {1}x but was found {2}x.";
               Assert.Fail(message, mismatchedElement, expectedCount, actualCount);
            }
         }
      }

      private static bool FindMismatchedElement<T>(
         IEnumerable<T> expected,
         IEnumerable<T> actual,
         IEqualityComparer<T> comparer,
         out int expectedCount,
         out int actualCount,
         out T mismatchedElement
      ) where T : class {
         int nullCount;
         int nullCount2;
         Dictionary<T, int> expectedElementCounts = GetElementCounts(expected, comparer, out nullCount);
         Dictionary<T, int> actualElementCounts = GetElementCounts(actual, comparer, out nullCount2);

         if (nullCount != nullCount2) {
            expectedCount = nullCount;
            actualCount = nullCount2;
            mismatchedElement = null;
            return true;
         }

         foreach (T element in expectedElementCounts.Keys) {
            expectedElementCounts.TryGetValue(element, out expectedCount);
            actualElementCounts.TryGetValue(element, out actualCount);

            if (expectedCount != actualCount) {
               mismatchedElement = element;
               return true;
            }
         }

         expectedCount = 0;
         actualCount = 0;
         mismatchedElement = null;

         return false;
      }

      private static Dictionary<T, int> GetElementCounts<T>(
         IEnumerable<T> collection,
         IEqualityComparer<T> comparer,
         out int nullCount
      ) where T : class {
         Dictionary<T, int> dict = new Dictionary<T, int>(comparer);
         nullCount = 0;

         foreach (T element in collection) {
            if (element == null) {
               nullCount++;
            } else {
               int count;
               dict.TryGetValue(element, out count);
               count++;
               dict[element] = count;
            }
         }

         return dict;
      }

      [DebuggerStepThrough]
      private sealed class DelegateEqualityComparer<T> : EqualityComparer<T>, IComparer {
         private readonly Func<T, T, bool> _comparer;

         public DelegateEqualityComparer(Func<T, T, bool> comparer) {
            _comparer = comparer;
         }

         public override bool Equals(T x, T y) {
            return _comparer(x, y);
         }

         public override int GetHashCode(T obj) {
            // There is no generic hash code algorithm. But this one complies to
            // the specification altjough it is quite inefficient.
            return default(int);
         }

         /// <summary>
         ///   This implementation does not fullfill the contract of 'IComparer' 
         ///   because it only differentiates between equal and non-equal. But this
         ///   is enough to be used for CollectionAssert.AreEqual.
         /// </summary>
         public int Compare(object x, object y) {
            return _comparer((T)x, (T)y) ?
               0 :
               1;
         }
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

      public ExceptionExpression<TException> WithMessage(string message) {
         Assert.AreEqual(_exception.Message, message, "Exception message did not match expected message.");
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
