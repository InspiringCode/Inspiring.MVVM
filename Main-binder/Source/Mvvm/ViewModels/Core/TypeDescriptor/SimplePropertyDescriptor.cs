namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.ComponentModel;

   /// <summary>
   ///   Simplifies the <see cref="PropertyDescriptor"/> class by providing a
   ///   default implementation for irrelevant, abstract methods.
   /// </summary>
   internal abstract class SimplePropertyDescriptor : PropertyDescriptor {
      private Type _propertyType;
      private Type _componentType;

      /// <param name="componentType">
      ///   The type on which the property is defined on. The 'component' parameter
      ///   is expected to be a subclass of this type.
      /// </param>
      public SimplePropertyDescriptor(
         string propertyName,
         Type propertyType,
         Type componentType
      )
         : base(propertyName, attrs: null) {
         _propertyType = propertyType;
         _componentType = componentType;
      }

      /// <inheritdoc/>
      public override Type ComponentType {
         get { return _componentType; }
      }

      /// <inheritdoc/>
      public override Type PropertyType {
         get { return _propertyType; }
      }

      /// <inheritdoc/>
      public override bool IsReadOnly {
         get { return false; }
      }

      /// <inheritdoc/>
      public override bool CanResetValue(object component) {
         return false;
      }

      /// <inheritdoc/>
      public override void ResetValue(object component) {
         throw new NotSupportedException();
      }

      /// <inheritdoc/>
      public override bool ShouldSerializeValue(object component) {
         return true;
      }
   }
}