namespace Inspiring.Mvvm.ViewModels {
   using System;

   public sealed class MultiSelectionItemVM<TItemSource, TItemVM> :
      ViewModel<MultiSelectionItemVMDescriptor<TItemVM>>,
      IHasSourceObject<TItemSource>,
      IComparable<MultiSelectionItemVM<TItemSource, TItemVM>>
      where TItemVM : IViewModel, IHasSourceObject<TItemSource> {

      public TItemVM VM {
         get { return GetValue(Descriptor.VM); }
      }

      public bool IsSelected {
         get { return GetValue(Descriptor.IsSelected); }
         set { SetValue(Descriptor.IsSelected, value); }
      }

      public int CompareTo(MultiSelectionItemVM<TItemSource, TItemVM> other) {
         var source = VM as IComparable<TItemVM>;
         if (source == null) {
            return 0;
         } else {
            return source.CompareTo(other.VM);
         }
      }

      public TItemSource Source { get; set; }
   }

   public class MultiSelectionItemVMDescriptor<TItemVM> : VMDescriptor {
      public IVMPropertyDescriptor<bool> IsSelected { get; set; }
      public IVMPropertyDescriptor<TItemVM> VM { get; set; }
   }
}
