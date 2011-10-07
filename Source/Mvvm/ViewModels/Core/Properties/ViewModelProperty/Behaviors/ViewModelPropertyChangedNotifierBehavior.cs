namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class ViewModelPropertyChangedNotifierBehavior<TValue> :
      PropertyChangedNotifierBehavior<TValue>
      where TValue : IViewModel {

      protected override ChangeArgs CreateChangeArgs(
         IVMPropertyDescriptor property,
         TValue oldValue,
         TValue newValue
      ) {
         return ChangeArgs.ViewModelPropertyChanged(property, oldValue, newValue, reason: null);
      }
   }
}
