namespace Inspiring.Mvvm.ViewModels.Core {

   internal static class BehaviorExtensionMethods {
      /// <summary>
      ///   Initializes the behavior chain for the given VM descriptor and 
      ///   optionally VM property.
      /// </summary>
      public static void Initialize(
         this BehaviorChain chain,
         VMDescriptorBase descriptor,
         IVMProperty property = null
      ) {
         var context = new BehaviorInitializationContext(descriptor, property);

         chain.TryCall<IBehaviorInitializationBehavior>(x =>
            x.Initialize(context)
         );
      }
   }
}
