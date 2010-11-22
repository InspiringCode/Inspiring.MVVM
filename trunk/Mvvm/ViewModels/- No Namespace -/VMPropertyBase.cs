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

      object IVMProperty.GetValue(IBehaviorContext context, ValueStage stage) {
         throw new NotImplementedException();
      }

      void IVMProperty.SetValue(IBehaviorContext context, object value) {
         throw new NotImplementedException();
      }

      internal TValue GetValue(IBehaviorContext context, ValueStage stage = ValueStage.PreValidation) {
         Contract.Requires(context != null);

         if (stage == ValueStage.PreConversion) {
            return (TValue)GetDisplayValue(context);
         }

         // Use IValueAccessor
         throw new NotImplementedException();
      }

      internal void SetValue(IBehaviorContext context, TValue value) {
         Contract.Requires(context != null);

         // Use IValueAccessor
         throw new NotImplementedException();
      }

      internal object GetDisplayValue(IBehaviorContext context) {
         Contract.Requires(context != null);
         // Use IDisplayValueAccessor
         throw new NotImplementedException();
      }

      internal void SetDisplayValue(IBehaviorContext context, object value) {
         Contract.Requires(context != null);
         // Use IDisplayValueAccessor
         throw new NotImplementedException();
      }

      string IVMProperty.PropertyName {
         get { throw new NotImplementedException(); }
      }

      Type IVMProperty.PropertyType {
         get { throw new NotImplementedException(); }
      }

      Behavior IVMProperty.Behaviors {
         get { throw new NotImplementedException(); }
      }
   }
}
