namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.ComponentModel;
   using System.Linq;

   /// <summary>
   ///   A view model behavior that caches all <see cref="ViewModelPropertyDescriptor"/> 
   ///   objects of a <see cref="IVMDescriptor"/> object.
   /// </summary>
   public sealed class TypeDescriptorProviderBehavior :
      Behavior,
      IBehaviorInitializationBehavior {

      private PropertyDescriptorCollection _propertyDescriptors;
      private IVMDescriptor _vmDescriptor;

      public PropertyDescriptorCollection PropertyDescriptors {
         get {
            AssertInitialized();

            if (_propertyDescriptors == null) {
               _propertyDescriptors = new PropertyDescriptorCollection(
                  _vmDescriptor
                     .Properties
                     .Select(GetDescriptor)
                     .ToArray()
               );
            }

            return _propertyDescriptors;
         }
      }

      public void Initialize(BehaviorInitializationContext context) {
         _vmDescriptor = context.Descriptor;
         this.InitializeNext(context);
      }

      private void AssertInitialized() {
         Check.Requires<InvalidOperationException>(_vmDescriptor != null, "Behavior is not initalized.");
      }

      private static PropertyDescriptor GetDescriptor(IVMPropertyDescriptor property) {
         return property
            .Behaviors
            .GetNextBehavior<IPropertyDescriptorProviderBehavior>()
            .PropertyDescriptor;
      }
   }
}
