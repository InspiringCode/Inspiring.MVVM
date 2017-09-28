namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.ComponentModel;

   /// <summary>
   ///   This class serves the internal infrastructure and should not be used.
   /// </summary>
   /// <remarks>
   ///   A view model base class that explicitly implements <see cref="ICustomTypeDescriptor"/>
   ///   and fowards all methods to the <see cref="TypeDescriptor"/> default implementation 
   ///   except <see cref="ICustomTypeDescriptor.GetProperties()"/> which is forwarded to
   ///   the abstract method <see cref="GetPropertyDescriptors"/>.
   /// </remarks>
   public abstract class ViewModelWithTypeDescriptor : ICustomTypeDescriptor {
      /// <inheritdoc />
      PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() {
         return GetPropertyDescriptors();
      }

      /// <inheritdoc />
      PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes) {
         return GetPropertyDescriptors();
      }

      /// <inheritdoc />
      object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) {
         return this;
      }

      /// <inheritdoc />
      AttributeCollection ICustomTypeDescriptor.GetAttributes() {
         return TypeDescriptor.GetAttributes(this, true);
      }

      /// <inheritdoc />
      EventDescriptorCollection ICustomTypeDescriptor.GetEvents() {
         return TypeDescriptor.GetEvents(this, true);
      }

      /// <inheritdoc />
      EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) {
         return TypeDescriptor.GetEvents(this, attributes, true);
      }

      /// <inheritdoc />
      PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() {
         return TypeDescriptor.GetDefaultProperty(this, true);
      }

      /// <inheritdoc />
      EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() {
         return TypeDescriptor.GetDefaultEvent(this, true);
      }

      /// <inheritdoc />
      string ICustomTypeDescriptor.GetClassName() {
         return TypeDescriptor.GetClassName(this, true);
      }

      /// <inheritdoc />
      string ICustomTypeDescriptor.GetComponentName() {
         return TypeDescriptor.GetComponentName(this, true);
      }

      /// <inheritdoc />
      TypeConverter ICustomTypeDescriptor.GetConverter() {
         return TypeDescriptor.GetConverter(this, true);
      }

      /// <inheritdoc />
      object ICustomTypeDescriptor.GetEditor(Type editorBaseType) {
         return TypeDescriptor.GetEditor(this, editorBaseType, true);
      }

      /// <summary>
      ///   Returns a <see cref="PropertyDescriptorCollection"/> with property
      ///   descriptors for all VM properties defined by the VM descriptor of 
      ///   this view model. This method is called each time when <see 
      ///   cref="ICustomTypeDescriptor.GetProperties()"/> is called.
      /// </summary>
      protected abstract PropertyDescriptorCollection GetPropertyDescriptors();
   }
}
