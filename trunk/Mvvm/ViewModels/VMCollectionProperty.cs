namespace Inspiring.Mvvm.ViewModels {

   public interface IVMCollectionProperty<out TVM> {

   }

   public sealed class VMCollectionProperty<TVM> : VMPropertyBase<VMCollection<TVM>>, IVMCollectionProperty<TVM> {
   }
}
