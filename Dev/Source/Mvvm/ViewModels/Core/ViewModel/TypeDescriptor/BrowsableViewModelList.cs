﻿namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.ComponentModel;
   using System.Diagnostics.Contracts;
   using System.Linq;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.ViewModels.Core;

   public class BrowsableViewModelList<T> :
      BindingList<T>,
      ITypedList
      where T : IViewModel {

      private PropertyDescriptorCollection _itemProperties = null;

      public BrowsableViewModelList() {
         // TODO: Clean up?
         IVMDescriptor classDescriptor = ClassDescriptorAttribute.GetClassDescriptorOf(typeof(T));
         if (classDescriptor == null) {
            throw new ArgumentException(EViewModels.BrowsableListRequiresClassDescriptorAttribute);
         }
      }

      public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
         bool hasListAccessors = listAccessors != null && listAccessors.Any();

         if (hasListAccessors) {
            PropertyDescriptor lastAccessor = listAccessors.Last();
            var browsableDescriptor = lastAccessor as BrowsablePropertyDescriptor;

            return browsableDescriptor != null ?
               browsableDescriptor.ChildProperties :
               TypeDescriptor.GetProperties(lastAccessor.PropertyType);
         }

         if (_itemProperties == null) {
            _itemProperties = BrowsablePropertyDescriptor.GetPropertiesOf(typeof(T));
         }

         return _itemProperties;
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

   internal class BrowsablePropertyDescriptor : PropertyDescriptorDecorator<ViewModelPropertyDescriptor> {
      private PropertyDescriptorCollection _childProperties;

      public BrowsablePropertyDescriptor(ViewModelPropertyDescriptor decorated)
         : base(decorated) {
         Contract.Requires(decorated != null);
      }

      public override Type PropertyType {
         get { return Decorated.Property.PropertyType; }
      }

      public PropertyDescriptorCollection ChildProperties {
         get {
            if (_childProperties == null) {
               _childProperties = GetPropertiesOf(PropertyType);
            }
            return _childProperties;
         }
      }

      public static PropertyDescriptorCollection GetPropertiesOf(Type type) {
         Type itemType = TypeService.GetItemType(collectionType: type);

         if (itemType != null) {
            type = itemType;
         }

         IVMDescriptor childDescriptor = ClassDescriptorAttribute
            .GetClassDescriptorOf(type);

         if (childDescriptor != null) {
            var browsableDescriptors = childDescriptor
               .GetPropertyDescriptors()
               .OfType<ViewModelPropertyDescriptor>()
               .Select(actual => new BrowsablePropertyDescriptor(actual))
               .ToArray();

            return new PropertyDescriptorCollection(browsableDescriptors);
         }

         return TypeDescriptor.GetProperties(type);
      }
   }
}
