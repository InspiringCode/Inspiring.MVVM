﻿namespace Inspiring.Mvvm.ViewModels.Behaviors {
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
         throw new NotImplementedException();
      }

      public void CopyToSource(IBehaviorContext vm) {
         Contract.Requires<ArgumentNullException>(vm != null);
         throw new NotImplementedException();
      }

      public TValue GetValue(IBehaviorContext vm) {
         throw new NotImplementedException();
      }

      public void SetValue(IBehaviorContext vm, TValue value) {
         throw new NotImplementedException();
      }

      public IBehavior Successor {
         get {
            throw new NotImplementedException();
         }
         set {
            throw new NotImplementedException();
         }
      }


      public BehaviorPosition Position {
         get { throw new NotImplementedException(); }
      }
   }
}