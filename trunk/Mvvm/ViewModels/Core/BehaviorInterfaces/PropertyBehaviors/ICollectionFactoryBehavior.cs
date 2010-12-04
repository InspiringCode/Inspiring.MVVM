namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;
   using Contracts;

   /// <summary>
   ///   A property behavior that creates new, empty <see cref="IVMCollection"/>
   ///   instances.
   /// </summary>
   [ContractClass(typeof(ICollectionFactoryBehaviorContract<>))]
   internal interface ICollectionFactoryBehavior<TItemVM> : IBehavior where TItemVM : IViewModel {
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

      /// <summary>
      ///   Returns a new <see cref="IVMCollection"/> instance.
      /// </summary>
      IVMCollection<TItemVM> CreateCollection(IBehaviorContext context);
   }

   namespace Contracts {
      /// <inheritdoc />
      [ContractClassFor(typeof(ICollectionFactoryBehavior<>))]
      internal abstract class ICollectionFactoryBehaviorContract<TItemVM> :
         ICollectionFactoryBehavior<TItemVM>
         where TItemVM : IViewModel {

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

         /// <inheritdoc />
         public IVMCollection<TItemVM> CreateCollection(IBehaviorContext context) {
            Contract.Requires(context != null);
            Contract.Ensures(Contract.Result<IVMCollection<TItemVM>>() != null);

            return null;
         }

         [Obsolete]
         public void Initialize(BehaviorInitializationContext context) {
         }
      }
   }
}
