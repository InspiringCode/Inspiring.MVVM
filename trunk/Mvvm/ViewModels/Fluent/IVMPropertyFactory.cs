namespace Inspiring.Mvvm.ViewModels.Fluent {
   using System;
   using System.Collections.Generic;
   using System.Linq.Expressions;
   using System.Windows.Input;

   /// <summary>
   ///   Creates <see cref="VMProperty"/> objects with different underlying
   ///   value sources/stores.
   /// </summary>
   /// <typeparam name="TVM">
   ///   The type of the <see cref="IViewModel"/> for which this factory creates
   ///   properties.
   /// </typeparam>
   /// <typeparam name="TSource">
   ///   If this factory was created with a different source object than the 
   ///   VM itself the type of the actual source object for the mapped and 
   ///   calculated properties is different than <typeparamref name="TVM"/> 
   ///   (see <see cref="IVMPropertyFactoryProvider.GetFactory"/>).
   /// </typeparam>
   public interface IVMPropertyFactory<TVM, TSource> where TVM : IViewModel {
      /// <summary>
      ///   Creates a <see cref="VMProperty"/> that reads and sets the value of
      ///   standard property declared on the the source object.
      /// </summary>
      /// <param name="sourcePropertySelector">
      ///   <para>An expression of the form 'x => x.Person.Age' that specifies 
      ///      the property path of the property whose value should be returned
      ///      and set by the returned property.</para>
      ///   <para>The first argument of the expression ('x' in the example) is
      ///      the the <see cref="IViewModel"/> or some object referenced by it 
      ///      as defined by the <see cref="IVMPropertyFactoryProvider.GetFactory"/>
      ///      method.</para>
      /// </param>
      /// <remarks>
      ///   <para>Every time the VM property is accessed on a VM the getter of 
      ///      mapped property of the VMs source object is called. Every time 
      ///      the VM property is set on a VM the setter of the mapped property 
      ///      of the VMs source object is called.</para>
      ///   <para>A mapped property may also map to a complex property path of 
      ///      the source object, such as Invoice.Customer.Address.Street. In 
      ///      this case all properties are read in sequence and the value of 
      ///      the last property is returned or set on the sourceobject.</para>
      /// </remarks>
      IVMPropertyFactoryWithSource<TVM, T> Mapped<T>(Expression<Func<TSource, T>> sourcePropertySelector);

      /// <summary>
      ///   Creats a <see cref="VMProperty"/> that calls a delegate when the VM 
      ///   property is read or set.
      /// </summary>
      /// <param name="getter">
      ///   <para>A delegate that is called by the <see cref="VMProperty"/> to 
      ///      get its value.</para>
      ///   <para>The <see cref="IViewModel"/> or some object referenced by it (as 
      ///      defined by the <see cref="IVMPropertyFactoryProvider.GetFactory"/>
      ///      method) is passed to the delegate.</para>
      ///   <para>Example: 'vm => vm.Person.GetFee(vm.CurrentProject)'.</para>
      /// </param>
      /// <param name="setter">
      ///   <para>A delegate that is called by the <see cref="VMProperty"/> when 
      ///      its value is set.</para>
      ///   <para>The <see cref="IViewModel"/> or some object referenced by it (as 
      ///      defined by the <see cref="IVMPropertyFactoryProvider.GetFactory"/>
      ///      method) and the new value is passed to the delegate.</para>
      ///   <para>Example: 
      ///      '(vm, value) => vm.Person.UpdateFee(vm.CurrentProject, value)'.
      ///   </para>
      /// </param>
      /// <remarks>
      ///   If no setter is specified, the VM property is readonly and throws an
      ///   exception if its value is set.
      /// </remarks>
      IVMPropertyFactoryWithSource<TVM, T> Calculated<T>(Func<TSource, T> getter, Action<TSource, T> setter = null);

      /// <summary>
      ///   Creates a <see cref="VMProperty"/> that stores its value in the VM.
      ///   It is similar to a normal get/set property but enhanced with all VM 
      ///   property features.
      /// </summary>
      /// <typeparam name="T">
      ///   The type of the property (e.g. <see cref="System.String"/>).
      /// </typeparam>
      ILocalVMPropertyFactory<TVM> Local();

      /// <summary>
      ///   Creates a local <see cref="VMProperty"/> of type <see cref="IVMCollection"/>
      ///   that stores its value in the VM. A collection property ensures that its 
      ///   item VMs are properly initialized (for example its Parent is set).
      /// </summary>
      IVMCollectionPropertyFactory<TVM> Collection();

      /// <summary>
      ///   Creates a <see cref="VMProperty"/> of type <see cref="IVMCollection"/>
      ///   whos items are synchronized with a source collection returned by the 
      ///   passed <paramref name="sourceCollectionSelector"/>. A collection property 
      ///   ensures that its item VMs are properly initialized (for example its 
      ///   Parent is set).
      /// </summary>
      /// <param name="sourceCollectionSelector">
      ///   <para>A function that should return the source collection with which 
      ///      the <see cref="IVMCollection"/> is synchronized. This may be the
      ///      value of a source object collection or you may create and return
      ///      a new collection instance.</para> 
      ///   <para>The <see cref="IViewModel"/> or some object referenced by it (as 
      ///      defined by the <see cref="IVMPropertyFactoryProvider.GetFactory"/>
      ///      method) is passed to the delegate.</para>  
      /// </param>
      IVMCollectionPropertyFactoryWithSource<TVM, TItemSource> Collection<TItemSource>(
         Func<TSource, IEnumerable<TItemSource>> sourceCollectionSelector
      );

      /// <summary>
      ///   Creates a <see cref="VMProperty"/> of type <see cref="ICommand"/>.
      /// </summary>
      /// <param name="execute">
      ///   A delegate that is called when the command is executed.
      /// </param>
      /// <param name="canExecute">
      ///   A delegate taht is called to check whether the command can currently
      ///   be executed.
      /// </param>
      VMProperty<ICommand> Command(Action<TSource> execute, Func<TSource, bool> canExecute = null);
   }
}
