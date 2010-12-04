namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;

   /// <inheritdoc />
   public sealed class CollectionFactoryBehavior<TItemVM> :
      Behavior,
      ICollectionFactoryBehavior<TItemVM>
      where TItemVM : IViewModel {

      private BehaviorChain _collectionBehaviors;

      /// <param name="collectionBehaviorConfiguration">
      ///   Note that the passed <paramref name="collectionBehaviorConfiguration"/>
      ///   can be modified until <see cref="CreateCollection"/> is called the first
      ///   time.
      /// </param>
      public CollectionFactoryBehavior(BehaviorChainConfiguration collectionBehaviorConfiguration) {
         Contract.Requires<ArgumentNullException>(collectionBehaviorConfiguration != null);
         CollectionBehaviorConfiguration = collectionBehaviorConfiguration;
      }

      /// <inheritdoc />
      public BehaviorChainConfiguration CollectionBehaviorConfiguration {
         get;
         private set;
      }

      /// <inheritdoc />
      public IVMCollection<TItemVM> CreateCollection(IBehaviorContext context) {
         if (_collectionBehaviors == null) {
            _collectionBehaviors = CollectionBehaviorConfiguration.CreateChain();
            CollectionBehaviorConfiguration.Seal();
            Seal();
         }

         return new VMCollection<TItemVM>(_collectionBehaviors, owner: context.VM);
      }
   }
}
