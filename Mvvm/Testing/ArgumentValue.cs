namespace Inspiring.Mvvm.Testing {
   using System;
   using System.Text.RegularExpressions;

   public enum MatchType {
      None,
      Exact,
      Regex
   }

   internal class ArgumentValue {
      public ArgumentValue(string parameterName) {
         ParameterName = parameterName;
      }

      public bool IsSet { get; private set; }
      protected object Value { get; private set; }
      private string ParameterName { get; set; }

      public void SetValue(object value) {
         Value = value;
         IsSet = true;
      }

      public virtual bool Matches(ArgumentValue other) {
         if (!IsSet) {
            return true;
         }

         return Object.Equals(Value, other.Value);
      }

      public override string ToString() {
         string valueString = Object.ReferenceEquals(Value, null) ?
            "<NULL>" :
            Value.ToString();

         return String.Format("{0}: {1}", ParameterName, valueString);
      }
   }

   internal class StringArgumentValue : ArgumentValue {
      public StringArgumentValue(string parameterName)
         : base(parameterName) {
         MatchType = MatchType.None;
      }

      public MatchType MatchType {
         get;
         set;
      }

      public void SetValue(string value, MatchType matchType) {
         MatchType = matchType;
         SetValue(value);
      }

      public override bool Matches(ArgumentValue other) {
         StringArgumentValue actual = (StringArgumentValue)other;

         string expectedStr = (string)Value;
         string actualStr = (string)actual.Value;

         switch (MatchType) {
            case MatchType.None:
               return true;
            case MatchType.Exact:
               return String.Equals(expectedStr, actualStr);
            case MatchType.Regex:
               return Regex.IsMatch(expectedStr, actualStr);
            default:
               throw new NotSupportedException();
         }
      }
   }
}
