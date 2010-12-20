namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Diagnostics.Contracts;
   using Contracts;

   /// <summary>
   ///   A property behavior that creates new, empty <see cref="IVMCollection"/>
   ///   instances.
   /// </summary>
   // TODO: Refactor this? Own behavior? See also VMBehaviorBuilder.
   [ContractClass(typeof(ICollectionFactoryBehaviorContract))]
   internal interface ICollectionBehaviorConfigurationBehavior : IBehavior {
      /// <summary>
      ///   <para>Gets the <see cref="BehaviorChainConfiguration"/> that is used 
      ///      for the VM collection instances (see <see cref="IVMCollection"/> 
      ///      for more details).</para>
      ///   <para>Note that you cannot modify this configuration after the first 
      ///      collection has been created by <see cref="CreateCollection"/>. Also
      ///      note that the same <see cref="BehaviorChain"/> instance is used for
      ///      all collection instances created by this factory.</para>
      /// </summary>
      BehaviorChainConfiguration CollectionBehaviorConfiguration { get; }
   }

   namespace Contracts {
      /// <inheritdoc />
      [ContractClassFor(typeof(ICollectionBehaviorConfigurationBehavior))]
      internal abstract class ICollectionFactoryBehaviorContract :
         ICollectionBehaviorConfigurationBehavior {

         /// <inheritdoc />
         public BehaviorChainConfiguration CollectionBehaviorConfiguration {
            get {
               Contract.Ensures(Contract.Result<BehaviorChainConfiguration>() != null);
               return null;
            }
         }

         /// <inheritdoc />
         public IBehavior Successor {
            get { return null; }
            set { }
         }
      }
   }
}
