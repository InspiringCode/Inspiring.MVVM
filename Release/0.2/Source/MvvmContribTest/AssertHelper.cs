namespace Inspiring.MvvmContribTest {
   using System;
   using System.Collections.Generic;
   using System.ComponentModel;
   using System.Diagnostics.Contracts;
   using System.Linq;
   using System.Linq.Expressions;
   using System.Reflection;
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


      public static void AssertPropertyChangedEvent<TObject, TProperty>(
         TObject objectToWatch,
         Expression<Func<TObject, TProperty>> propertySelector,
         TProperty expectedValueAtRaiseTime,
         Action actionThatTriggersEvent,
         bool assertEventIsCalled = true
      ) where TObject : INotifyPropertyChanged {
         bool called = false;
         string expectedPropertyName = GetPropertyName(propertySelector);
         PropertyChangedEventHandler handler = delegate(object sender, PropertyChangedEventArgs args) {
            Assert.IsFalse(called);
            called = true;
            Assert.AreEqual(expectedPropertyName, args.PropertyName);
            Assert.AreEqual(expectedValueAtRaiseTime, propertySelector.Compile()(objectToWatch));
         };

         objectToWatch.PropertyChanged += handler;
         actionThatTriggersEvent();
         Assert.AreEqual(assertEventIsCalled, called);

         objectToWatch.PropertyChanged -= handler;
      }

      /// <summary>
      ///   Converts an expression tree of the form 'x => x.Address.City' to 
      ///   a PropertyInfo list of the form { Address, City }.
      /// </summary>
      private static PropertyInfo[] GetProperties<TObject, TProperty>(
         Expression<Func<TObject, TProperty>> propertyPathSelector
      ) {
         Contract.Requires(propertyPathSelector != null);
         Contract.Ensures(Contract.Result<PropertyInfo[]>() != null);

         List<PropertyInfo> propertyPath = new List<PropertyInfo>();
         MemberExpression exp = propertyPathSelector.Body as MemberExpression;
         ParameterExpression last = propertyPathSelector.Body as ParameterExpression;

         while (exp != null) {
            PropertyInfo pi = exp.Member as PropertyInfo;
            if (pi == null) {
               throw new ArgumentException(
                  "Fields are not supported in property paths. Make sure you only reference properties.");
            }

            // An expression tree is a recursive structure where exp.Expression holds 
            // the next or inner expression. The expression x => x.Person.BirthDate 
            // would have the following structure:
            //
            //   new MemberExpression {
            //      Member = "BirthDate",
            //      Expression = new MemberExpression {
            //         Member = "Person",
            //         Expression = new ParameterExpression {
            //            PropertyName = "x"
            //         }
            //      }
            //   }
            //
            // This means to get a path of the form (Person, BirthDate) we have to
            // prepend (and not add) each expression as we go down further into the
            // expression tree.
            propertyPath.Insert(0, pi);

            last = exp.Expression as ParameterExpression;
            exp = exp.Expression as MemberExpression;
         }

         // The above loop exists if the current expression is not a MemberExpression.
         // After 1 to n member expression the only valid expression that can follow
         // is the parameter expression for x in 'x => x.Address.City'. Otherwise
         // we have a malformed property path expression (e.g. 'x => x.GetAddress().City').
         if (last == null || last.Type != typeof(TObject)) {
            throw new ArgumentException(
               "The given expression is not a valid property path. It must only select properties like  'x => x.Address.City'."
            );
         }

         return propertyPath.ToArray();
      }

      /// <summary>
      ///   Gets the propertyName of a single property that is expressed like 'x => x.Address'.
      /// </summary>
      private static string GetPropertyName<TObject, TProperty>(
         Expression<Func<TObject, TProperty>> propertySelector
      ) {
         Contract.Requires(propertySelector != null);

         PropertyInfo[] propertyPath = GetProperties(propertySelector);

         if (propertyPath.Length == 0) {
            throw new ArgumentException(
               "The given expression does not select a property. It must select a single property like 'x => x.Address'."
            );
         }

         if (propertyPath.Length > 1) {
            throw new ArgumentException(
               "The given expression consists of more than one property. It must select a single property like 'x => x.Address'."
            );
         }

         return propertyPath.Single().Name;
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
