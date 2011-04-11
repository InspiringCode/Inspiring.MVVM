namespace Inspiring.Mvvm.ViewModels.Core {
   using System.ComponentModel;
   using System.Diagnostics.Contracts;
   using System.Linq;

   /// <summary>
   ///   A view model behavior that caches all <see cref="TypeDescriptorPropertyDescriptor"/> 
   ///   objects of a <see cref="VMDescriptorBase"/> object.
   /// </summary>
   public sealed class TypeDescriptorBehavior :
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
         Contract.Assert(_vmDescriptor != null, "Behavior is not initalized.");
      }

      private static TypeDescriptorPropertyDescriptor GetDescriptor(IVMPropertyDescriptor property) {
         return property
            .Behaviors
            .GetNextBehavior<PropertyDescriptorBehavior>()
            .PropertyDescriptor;
      }
   }
}
