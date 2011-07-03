namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;

   /// <summary>
   ///   <para>Stores which VM property of a VM descriptor class was selected.</para>
   ///   <example>
   ///      <para>There are two instances of 'StatusVM' which defines the VM property
   ///         'Title'. This means there are also two VM property instances for the
   ///         'Title' property. The <see cref="PropertySelector"/> allows us to refer 
   ///         to the 'Title' property independently of VM descriptor instance.</para>
   ///      <para>'PropertySelector.Create((PersonVMDescriptor x) => x.Name)' gives
   ///         us a <see cref="PropertySelector"/> instance that returns the first VM 
   ///         property instance if we pass in the first descriptor to the <see 
   ///         cref="GetProperty"/> method and the VM property instance of 
   ///         and the second descriptor if we pass the second descriptor.</para>
   ///   </example>
   /// </summary>
   public abstract class PropertySelector {
      public static PropertySelector Create<TDescriptor>(
         Func<TDescriptor, IVMPropertyDescriptor> propertySelector
      ) where TDescriptor : IVMDescriptor {
         Contract.Requires(propertySelector != null);
         return new NonGenericPropertySelector<TDescriptor>(propertySelector);
      }

      public static PropertySelector CreateExactlyTyped<TDescriptor, TValue>(
         Func<TDescriptor, IVMPropertyDescriptor<TValue>> propertySelector
      ) where TDescriptor : IVMDescriptor {
         Contract.Requires(propertySelector != null);
         return new GenericPropertySelector<TDescriptor, TValue>(propertySelector);
      }

      /// <summary>
      ///   Returns the VM property of the given <paramref name="descriptor"/>
      ///   that represents the VM property selected by this <see 
      ///   cref="PropertySelector"/>.
      /// </summary>
      /// <param name="descriptor">
      ///   The descriptor instance whos VM property instance should be returned.
      /// </param>
      public abstract IVMPropertyDescriptor GetProperty(IVMDescriptor descriptor);

      public abstract object GetPropertyValue(IViewModel viewModel);

      private sealed class NonGenericPropertySelector<TDescriptor> :
         PropertySelector
         where TDescriptor : IVMDescriptor {

         private PropertySelector _genericSelector;
         private readonly Func<TDescriptor, IVMPropertyDescriptor> _selector;

         public NonGenericPropertySelector(Func<TDescriptor, IVMPropertyDescriptor> selector) {
            Contract.Requires(selector != null);
            _selector = selector;
         }

         public override IVMPropertyDescriptor GetProperty(IVMDescriptor descriptor) {
            var concreteDescriptor = (TDescriptor)descriptor;
            return _selector(concreteDescriptor);
         }

         /// <remarks>
         ///   <para>Because we can only get the value of a property if we know its type
         ///      at COMPILE time, we use a little hack: The first time this method is 
         ///      called, the descriptor of the passed in view model is used to determine
         ///      the type of the property. Reflection is then used to create a strongly
         ///      type selector, to which <see cref="GetPropertyValue"/> operations are
         ///      delegated.</para>
         ///   <para>Design trade-off: We could have avoided this hack if we had added
         ///      a 'GetValueAsObject' method to the <see cref="IVMPropertyDescriptor"/>
         ///      interface but it was decided to keep the public interface clean.</para>
         /// </remarks>
         public override object GetPropertyValue(IViewModel viewModel) {
            if (_genericSelector == null) {
               InitializeGenericSelector(viewModel.Descriptor);
            }

            return _genericSelector.GetPropertyValue(viewModel);
         }

         /// <param name="arbitraryDescriptor">
         ///   Because the type of the property selected by this object always
         ///   has to be the same, any instance can be used for this purpose.
         /// </param>
         private void InitializeGenericSelector(IVMDescriptor arbitraryDescriptor) {
            IVMPropertyDescriptor property = GetProperty(arbitraryDescriptor);
            _genericSelector = CreateGenericSelector(property.PropertyType);
         }

         private PropertySelector CreateGenericSelector(Type propertyType) {
            Type type = MakeConcreateGenericSelectorType(propertyType);
            var constructorArgs = new[] { _selector };

            return (PropertySelector)Activator.CreateInstance(type, constructorArgs);
         }

         private Type MakeConcreateGenericSelectorType(Type propertyType) {
            var genericSelectorType = typeof(GenericPropertySelector<,>);
            var typeArguments = new[] { typeof(TDescriptor), propertyType };

            return genericSelectorType.MakeGenericType(typeArguments);
         }
      }

      /// <summary>
      ///   Encapsulates the generic argument needed to call the 'selector' 
      ///   function.
      /// </summary>
      private sealed class GenericPropertySelector<TDescriptor, TValue> :
         PropertySelector
         where TDescriptor : IVMDescriptor {

         private readonly Func<TDescriptor, IVMPropertyDescriptor> _selector;

         public GenericPropertySelector(Func<TDescriptor, IVMPropertyDescriptor> selector) {
            Contract.Requires(selector != null);
            _selector = selector;
         }

         public override IVMPropertyDescriptor GetProperty(IVMDescriptor descriptor) {
            var concreteDescriptor = (TDescriptor)(object)descriptor;
            return _selector(concreteDescriptor);
         }

         public override object GetPropertyValue(IViewModel viewModel) {
            var typedProperty = (IVMPropertyDescriptor<TValue>)GetProperty(viewModel.Descriptor);
            return viewModel.Kernel.GetValue(typedProperty);
         }
      }
   }
}