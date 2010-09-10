namespace Inspiring.Mvvm.ViewModels.Behaviors {
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
         return ((VMPropertyBehavior)behavior).GetNextBehavior<IAccessPropertyBehavior<T>>().GetValue(vm);
      }

      public static void SetNextValue<T>(this IAccessPropertyBehavior<T> behavior, IBehaviorContext vm, T value) {
         ((VMPropertyBehavior)behavior).GetNextBehavior<IAccessPropertyBehavior<T>>().SetValue(vm, value);
      }
   }

   [ContractClassFor(typeof(IAccessPropertyBehavior<>))]
   internal abstract class AccessPropertyBehaviorContracts<TValue> : IAccessPropertyBehavior<TValue> {
      public TValue GetValue(IBehaviorContext vm) {
         Contract.Requires<ArgumentNullException>(vm != null);
         throw new NotImplementedException();
      }

      public void SetValue(IBehaviorContext vm, TValue value) {
         Contract.Requires<ArgumentNullException>(vm != null);
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

   [ContractClassFor(typeof(IAccessPropertyBehavior))]
   internal abstract class AccessPropertyBehaviorContracts : IAccessPropertyBehavior {
      public object GetValue(IBehaviorContext vm) {
         Contract.Requires<ArgumentNullException>(vm != null);
         throw new NotImplementedException();
      }

      public void SetValue(IBehaviorContext vm, object value) {
         Contract.Requires<ArgumentNullException>(vm != null);
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
