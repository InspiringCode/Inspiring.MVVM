namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;
   using System.Linq;

   // TODO: Document me.
   public sealed class ViewModelBehaviorBuilder<TVM, TDescriptor>
      where TDescriptor : IVMDescriptor
      where TVM : IViewModel {

      private VMDescriptorConfiguration _configuration;
      private TDescriptor _descriptor;

      internal ViewModelBehaviorBuilder(VMDescriptorConfiguration configuration, TDescriptor descriptor) {
         Contract.Requires(configuration != null);
         Contract.Requires(configuration.ViewModelConfiguration != null);

         _configuration = configuration;
         _descriptor = descriptor;
      }

      public void ReplaceConfiguration(BehaviorChainTemplateKey templateKey) {
         _configuration.ViewModelConfiguration = BehaviorChainConfiguration.GetConfiguration(
            templateKey,
            BehaviorFactoryConfigurations.ForViewModel<TVM>()
         );
      }

      // Todo: Should this be public ?
      public ViewModelBehaviorBuilder<TVM, TDescriptor> Enable(
        BehaviorKey key,
        IBehavior behaviorInstance = null
      ) {
         _configuration.ViewModelConfiguration.Enable(key, behaviorInstance);
         return this;
      }

      // Todo: Should this be public ?
      public ViewModelBehaviorBuilder<TVM, TDescriptor> Configure<TBehavior>(
         BehaviorKey key,
         Action<TBehavior> configurationAction
      ) where TBehavior : IBehavior {
         _configuration.ViewModelConfiguration.ConfigureBehavior<TBehavior>(key, configurationAction);
         return this;
      }

      public ViewModelBehaviorBuilder<TVM, TDescriptor> OverrideUpdateFromSourceProperties(
         params Func<TDescriptor, IVMPropertyDescriptor>[] orderedProperties
      ) {
         return Configure(
            ViewModelBehaviorKeys.LoadOrderController,
            (LoadOrderBehavior behavior) => {
               behavior.UpdateFromSourceProperties = orderedProperties
                  .Select(selector => selector(_descriptor))
                  .ToArray();
            }
         );
      }

      public ViewModelBehaviorBuilder<TVM, TDescriptor> OverrideUpdateSourceProperties(
          params Func<TDescriptor, IVMPropertyDescriptor>[] orderedProperties
       ) {
         return Configure(
            ViewModelBehaviorKeys.LoadOrderController,
            (LoadOrderBehavior behavior) => {
               behavior.UpdateSourceProperties = orderedProperties
                  .Select(selector => selector(_descriptor))
                  .ToArray();
            }
         );
      }

      public void AddChangeHandler(Action<TVM, ChangeArgs> changeHandler) {
         // TODO: Make this more official...
         var key = new BehaviorKey("ChangeListener");
         _configuration.ViewModelConfiguration.Append(key);
         _configuration.ViewModelConfiguration.Enable(
            key,
            new ChangeListenerBehavior<TVM>(changeHandler)
         );
      }

      public void EnableUndo() {
         foreach (var property in _configuration.PropertyConfigurations.ConfiguredProperties) {
            var config = _configuration.PropertyConfigurations[property];
            if (config.Contains(PropertyBehaviorKeys.Undo)) {
               config.Enable(PropertyBehaviorKeys.Undo);
            }
         }
      }

      public void IsUndoRoot() {
         Enable(ViewModelBehaviorKeys.UndoRoot);
         EnableUndo();
      }
   }
}
