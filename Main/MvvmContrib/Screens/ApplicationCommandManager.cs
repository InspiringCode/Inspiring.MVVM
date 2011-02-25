namespace Inspiring.Mvvm.Screens {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using System.Windows.Input;

   // TODO: Test this class.

   /// <summary>
   ///   A service that provides view models and screens a way to register and 
   ///   unregister context-sensitive application commands identified by a key.
   ///   The 'Commands[commandKey]' property returns a proxy command for every
   ///   'commandKey' that is passed independent of whether an actual command
   ///   (one implemented by a view model or screen) is currently registered or
   ///   not. A proxy command is only created once for each 'commandKey'. This
   ///   means that the same proxy command instance is returned for every call
   ///   with the same 'commandKey' which does not depend on actual command
   ///   registrations.
   /// </summary>
   public sealed class ApplicationCommandManager {
      private Dictionary<object, CommandProxy> _commandProxies =
         new Dictionary<object, CommandProxy>();

      public ICommand this[object commandKey] {
         get {
            Contract.Requires<ArgumentNullException>(commandKey != null);
            return GetProxy(commandKey);
         }
      }

      public void RegisterCommand(object commandKey, ICommand actualImplementation) {
         Contract.Requires<ArgumentNullException>(commandKey != null);
         Contract.Requires<ArgumentNullException>(actualImplementation != null);
         GetProxy(commandKey).SetActualCommand(actualImplementation);
      }

      public void UnregisterCommand(object commandKey) {
         Contract.Requires<ArgumentNullException>(commandKey != null);
         GetProxy(commandKey).SetActualCommand(null);
      }

      private CommandProxy GetProxy(object commandKey) {
         CommandProxy proxy;

         if (!_commandProxies.TryGetValue(commandKey, out proxy)) {
            proxy = new CommandProxy();
            _commandProxies.Add(commandKey, proxy);
         }

         return proxy;
      }

      private class CommandProxy : ICommand {
         private ICommand _actual;
         EventHandler _strongReferenceToHandlerDelegate;

         public CommandProxy() {
            _strongReferenceToHandlerDelegate = new EventHandler(OnCanExecuteChanged);
         }

         public event EventHandler CanExecuteChanged;

         public void SetActualCommand(ICommand actual) {
            if (_actual != null) {
               _actual.CanExecuteChanged -= _strongReferenceToHandlerDelegate;
            }

            if (actual != null) {
               actual.CanExecuteChanged += _strongReferenceToHandlerDelegate;
            }

            _actual = actual;
            OnCanExecuteChanged(this, EventArgs.Empty);
         }

         public bool CanExecute(object parameter) {
            return _actual != null ?
               _actual.CanExecute(parameter) :
               false;
         }

         public void Execute(object parameter) {
            if (_actual == null) {
               throw new InvalidOperationException(
                  ExceptionTexts.ExecuteCalledWithoutActualCommand
               );
            }

            _actual.Execute(parameter);
         }

         private void OnCanExecuteChanged(object sender, EventArgs e) {
            EventHandler handler = CanExecuteChanged;
            if (handler != null) {
               handler(sender, e);
            }
         }
      }
   }
}
