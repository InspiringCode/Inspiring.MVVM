using System.Collections.ObjectModel;
using System.ComponentModel;
namespace Inspiring.Mvvm.ViewModels {

   public sealed class VMCollection<T> : ObservableCollection<T>, ITypedList {
      public VMCollection(VMDescriptor itemDescriptor) {
         ItemDescriptor = itemDescriptor;
      }

      public VMDescriptor ItemDescriptor { get; private set; }

      public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
         return ItemDescriptor.PropertyDescriptors;
      }

      public string GetListName(PropertyDescriptor[] listAccessors) {
         return GetType().Name;
      }
   }
}
