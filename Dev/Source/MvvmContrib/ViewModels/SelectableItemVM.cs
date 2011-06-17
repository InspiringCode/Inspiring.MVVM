namespace Inspiring.Mvvm.ViewModels {
   using System;

   public sealed class SelectableItemVM<TItemSource, TItemVM> :
      ViewModel<SelectableItemVMDescriptor<TItemVM>>,
      IHasSourceObject<TItemSource>,
      IComparable<SelectableItemVM<TItemSource, TItemVM>>
      where TItemVM : IViewModel, IHasSourceObject<TItemSource> {

      public TItemVM VM {
         get { return GetValue(Descriptor.VM); }
      }

      public TItemSource Source { get; set; }

      public bool IsSelected {
         get { return GetValue(Descriptor.IsSelected); }
         set { SetValue(Descriptor.IsSelected, value); }
      }

      public int CompareTo(SelectableItemVM<TItemSource, TItemVM> other) {
         var source = VM as IComparable<TItemVM>;
         if (source == null) {
            return 0;
         } else {
            return source.CompareTo(other.VM);
         }
      }

      public override string ToString() {
         return VM.ToString();
      }
   }

   public class SelectableItemVMDescriptor<TItemVM> : VMDescriptor {
      public IVMPropertyDescriptor<bool> IsSelected { get; set; }
      public IVMPropertyDescriptor<TItemVM> VM { get; set; }
   }
}
