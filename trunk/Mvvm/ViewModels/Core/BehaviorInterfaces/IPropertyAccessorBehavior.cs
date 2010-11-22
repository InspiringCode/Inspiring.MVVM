namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;

   /// <summary>
   /// A behavior that intercepts or implements the strongly typed get/set
   /// operation of a property.
   /// </summary>
   /// <typeparam propertyName="TValue">The type of the property target.</typeparam>
   [ContractClass(typeof(AccessPropertyBehaviorContracts<>))]
   public interface IPropertyAccessorBehavior<TValue> : IBehavior {
      TValue GetValue(IBehaviorContext context, ValueStage stage);
      void SetValue(IBehaviorContext context, TValue value);
   }

   [ContractClassFor(typeof(IPropertyAccessorBehavior<>))]
   internal abstract class AccessPropertyBehaviorContracts<TValue> : IPropertyAccessorBehavior<TValue> {
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
