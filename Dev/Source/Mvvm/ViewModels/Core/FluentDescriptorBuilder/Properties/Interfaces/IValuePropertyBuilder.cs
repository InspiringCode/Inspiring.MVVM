namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Linq.Expressions;

   /// <summary>
   ///   Provides a fluent interface to create <see cref="IVMPropertyDescriptor"/> that 
   ///   hold simple values (objects, strings, value types).
   /// </summary>
   /// <typeparam name="TSourceObject">
   ///   The type of source objects as selected by the <see 
   ///   cref="IVMPropertyBuilderProvider.GetPropertyBuilder"/> method call 
   ///   used to create the <see cref="IVMPropertyBuilder"/>.
   /// </typeparam>
   public interface IValuePropertyBuilder<TSourceObject> {
      /// <summary>
      ///   Creates a <see cref="IVMPropertyDescriptor"/> that reads and sets the value of
      ///   standard property declared on the source object.
      /// </summary>
      /// <param name="sourcePropertySelector">
      ///   <para>An expression of the form 'x => x.Person.Age' that specifies 
      ///      the path of the property whose value should be returned and set 
      ///      by the returned property.</para>
      ///   <para>The first argument of the expression ('x' in the example) is
      ///      the VM or some object referenced by it as selected by the 
      ///      <see cref="IVMPropertyBuilderProvider.GetPropertyBuilder"/> method
      ///      call used to create the <see cref="IVMPropertyBuilder"/>.</para>
      /// </param>
      /// <remarks>
      ///   <para>Every time the VM property is accessed on a VM the getter of 
      ///      the mapped property of the VMs source object is called. Every time 
      ///      the VM property is set on a VM the setter of the mapped property 
      ///      of the VMs source object is called.</para>
      ///   <para>A mapped property may also map to a complex property path of 
      ///      the source object, such as Invoice.Customer.Address.Street. In 
      ///      this case all properties are read in sequence and the value of 
      ///      the last property is returned or set on the source object. The
      ///      default value of <typeparamref name="T"/> is returned if one of
      ///      properties in the path returns null.</para>
      /// </remarks>
      IVMPropertyDescriptor<T> MapsTo<T>(Expression<Func<TSourceObject, T>> sourcePropertySelector);

      /// <summary>
      ///   Creats a <see cref="IVMPropertyDescriptor"/> that calls a delegate when the VM 
      ///   property is read or set.
      /// </summary>
      /// <param name="getter">
      ///   <para>A delegate that is called by the <see cref="IVMPropertyDescriptor"/> to 
      ///      get its value.</para>
      ///   <para>The VM or some object referenced by it (as defined by the <see
      ///      cref="IVMPropertyBuilderProvider.GetPropertyBuilder"/> method call)
      ///      is passed to the delegate.</para>
      ///   <para>Example: 'vm => vm.Person.CalculateFee(vm.CurrentProject)'.</para>
      /// </param>
      /// <param name="setter">
      ///   <para>A delegate that is called by the <see cref="IVMPropertyDescriptor"/> when 
      ///      its value is set.</para>
      ///   <para>The VM or some object referenced by it (as defined by the <see 
      ///      cref="IVMPropertyFactoryProvider.GetPropertyBuilder"/> method call)
      ///      and the new value is passed to the delegate.</para>
      ///   <para>Example: 
      ///      '(vm, value) => vm.Person.UpdateFee(vm.CurrentProject, value)'.
      ///   </para>
      /// </param>
      /// <remarks>
      ///   If no setter is specified, the VM property is readonly and throws an
      ///   exception if its value is set.
      /// </remarks>
      IVMPropertyDescriptor<T> DelegatesTo<T>(
         Func<TSourceObject, T> getter,
         Action<TSourceObject, T> setter = null
      );

      /// <summary>
      ///   Creates a <see cref="IVMPropertyDescriptor"/> that stores its value in the VM.
      ///   It is similar to a normal get/set property but enhanced with all VM 
      ///   property features.
      /// </summary>
      /// <typeparam name="T">
      ///   The type of the property (e.g. <see cref="System.String"/>).
      /// </typeparam>
      IVMPropertyDescriptor<T> Of<T>();
   }
}
