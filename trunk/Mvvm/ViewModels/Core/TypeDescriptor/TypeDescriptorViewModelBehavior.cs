namespace Inspiring.Mvvm.ViewModels.Core {
   using System.ComponentModel;
   using System.Diagnostics.Contracts;
   using System.Linq;

   /// <summary>
   ///   A view model behavior that caches all <see cref="VMPropertyDescriptor"/> 
   ///   objects of a <see cref="VMDescriptorBase"/> object.
   /// </summary>
   public sealed class TypeDescriptorViewModelBehavior : 
      Behavior,
      IBehaviorInitializationBehavior {

      private PropertyDescriptorCollection _propertyDescriptors;
      private VMDescriptorBase _vmDescriptor;
      
      public PropertyDescriptorCollection PropertyDescriptors {
         get {
            AssertInitialized();

            if (_propertyDescriptors == null) {
               _propertyDescriptors = new PropertyDescriptorCollection(
                  _vmDescriptor
                     .Properties
                     .Select(p => new VMPropertyDescriptor(p))
                     .ToArray()
               );
            }

            return _propertyDescriptors;
         }
      }

      public new void Initialize(BehaviorInitializationContext context) {
         _vmDescriptor = context.Descriptor;
      }

      private void AssertInitialized() {
         Contract.Assert(_vmDescriptor != null, "Behavior is not initalized.");
      }
   }
}
