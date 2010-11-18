namespace Inspiring.Mvvm.ViewModels.__No_Namespace__ {
   using System;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.ViewModels.Core;

   internal sealed class VMPropertyBase<TValue> : IVMProperty {
      public string PropertyName {
         get;
         private set;
      }

      public Type PropertyType {
         get { return typeof(TValue); }
      }

      object IVMProperty.GetValue(IBehaviorContext_ context, ValueStage stage) {
         throw new NotImplementedException();
      }

      void IVMProperty.SetValue(IBehaviorContext_ context, object value) {
         throw new NotImplementedException();
      }

      internal TValue GetValue(IBehaviorContext_ context, ValueStage stage = ValueStage.PreValidation) {
         Contract.Requires(context != null);

         if (stage == ValueStage.PreConversion) {
            return (TValue)GetDisplayValue(context);
         }

         // Use IValueAccessor
         throw new NotImplementedException();
      }

      internal void SetValue(IBehaviorContext_ context, TValue value) {
         Contract.Requires(context != null);

         // Use IValueAccessor
         throw new NotImplementedException();
      }

      internal object GetDisplayValue(IBehaviorContext_ context) {
         Contract.Requires(context != null);
         // Use IDisplayValueAccessor
         throw new NotImplementedException();
      }

      internal void SetDisplayValue(IBehaviorContext context, object value) {
         Contract.Requires(context != null);
         // Use IDisplayValueAccessor
         throw new NotImplementedException();
      }
   }
}
