namespace Inspiring.MvvmTest {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using System.Linq.Expressions;

   internal abstract class ComparisonObject {
      private const int HashMultiplier = 31;
      private const string NullStringValue = "<NULL>";
      private Dictionary<string, object> _properties = new Dictionary<string, object>();

      public override bool Equals(object obj) {
         var other = obj as ComparisonObject;

         if (other == null) {
            return false;
         }

         var otherProperties = other._properties;

         if (_properties.Count != otherProperties.Count) {
            return false;
         }

         foreach (KeyValuePair<string, object> pair in _properties) {
            object otherValue;
            bool otherHasKey = otherProperties.TryGetValue(pair.Key, out otherValue);

            if (!otherHasKey || !Object.Equals(pair.Value, otherValue)) {
               return false;
            }
         }

         return true;
      }

      public override int GetHashCode() {
         unchecked {
            int hashCode = 0;

            foreach (object value in _properties.Values) {
               if (value != null) {
                  hashCode = (hashCode * HashMultiplier) ^ value.GetHashCode();
               }
            }

            return hashCode;
         }
      }

      public override string ToString() {
         var propertyPairs = _properties
            .Select(pair => pair.Key + ": " + ConvertToString(pair.Value));

         return "{ " + String.Join(", ", propertyPairs) + " }";
      }

      protected void SetEqualityProperties(params Expression<Func<object>>[] valueSelectors) {
         _properties = valueSelectors
            .ToDictionary(
               exp => ExtractPropertyName(exp),
               exp => exp.Compile().Invoke()
            );
      }

      private static string ExtractPropertyName(Expression<Func<object>> expression) {
         MemberExpression exp = TraverseExpressionTree(expression)
            .OfType<MemberExpression>()
            .LastOrDefault();

         if (exp == null) {
            throw new ArgumentException(
               "Expression not supported. Make sure you only pass expression " +
               "of the form '() => localVariableOrParameter'."
            );
         }

         return CapitalizeFirstCharacter(exp.Member.Name);
      }

      private static IEnumerable<Expression> TraverseExpressionTree(Expression expression) {
         while (expression != null) {
            yield return expression;
            var lambdaExp = expression as LambdaExpression;
            var memberExp = expression as MemberExpression;
            var unaryExp = expression as UnaryExpression;

            if (lambdaExp != null) {
               expression = lambdaExp.Body;
            } else if (memberExp != null) {
               expression = memberExp.Expression;
            } else if (unaryExp != null) {
               expression = unaryExp.Operand;
            } else {
               expression = null;
            }
         }
      }


      private static string CapitalizeFirstCharacter(string str) {
         return String.IsNullOrEmpty(str) ?
            str :
            Char.ToUpper(str.First()) + str.Substring(startIndex: 1);
      }

      private static string ConvertToString(object value) {
         if (value == null) {
            return NullStringValue;
         }

         return value.ToString();
      }
   }
}
