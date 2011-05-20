namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.ComponentModel;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels.Core;

   public class TypedBindingList<T> : BindingList<T>, ITypedList {
      public virtual PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
         Type requestedType;

         if (listAccessors != null && listAccessors.Any()) {
            PropertyDescriptor lastAccessor = listAccessors.Last();
            var pd = lastAccessor as ViewModelPropertyDescriptor;

            requestedType = pd != null ?
               pd.Property.PropertyType :
               lastAccessor.PropertyType;
         } else {
            requestedType = typeof(T);
         }

         return TypeDescriptor.GetProperties(requestedType);
      }

      public string GetListName(PropertyDescriptor[] listAccessors) {
         // This method is used only in the design-time framework and by the 
         // obsolete DataGrid control.
         if (listAccessors != null && listAccessors.Any()) {
            return listAccessors
               .Last()
               .PropertyType
               .ToString();
         } else {
            return typeof(T).ToString();
         }
      }
   }
}
