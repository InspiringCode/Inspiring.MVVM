namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;
   using System.Windows.Input;

   public class CommandFactoryBehavior :
      Behavior,
      ICommandBehaviorConfigurationBehavior,
      IValueAccessorBehavior<ICommand> {

      private BehaviorChain _commandBehaviors;

      /// <param name="commandBehaviorConfiguration">
      ///   Note that the passed <paramref name="commandBehaviorConfiguration"/>
      ///   can be modified until <see cref="GetValue"/> is called the first
      ///   time.
      /// </param>
      public CommandFactoryBehavior(BehaviorChainConfiguration commandBehaviorConfiguration) {
         Contract.Requires<ArgumentNullException>(commandBehaviorConfiguration != null);
         CommandBehaviorConfiguration = commandBehaviorConfiguration;
      }

      /// <inheritdoc />
      public BehaviorChainConfiguration CommandBehaviorConfiguration {
         get;
         private set;
      }

      public ICommand GetValue(IBehaviorContext context) {
         if (_commandBehaviors == null) {
            _commandBehaviors = CommandBehaviorConfiguration.CreateChain();
            CommandBehaviorConfiguration.Seal();
            Seal();
         }

         return CreateCommand(context.VM, _commandBehaviors);
      }

      public void SetValue(IBehaviorContext context, ICommand value) {
         throw new NotSupportedException();
      }

      protected virtual ICommand CreateCommand(IViewModel ownerViewModel, BehaviorChain commandBehaviors) {
         return new ViewModelCommand(ownerViewModel, commandBehaviors);
      }
   }
}
