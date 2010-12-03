namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;
   using Contracts;

   /// <summary>
   /// A behavior that intercepts or implements the strongly typed get/set
   /// operation of a property.
   /// </summary>
   /// <typeparam propertyName="TValue">The type of the property target.</typeparam>
   [ContractClass(typeof(IValueAccessorContract<>))]
   public interface IValueAccessorBehavior<TValue> : IBehavior {
      TValue GetValue(IBehaviorContext context, ValueStage stage);
      void SetValue(IBehaviorContext context, TValue value);
   }

   namespace Contracts {
      [ContractClassFor(typeof(IValueAccessorBehavior<>))]
      internal abstract class IValueAccessorContract<TValue> : IValueAccessorBehavior<TValue> {
         public TValue GetValue(IBehaviorContext vm, ValueStage stage) {
            Contract.Requires<ArgumentNullException>(vm != null);
            return default(TValue);
         }

         public void SetValue(IBehaviorContext vm, TValue value) {
            Contract.Requires<ArgumentNullException>(vm != null);
         }

         public IBehavior Successor {
            get { return default(IBehavior); }
            set { }
         }

         public void Initialize(BehaviorInitializationContext context) {
         }
      }
   }
}
