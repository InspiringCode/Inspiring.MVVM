namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;

   /// <summary>
   /// A 
   /// </summary>
   /// <typeparam propertyName="TValue">The type of the property target.</typeparam>
   [ContractClass(typeof(DisconnectedVMBehaviorContracts<>))]
   public interface ICacheValueBehavior<TValue> : IAccessPropertyBehavior<TValue> {
      void CopyFromSource(IBehaviorContext vm);
      void CopyToSource(IBehaviorContext vm);
   }

   [ContractClassFor(typeof(ICacheValueBehavior<>))]
   internal abstract class DisconnectedVMBehaviorContracts<TValue> : ICacheValueBehavior<TValue> {
      public void CopyFromSource(IBehaviorContext vm) {
         Contract.Requires<ArgumentNullException>(vm != null);
      }

      public void CopyToSource(IBehaviorContext vm) {
         Contract.Requires<ArgumentNullException>(vm != null);
      }

      public TValue GetValue(IBehaviorContext vm) {
         return default(TValue);
      }

      public void SetValue(IBehaviorContext vm, TValue value) {
      }

      public IBehavior Successor {
         get { return default(IBehavior); }
         set { }
      }

      public void Initialize(BehaviorInitializationContext context) {
      }
   }
}
