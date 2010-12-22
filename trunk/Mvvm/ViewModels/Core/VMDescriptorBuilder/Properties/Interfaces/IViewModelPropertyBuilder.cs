namespace Inspiring.Mvvm.ViewModels.Fluent {
   using System;
   using System.ComponentModel;
   using System.Linq.Expressions;
   using Inspiring.Mvvm.ViewModels.Core;

   /// <summary>
   ///   Provides a fluent interface to create <see cref="VMProperty"/> that 
   ///   hold child view models.
   /// </summary>
   /// <typeparam name="TSourceObject">
   ///   The type of source objects as selected by the <see 
   ///   cref="IVMPropertyBuilderProvider.GetPropertyBuilder"/> method call 
   ///   used to create the <see cref="IVMPropertyBuilder"/>.
   /// </typeparam>
   public interface IViewModelPropertyBuilder<TSourceObject> {
      /// <summary>
      ///   Selects the source value from which the child view model should be
      ///   initialized.
      /// </summary>
      /// <param name="sourceValueSelector">
      ///   <para>The property selected by this expression is read the first time 
      ///      when the VM property is accessed. The returned value is used to 
      ///      initialize the child view model that is held by this VM property.</para>
      ///   <para>If the VM property is set to a new view model its source object
      ///      is assigned to the selected source property.</para>
      ///   <para>The first argument of the expression ('x' in the example) is
      ///      the VM or some object referenced by it as selected by the 
      ///      <see cref="IVMPropertyBuilderProvider.GetPropertyBuilder"/> method
      ///      call used to create the <see cref="IVMPropertyBuilder"/>.</para>
      /// </param>
      IViewModelPropertyBuilderWithSource<TSourceValue> Wraps<TSourceValue>(
         Expression<Func<TSourceObject, TSourceValue>> sourceValueSelector
      );

      /// <summary>
      ///   Selects the source value from which the child view model should be
      ///   initialized.
      /// </summary>
      /// <param name="getter">
      ///   <para>A delegate that is called the first time the VM property is 
      ///      accessed. The returned value is used to initialize the child view
      ///      model that is held by this VM property.</para>
      ///   <para>The VM or some object referenced by it (as defined by the <see
      ///      cref="IVMPropertyBuilderProvider.GetPropertyBuilder"/> method call)
      ///      is passed to the delegate.</para>
      /// </param>
      /// <param name="setter">
      ///   <para>A delegate that is called when the VM property is set to a new 
      ///      view model.</para>
      ///   <para>The VM or some object referenced by it (as defined by the <see
      ///      cref="IVMPropertyBuilderProvider.GetPropertyBuilder"/> method call)
      ///      and the source object of the new VM is passed to the delegate.</para>
      /// </param>
      IViewModelPropertyBuilderWithSource<TSourceValue> Wraps<TSourceValue>(
         Func<TSourceObject, TSourceValue> getter,
         Action<TSourceObject, TSourceValue> setter
      );

      /// <summary>
      ///   Creates a simple <see cref="VMProperty"/> that holds a child view 
      ///   model that can be get and set.
      /// </summary>
      /// <param name="viewModelFactory">
      ///   A delegate that is called the first time the VM property is accessed 
      ///   and should return a new child VM instance.
      /// </param>
      [Obsolete("Use DelegatesTo")]
      VMProperty<TChildVM> CreatedBy<TChildVM>(
         Func<TSourceObject, TChildVM> viewModelFactory
      ) where TChildVM : IViewModel;

      VMProperty<TChildVM> DelegatesTo<TChildVM>(
         Func<TSourceObject, TChildVM> getter,
         Action<TSourceObject, TChildVM> setter = null
      ) where TChildVM : IViewModel;

      /// <summary>
      ///   Creates a simple <see cref="VMProperty"/> that holds a child view 
      ///   model that can be get and set.
      /// </summary>
      /// <typeparam name="TChildVM">
      ///   The type of the child VM (for example PersonVM).
      /// </typeparam>
      VMProperty<TChildVM> Of<TChildVM>() where TChildVM : IViewModel;

      // TODO: Comment
      [EditorBrowsable(EditorBrowsableState.Never)]
      VMProperty<TChildVM> Custom<TChildVM>(
         IValueAccessorBehavior<TChildVM> viewModelAccessor
      ) where TChildVM : IViewModel;

      // TODO: Comment
      [EditorBrowsable(EditorBrowsableState.Never)]
      VMProperty<TChildVM> Custom<TChildVM>(
         IViewModelFactoryBehavior<TChildVM> viewModelFactory
      ) where TChildVM : IViewModel;
   }
}
