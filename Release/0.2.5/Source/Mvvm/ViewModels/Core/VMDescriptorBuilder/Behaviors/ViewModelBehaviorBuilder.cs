﻿namespace Inspiring.Mvvm.ViewModels.Core.VMDescriptorBuilder.Behaviors {
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
         var template = BehaviorChainTemplateRegistry.GetTemplate(templateKey);
         var invoker = ViewModelBehaviorFactory.CreateInvoker<TVM>();
         var config = template.CreateConfiguration(invoker);

         _configuration.ViewModelConfiguration = config;
      }

      ViewModelBehaviorBuilder<TVM, TDescriptor> Enable(
         BehaviorKey key,
         IBehavior behaviorInstance
      ) {
         _configuration.ViewModelConfiguration.Enable(key, behaviorInstance);
         return this;
      }

      ViewModelBehaviorBuilder<TVM, TDescriptor> Configure<TBehavior>(
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
            BehaviorKeys.ManualUpdateCoordinator,
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
            BehaviorKeys.ManualUpdateCoordinator,
            (LoadOrderBehavior behavior) => {
               behavior.UpdateSourceProperties = orderedProperties
                  .Select(selector => selector(_descriptor))
                  .ToArray();
            }
         );
      }



      public void AddChangeHandler(Action<TVM, ChangeArgs, InstancePath> changeHandler) {
         // TODO: Make this more official...
         var key = new BehaviorKey("ChangeListener");
         _configuration.ViewModelConfiguration.Append(key);
         _configuration.ViewModelConfiguration.Enable(
            key,
            new ChangeListenerBehavior<TVM>(changeHandler)
         );
      }
   }
}