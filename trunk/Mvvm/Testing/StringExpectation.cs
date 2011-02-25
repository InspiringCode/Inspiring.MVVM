namespace Inspiring.Mvvm.Testing {
   using System;
   using System.Text.RegularExpressions;

   public enum MatchType {
      None,
      Exact,
      Regex
   }

   public sealed class StringExpectation {
      private readonly MatchType _matchType;
      private readonly string _expectedValue;

      public StringExpectation()
         : this(null, MatchType.None) {
      }

      public StringExpectation(string expectedValue, MatchType matchType) {
         _expectedValue = expectedValue;
         _matchType = matchType;
      }

      public bool Matches(string actualValue) {
         switch (_matchType) {
            case MatchType.None:
               return true;
            case MatchType.Exact:
               return String.Equals(actualValue, _expectedValue);
            case MatchType.Regex:
               return Regex.IsMatch(actualValue, _expectedValue);
            default:
               throw new NotSupportedException();
         }
      }
   }
}
