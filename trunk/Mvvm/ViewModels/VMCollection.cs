using System.Collections.ObjectModel;
using System.ComponentModel;
namespace Inspiring.Mvvm.ViewModels {

   public sealed class VMCollection<T> : ObservableCollection<T>, ITypedList {
      private VMDescriptor _itemDescriptor;

      public VMCollection(VMDescriptor itemDescriptor) {
         _itemDescriptor = itemDescriptor;
      }

      public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
         return _itemDescriptor.PropertyDescriptors;
      }

      public string GetListName(PropertyDescriptor[] listAccessors) {
         return GetType().Name;
      }
   }
}
