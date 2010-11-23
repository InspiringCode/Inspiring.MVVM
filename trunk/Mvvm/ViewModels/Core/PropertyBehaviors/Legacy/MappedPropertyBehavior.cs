namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.Common;

   // TODO (important): Implement IManuelUpdateBehavior correctly!!!!!!!

   /// <summary>
   ///   A <see cref="IDisplayValueAccessorBehavior"/> that implements the get/set 
   ///   operation of a <see cref="VMPropertyBase"/> by using the target of a CLR
   ///   property defined on a target object of the view model.
   /// </summary>
   internal sealed class MappedPropertyBehavior<TVM, TValue> :
      Behavior,
      IPropertyAccessorBehavior<TValue>
      where TVM : IViewModel {

      private VMPropertyBase<TValue> _property;
      private PropertyPath<TVM, TValue> _path;

      public MappedPropertyBehavior(PropertyPath<TVM, TValue> path) {
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
