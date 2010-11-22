namespace Inspiring.Mvvm.ViewModels {
   using Inspiring.Mvvm.ViewModels.Core;

   public sealed class VMCollectionProperty<TItemVM> :
      VMPropertyBase<VMCollection<TItemVM>>,
      IBindableCollection<TItemVM>
      where TItemVM : IViewModel {
   }
}
