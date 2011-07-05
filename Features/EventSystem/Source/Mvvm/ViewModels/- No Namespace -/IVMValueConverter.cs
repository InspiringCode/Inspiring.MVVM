namespace Inspiring.Mvvm.ViewModels {

   public interface IVMValueConverter<TValue> {
      object Convert(TValue value);
      ConversionResult<TValue> ConvertBack(object value);
   }

   public sealed class ConversionResult {
      public static ConversionResult<T> Success<T>(T value) {
         return new ConversionResult<T>(value);
      }

      public static ConversionResult<T> Failure<T>(string errorMessage) {
         return new ConversionResult<T>(errorMessage);
      }
   }

   public sealed class ConversionResult<T> {
      internal ConversionResult(T value) {
         Value = value;
         Successful = true;
         ErrorMessage = null;
      }

      internal ConversionResult(string errorMessage) {
         ErrorMessage = errorMessage;
         Value = default(T);
         Successful = false;
      }

      public T Value { get; private set; }
      public bool Successful { get; private set; }
      public string ErrorMessage { get; private set; }
   }
}
