namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.ComponentModel;
   
   internal abstract class PropertyDescriptorDecorator<T> :
      PropertyDescriptor
      where T : PropertyDescriptor {

      private static readonly Attribute[] EmptyAttributeArray = new Attribute[0];

      public PropertyDescriptorDecorator(T decorated)
         : base(decorated.Name, ToArray(decorated.Attributes)) {
         Check.NotNull(decorated, nameof(decorated));
         Decorated = decorated;
      }

      public override Type ComponentType {
         get { return Decorated.ComponentType; }
      }

      public override bool IsReadOnly {
         get { return Decorated.IsReadOnly; }
      }

      public override object GetValue(object component) {
         return Decorated.GetValue(component);
      }

      public override Type PropertyType {
         get { return Decorated.PropertyType; }
      }

      protected T Decorated {
         get;
         private set;
      }

      public override void SetValue(object component, object value) {
         Decorated.SetValue(component, value);
      }

      public override bool CanResetValue(object component) {
         return Decorated.CanResetValue(component);
      }

      public override void ResetValue(object component) {
         Decorated.ResetValue(component);
      }

      public override bool ShouldSerializeValue(object component) {
         return Decorated.ShouldSerializeValue(component);
      }

      private static Attribute[] ToArray(AttributeCollection attributes) {
         if (attributes.Count == 0) {
            return EmptyAttributeArray;
         }

         var array = new Attribute[attributes.Count];
         attributes.CopyTo(array, 0);
         return array;
      }
   }
}
