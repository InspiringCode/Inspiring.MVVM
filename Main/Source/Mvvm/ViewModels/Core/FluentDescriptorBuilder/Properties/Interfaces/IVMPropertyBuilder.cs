namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Windows.Input;
   using Inspiring.Mvvm.Common;

   /// <summary>
   ///   Provides a fluent interface to create <see cref="IVMPropertyDescriptor"/> objects.
   /// </summary>
   /// <typeparam name="TSourceObject">
   ///   The type of source objects as selected by the <see 
   ///   cref="IVMPropertyBuilderProvider.GetPropertyBuilder"/> method call 
   ///   used to create the <see cref="IVMPropertyBuilder"/>.
   /// </typeparam>
   public interface IVMPropertyBuilder<TSourceObject> : ICustomPropertyFactoryProvider<TSourceObject>, IHideObjectMembers, IConfigurationProvider {
      /// <summary>
      ///   Creates a <see cref="IVMPropertyDescriptor"/> that holds a simple value (an object,
      ///   string or value type).
      /// </summary>
      IValuePropertyBuilder<TSourceObject> Property { get; }

      /// <summary>
      ///   Creates a <see cref="IVMPropertyDescriptor"/> that holds a child view model and
      ///   ensures that the child VM is properly initialized (for example its
      ///   parent is set).
      /// </summary>
      IViewModelPropertyBuilder<TSourceObject> VM { get; }

      /// <summary>
      ///   Creates a <see cref="IVMPropertyDescriptor"/> that holds a <see cref="IVMCollection"/>
      ///   of chlild view models and ensures that its item VMs are properly 
      ///   initialized (for example its parent is set).
      /// </summary>
      ICollectionPropertyBuilder<TSourceObject> Collection { get; }

      /// <summary>
      ///   Creates a <see cref="IVMPropertyDescriptor"/> of type <see cref="ICommand"/>.
      /// </summary>
      /// <param name="executeAction">
      ///   A delegate that is called when the command is executed.
      /// </param>
      /// <param name="canExecutePredicate">
      ///   A delegate taht is called to check whether the command can currently
      ///   be executed.
      /// </param>
      IVMPropertyDescriptor<ICommand> Command(
         Action<TSourceObject> executeAction,
         Func<TSourceObject, bool> canExecutePredicate = null
      );
   }

   /// <inheritdoc />
   public interface IVMPropertyBuilder<TOwnerVM, TSourceObject> :
      IVMPropertyBuilder<TSourceObject>
      where TOwnerVM : IViewModel {
   }
}
