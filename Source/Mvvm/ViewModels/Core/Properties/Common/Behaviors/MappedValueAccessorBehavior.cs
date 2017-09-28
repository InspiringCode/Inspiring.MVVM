namespace Inspiring.Mvvm.ViewModels.Core {
   using Inspiring.Mvvm.Common;

   /// <summary>
   ///   A <see cref="IDisplayValueAccessorBehavior"/> that implements the get/set 
   ///   operation of a <see cref="IVMPropertyDescriptor"/> by using a CLR property defined 
   ///   on a the view model or an object referenced by it.
   /// </summary>
   internal sealed class MappedValueAccessorBehavior<TVM, TValue> :
      Behavior,
      IValueAccessorBehavior<TValue>
      where TVM : IViewModel {

      private PropertyPath<TVM, TValue> _path;

      public MappedValueAccessorBehavior(PropertyPath<TVM, TValue> path) {
         Check.NotNull(path, nameof(path));
         _path = path;
      }

      public TValue GetValue(IBehaviorContext vm) {
         return _path.GetValue((TVM)vm.VM);
      }

      public void SetValue(IBehaviorContext vm, TValue value) {
         _path.SetValue((TVM)vm.VM, value);
      }
   }
}
