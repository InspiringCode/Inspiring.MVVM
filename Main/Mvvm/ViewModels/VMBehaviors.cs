namespace Inspiring.Mvvm.ViewModels {
   using Inspiring.Mvvm.ViewModels.Behaviors;

   /// <summary>
   ///   A class that holds all available <see cref="VMBehavior"/>s that can be
   ///   enabled on a <see cref="VMDescriptor"/> instance. Use the 'WithBehaviors'
   ///   method of the <see cref="VMDescriptorBuilder"/> fluent interface.
   /// </summary>
   public static class VMBehaviors {
      public static readonly VMBehaviorFactory DisconnectedViewModel = new DisconnectedViewModelFactory();

      private class DisconnectedViewModelFactory : VMBehaviorFactory {
         public DisconnectedViewModelFactory()
            : base(typeof(CacheValueBehavior<>)) {
         }

         public override IBehavior Create<TValue>() {
            return new CacheValueBehavior<TValue>(BehaviorPosition.DisconnectedValueHolder);
         }
      }
   }
}
