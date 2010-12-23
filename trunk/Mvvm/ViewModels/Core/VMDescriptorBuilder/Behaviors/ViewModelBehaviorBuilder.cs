namespace Inspiring.Mvvm.ViewModels.Core.VMDescriptorBuilder.Behaviors {
   using System;
   using System.Diagnostics.Contracts;

   // TODO: Document me.
   public sealed class ViewModelBehaviorBuilder<TVM> where TVM : IViewModel {
      private VMDescriptorConfiguration _configuration;

      internal ViewModelBehaviorBuilder(VMDescriptorConfiguration configuration) {
         Contract.Requires(configuration != null);
         Contract.Requires(configuration.ViewModelConfiguration != null);

         _configuration = configuration;
      }

      public void ReplaceConfiguration(BehaviorChainTemplateKey templateKey) {
         var template = BehaviorChainTemplateRegistry.GetTemplate(templateKey);
         var invoker = ViewModelBehaviorFactory.CreateInvoker<TVM>();
         var config = template.CreateConfiguration(invoker);

         _configuration.ViewModelConfiguration = config;
      }

      ViewModelBehaviorBuilder<TVM> Enable(
         BehaviorKey key,
         IBehavior behaviorInstance
      ) {
         _configuration.ViewModelConfiguration.Enable(key, behaviorInstance);
         return this;
      }

      ViewModelBehaviorBuilder<TVM> Configure<TBehavior>(
         BehaviorKey key,
         Action<TBehavior> configurationAction
      ) where TBehavior : IBehavior {
         _configuration.ViewModelConfiguration.ConfigureBehavior<TBehavior>(key, configurationAction);
         return this;
      }
   }
}
