﻿namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.Common;

   // TODO (important): Implement IManuelUpdateBehavior correctly!!!!!!!

   /// <summary>
   ///   A <see cref="IDisplayValueAccessorBehavior"/> that implements the get/set 
   ///   operation of a <see cref="VMPropertyBase"/> by using a CLR property defined 
   ///   on a the view model or an object referenced by it.
   /// </summary>
   internal sealed class MappedPropertyAccessor<TVM, TValue> :
      Behavior,
      IValueAccessorBehavior<TValue>
      where TVM : IViewModel {

      private VMPropertyBase<TValue> _property;
      private PropertyPath<TVM, TValue> _path;

      public MappedPropertyAccessor(PropertyPath<TVM, TValue> path) {
         Contract.Requires(path != null);
         _path = path;
      }

      public TValue GetValue(IBehaviorContext vm, ValueStage stage) {
         return _path.GetValue((TVM)vm.VM);
      }

      public void SetValue(IBehaviorContext vm, TValue value) {
         _path.SetValue((TVM)vm.VM, value);
      }
   }
}
