﻿namespace Inspiring.Mvvm.ViewModels.Core {

   /// <summary>
   /// A 
   /// </summary>
   //[ContractClass(typeof(DisconnectedVMBehaviorContracts<>))]
   public interface IManuelUpdateBehavior : IBehavior {
      void UpdateFromSource(IBehaviorContext vm);
      void UpdateSource(IBehaviorContext vm);
   }

   //[ContractClassFor(typeof(ICacheValueBehavior<>))]
   //internal abstract class DisconnectedVMBehaviorContracts<TValue> : ICacheValueBehavior<TValue> {
   //   public void CopyFromSource(IBehaviorContext vm) {
   //      Contract.Requires<ArgumentNullException>(vm != null);
   //   }

   //   public void CopyToSource(IBehaviorContext vm) {
   //      Contract.Requires<ArgumentNullException>(vm != null);
   //   }

   //   public TValue GetValue(IBehaviorContext vm) {
   //      return default(TValue);
   //   }

   //   public void SetValue(IBehaviorContext vm, TValue value) {
   //   }

   //   public IBehavior Successor {
   //      get { return default(IBehavior); }
   //      set { }
   //   }

   //   public void Initialize(BehaviorInitializationContext context) {
   //   }
   //}
}
