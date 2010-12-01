namespace Inspiring.Mvvm.ViewModels.Core.Behaviors {
   using System;

   internal sealed class BehaviorChainConfigurationCollection {
      public BehaviorChainConfiguration this[VMPropertyBase forProperty] {
         get {
            throw new NotImplementedException();
         }
      }

      public void RegisterProperty<TValue>(VMPropertyBase<TValue> property, BehaviorChainTemplateKey behavior) {

      }

      /// <summary>
      ///   Calls <see cref="BehaviorChainConfiguration.ConfigureBehavior"/> on
      ///   all configurations contained by this collection that contain the 
      ///   specified '<paramref name="key"/>'.
      /// </summary>
      public void ConfigureBehavior<T>(
         BehaviorKey key,
         Action<T> configurationAction
      ) where T : IBehavior {
         throw new NotImplementedException();
      }

      /// <summary>
      ///   Calls <see cref="BehaviorChainConfiguration.Enable(BehaviorKey)"/> on
      ///   all configurations contained by this collection that contain the 
      ///   specified '<paramref name="key"/>'.
      /// </summary>
      public void Enable(BehaviorKey key) {
         throw new NotImplementedException();
      }
   }
}
