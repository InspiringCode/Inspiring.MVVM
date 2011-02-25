namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Diagnostics.Contracts;

   public sealed class ChangeValueRequest<TValue> {
      public ChangeValueRequest(object newValue, IVMValueConverter<TValue> converter) {
         Contract.Requires<ArgumentNullException>(newValue != null);
         Contract.Requires<ArgumentNullException>(converter != null);

         NewValue = newValue;
         Converter = converter;
      }

      public object NewValue { get; private set; }
      public IVMValueConverter<TValue> Converter { get; private set; }
   }
}
