namespace Inspiring.Mvvm.Screens {
   using System;
   using System.Collections.Generic;
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

      public event EventHandler RegisteredCommandsChanged;

      public ICommand this[object commandKey] {
         get {
            Check.NotNull(commandKey, nameof(commandKey));
            return GetProxy(commandKey);
         }
      }

      public void RegisterCommand(object commandKey, ICommand actualImplementation) {
         Check.NotNull(commandKey, nameof(commandKey));
         Check.NotNull(actualImplementation, nameof(actualImplementation));
         GetProxy(commandKey).SetActualCommand(actualImplementation);
         RaiseRegisteredCommandsChanged();
      }

      public void UnregisterCommand(object commandKey) {
         Check.NotNull(commandKey, nameof(commandKey));
         GetProxy(commandKey).SetActualCommand(null);
         RaiseRegisteredCommandsChanged();
      }

      private CommandProxy GetProxy(object commandKey) {
         CommandProxy proxy;

         if (!_commandProxies.TryGetValue(commandKey, out proxy)) {
            proxy = new CommandProxy();
            _commandProxies.Add(commandKey, proxy);
         }

         return proxy;
      }

      private void RaiseRegisteredCommandsChanged() {
         EventHandler handler = this.RegisteredCommandsChanged;
         if (handler != null) {
            handler(this, EventArgs.Empty);
         }
      }
   }
}