namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;
   using Contracts;

   [ContractClass(typeof(IDisplayValueAccessorBehaviorContracts))]
   public interface IDisplayValueAccessorBehavior : IBehavior {
      object GetDisplayValue(IBehaviorContext vm);
      void SetDisplayValue(IBehaviorContext vm, object value);
   }

   namespace Contracts {
      [ContractClassFor(typeof(IDisplayValueAccessorBehavior))]
      internal abstract class IDisplayValueAccessorBehaviorContracts : IDisplayValueAccessorBehavior {
         public object GetDisplayValue(IBehaviorContext vm) {
            Contract.Requires<ArgumentNullException>(vm != null);
            return default(object);
         }

         public void SetDisplayValue(IBehaviorContext vm, object value) {
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
