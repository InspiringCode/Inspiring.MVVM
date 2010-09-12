namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;

   /// <summary>
   /// A behavior that intercepts or implements the strongly typed get/set
   /// operation of a property.
   /// </summary>
   /// <typeparam propertyName="TValue">The type of the property target.</typeparam>
   [ContractClass(typeof(AccessPropertyBehaviorContracts<>))]
   public interface IAccessPropertyBehavior<TValue> : IBehavior {
      TValue GetValue(IBehaviorContext vm);
      void SetValue(IBehaviorContext vm, TValue value);
   }

   [ContractClass(typeof(AccessPropertyBehaviorContracts))]
   public interface IAccessPropertyBehavior : IBehavior {
      object GetValue(IBehaviorContext vm);
      void SetValue(IBehaviorContext vm, object value);
   }

   public static class AccessPropertyBehaviorExtension {
      public static T GetNextValue<T>(this IAccessPropertyBehavior<T> behavior, IBehaviorContext vm) {
         return ((Behavior)behavior).GetNextBehavior<IAccessPropertyBehavior<T>>().GetValue(vm);
      }

      public static void SetNextValue<T>(this IAccessPropertyBehavior<T> behavior, IBehaviorContext vm, T value) {
         ((Behavior)behavior).GetNextBehavior<IAccessPropertyBehavior<T>>().SetValue(vm, value);
      }
   }

   [ContractClassFor(typeof(IAccessPropertyBehavior<>))]
   internal abstract class AccessPropertyBehaviorContracts<TValue> : IAccessPropertyBehavior<TValue> {
      public TValue GetValue(IBehaviorContext vm) {
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

   [ContractClassFor(typeof(IAccessPropertyBehavior))]
   internal abstract class AccessPropertyBehaviorContracts : IAccessPropertyBehavior {
      public object GetValue(IBehaviorContext vm) {
         Contract.Requires<ArgumentNullException>(vm != null);
         return default(object);
      }

      public void SetValue(IBehaviorContext vm, object value) {
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
