namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class SetValueAction<TValue> : IUndoableAction {

      private readonly IViewModel _vm;
      private readonly IVMPropertyDescriptor _property;
      private readonly TValue _originalValue;

      public SetValueAction(
         IViewModel vm,
         IVMPropertyDescriptor property,
         TValue originalValue
      ) {
         _vm = vm;
         _property = property;
         _originalValue = originalValue;
      }

      public void Undo() {
         _vm.Kernel.SetValue(_property, _originalValue);
      }
   }
}