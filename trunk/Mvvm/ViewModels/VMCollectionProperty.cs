namespace Inspiring.Mvvm.ViewModels {
   using Inspiring.Mvvm.ViewModels.Core;

   public sealed class VMCollectionProperty<TItemVM> :
      VMProperty<VMCollection<TItemVM>>,
      IBindableCollection<TItemVM>
      where TItemVM : IViewModel {
   }
}
