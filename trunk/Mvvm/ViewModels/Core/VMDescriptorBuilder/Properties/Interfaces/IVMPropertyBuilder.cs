namespace Inspiring.Mvvm.ViewModels.Fluent {
   using System;
   using System.Windows.Input;
   using Inspiring.Mvvm.Common;

   /// <summary>
   ///   Provides a fluent interface to create <see cref="VMProperty"/> objects.
   /// </summary>
   /// <typeparam name="TSourceObject">
   ///   The type of source objects as selected by the <see 
   ///   cref="IVMPropertyBuilderProvider.GetPropertyBuilder"/> method call 
   ///   used to create the <see cref="IVMPropertyBuilder"/>.
   /// </typeparam>
   public interface IVMPropertyBuilder<TSourceObject> : IHideObjectMembers, IConfigurationProvider {
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
      VMProperty<ICommand> Command(
         Action<TSourceObject> execute,
         Func<TSourceObject, bool> canExecute = null
      );
   }
}
