namespace Inspiring.Mvvm.Common {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using System.Linq;
   using System.Linq.Expressions;
   using System.Reflection;

   /// <summary>
   /// Provides services for parsing expression trees.
   /// </summary>
   internal static class ExpressionService {
      /// <summary>
      ///   Converts an expression tree of the form 'x => x.Address.City' to 
      ///   a PropertyInfo list of the form { Address, City }.
      /// </summary>
      public static PropertyInfo[] GetProperties<TObject, TProperty>(
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
               throw new ArgumentException(ExceptionTexts.ExpressionCannotContainFields);
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
            throw new ArgumentException(ExceptionTexts.UnsupportedPropertyPathExpression);
         }

         return propertyPath.ToArray();
      }

      /// <summary>
      ///   Converts an expression tree of the form 'x => x.Address.City' to 
      ///   string that contains the property names separated by dots (e.g.
      ///   'Address.City'.
      /// </summary>
      public static string GetPropertyPathString<TObject, TProperty>(
         Expression<Func<TObject, TProperty>> propertyPathSelector
      ) {
         IEnumerable<PropertyInfo> properties = GetProperties(propertyPathSelector);
         return String.Join(".", properties.Select(p => p.Name));
      }

      /// <summary>
      ///   Gets the propertyName of a single property that is expressed like 'x => x.Address'.
      /// </summary>
      public static string GetPropertyName<TObject, TProperty>(
         Expression<Func<TObject, TProperty>> propertySelector
      ) {
         Contract.Requires<ArgumentNullException>(propertySelector != null);

         PropertyInfo[] propertyPath = GetProperties(propertySelector);

         if (propertyPath.Length == 0) {
            throw new ArgumentException(ExceptionTexts.ExpressionContainsNoProperties);
         }

         if (propertyPath.Length > 1) {
            throw new ArgumentException(ExceptionTexts.ExpressionContainsMoreThanOneProperty);
         }

         return propertyPath.Single().Name;
      }

      public static string GetPropertyName<TProperty>(Expression<Func<TProperty>> propertySelector) {
         Contract.Requires<ArgumentNullException>(propertySelector != null);

         UnaryExpression unary = propertySelector.Body as UnaryExpression;
         MemberExpression exp = unary != null ?
            unary.Operand as MemberExpression :
            propertySelector.Body as MemberExpression;

         if (exp == null || !(exp.Member is PropertyInfo) || !(exp.Expression is ConstantExpression)) {
            throw new ArgumentException(ExceptionTexts.UnsupportedParameterlessPropertyExpression);
         }

         return exp.Member.Name;
      }
   }
}
