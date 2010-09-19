namespace Inspiring.Mvvm.ViewModels {

   public interface IVMCollectionProperty<out TVM> {

   }

   public sealed class VMCollectionProperty<TItemVM> :
      VMPropertyBase<VMCollection<TItemVM>>,
      IVMCollectionProperty<TItemVM>
      where TItemVM : ViewModel {
   }
}
