//namespace Inspiring.Mvvm.ViewModels.Core {
//   using System;
//   using System.Diagnostics.Contracts;

//   // TODO: Allow a user to write his own FactoryBehavior and replace it by configuration.
//   //       This is currently not possible because ctor call is hardcoded.

//   /// <inheritdoc />
//   public sealed class CollectionFactoryBehavior<TItemVM> :
//      Behavior,
//      ICollectionBehaviorConfigurationBehavior,
//      IValueAccessorBehavior<IVMCollection<TItemVM>>,
//      IValueFactoryBehavior<IVMCollection<TItemVM>>
//      where TItemVM : IViewModel {

//      private BehaviorChain _collectionBehaviors;

//      /// <param name="collectionBehaviorConfiguration">
//      ///   Note that the passed <paramref name="collectionBehaviorConfiguration"/>
//      ///   can be modified until <see cref="CreateCollection"/> is called the first
//      ///   time.
//      /// </param>
//      public CollectionFactoryBehavior(BehaviorChainConfiguration collectionBehaviorConfiguration) {
//         Contract.Requires<ArgumentNullException>(collectionBehaviorConfiguration != null);
//         CollectionBehaviorConfiguration = collectionBehaviorConfiguration;
//      }

//      /// <inheritdoc />
//      public BehaviorChainConfiguration CollectionBehaviorConfiguration {
//         get;
//         private set;
//      }

//      public IVMCollection<TItemVM> GetValue(IBehaviorContext context) {
//         if (_collectionBehaviors == null) {
//            _collectionBehaviors = CollectionBehaviorConfiguration.CreateChain();
//            CollectionBehaviorConfiguration.Seal();
//            Seal();
//         }

//         return new VMCollection<TItemVM>(_collectionBehaviors, owner: context.VM);
//      }

//      public void SetValue(IBehaviorContext context, IVMCollection<TItemVM> value) {
//         throw new NotSupportedException();
//      }

//      public IVMCollection<TItemVM> CreateValue(IBehaviorContext context) {
//         if (_collectionBehaviors == null) {
//            _collectionBehaviors = CollectionBehaviorConfiguration.CreateChain();
//            CollectionBehaviorConfiguration.Seal();
//            Seal();
//         }

//         return new VMCollection<TItemVM>(_collectionBehaviors, owner: context.VM);
//      }
//   }
//}
