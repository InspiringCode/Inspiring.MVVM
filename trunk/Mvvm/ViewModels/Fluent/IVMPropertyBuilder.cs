﻿namespace Inspiring.Mvvm.ViewModels.Fluent {
   /// <summary>
   ///   Provides a fluent interface to create <see cref="VMProperty"/> objects.
   /// </summary>
   /// <typeparam name="TSourceObject">
   ///   The type of source objects as selected by the <see 
   ///   cref="IVMPropertyBuilderProvider.GetPropertyBuilder"/> method call 
   ///   used to create the <see cref="IVMPropertyBuilder"/>.
   /// </typeparam>
   public interface IVMPropertyBuilder<TSourceObject> {
      /// <summary>
      ///   Creates a <see cref="VMProperty"/> that holds a simple value (an object,
      ///   string or value type).
      /// </summary>
      IValuePropertyBuilder<TSourceObject> Property { get; }

      /// <summary>
      ///   Creates a <see cref="VMProperty"/> that holds a child view model and
      ///   ensures that the child VM is properly initialized (for example its
      ///   parent is set).
      /// </summary>
      IViewModelPropertyBuilder<TSourceObject> VM { get; }

      /// <summary>
      ///   Creates a <see cref="VMProperty"/> that holds a <see cref="IVMCollection"/>
      ///   of chlild view models and ensures that its item VMs are properly 
      ///   initialized (for example its parent is set).
      /// </summary>
      ICollectionPropertyBuilder<TSourceObject> Collection { get; }
   }
}