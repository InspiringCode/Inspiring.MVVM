namespace Inspiring.MvvmTest.ViewModels.Core.Properties.CollectionProperty {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using System.Text;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class CollectionAccessorBehaviorFixture : TestBase {
      protected const string RefreshItemLog = "RefreshItem ";
      protected NextBehavior Next { get; set; }
      protected IBehaviorContext Context { get; set; }

      [TestInitialize]
      public void BaseSetup() {
         Next = new NextBehavior();
      }

      protected IBehaviorContext CreateContext(IBehavior collectionAccessorBehavior) {
         return PropertyStub
            .WithBehaviors(collectionAccessorBehavior, Next)
            .GetContext();
      }

      protected static IVMCollection<ItemVM> CreateCollection() {
         return VMCollectionStub.Build<ItemVM>();
      }

      protected static ItemVM CreateItem() {
         return new ItemVM();
      }

      protected class NextBehavior :
         Behavior,
         IValueFactoryBehavior<ItemVM>,
         IValueFactoryBehavior<IVMCollection<ItemVM>>,
         IValueAccessorBehavior<IEnumerable<ItemSource>>,
         IValueAccessorBehavior<IEnumerable<ItemVM>>,
         IValueInitializerBehavior,
         IRefreshBehavior {

         public NextBehavior() {
            CollectionToReturn = CreateCollection();
            SourceCollectionToReturn = Enumerable.Empty<ItemSource>();
         }

         public IVMCollection<ItemVM> CollectionToReturn { get; set; }
         public IEnumerable<ItemSource> SourceCollectionToReturn { get; set; }
         public IEnumerable<ItemVM> PopulatedItemsToReturn { get; set; }

         public int CreateItemInvocations { get; set; }
         public int CreateCollectionInvocations { get; set; }
         public int GetPopulatedItemsInvocations { get; set; }

         public int InitializeValueInvocations { get; set; }
         public int RefreshInvocations { get; set; }

         public StringBuilder ActionLog { get; set; }

         ItemVM IValueFactoryBehavior<ItemVM>.CreateValue(IBehaviorContext context) {
            CreateItemInvocations++;
            var item = new ItemVM();

            if (ActionLog != null) {
               item.ActionLog = ActionLog;
            }

            return item;
         }

         IVMCollection<ItemVM> IValueFactoryBehavior<IVMCollection<ItemVM>>.CreateValue(
            IBehaviorContext context
         ) {
            CreateCollectionInvocations++;
            return CollectionToReturn;
         }

         IEnumerable<ItemVM> IValueAccessorBehavior<IEnumerable<ItemVM>>.GetValue(IBehaviorContext context) {
            GetPopulatedItemsInvocations++;
            return PopulatedItemsToReturn;
         }

         void IValueAccessorBehavior<IEnumerable<ItemVM>>.SetValue(IBehaviorContext context, IEnumerable<ItemVM> value) {
            throw new NotSupportedException();
         }

         public IEnumerable<ItemSource> GetValue(IBehaviorContext context) {
            return SourceCollectionToReturn;
         }

         public void SetValue(IBehaviorContext context, IEnumerable<ItemSource> value) {
            throw new NotSupportedException();
         }

         public void InitializeValue(IBehaviorContext context) {
            InitializeValueInvocations++;
         }

         public void Refresh(IBehaviorContext context) {
            RefreshInvocations++;
         }
      }

      protected class ItemVM : ViewModelStub, IHasSourceObject<ItemSource> {
         public ItemVM() :
            base(DescriptorStub.WithBehaviors(new RefreshMock()).Build()) {
         }

         public ItemSource Source { get; set; }

         public int RefreshInvocations {
            get {
               return Descriptor
                  .Behaviors
                  .GetNextBehavior<RefreshMock>()
                  .RefreshInvocations;
            }
         }

         public StringBuilder ActionLog {
            set {
               Descriptor
                  .Behaviors
                  .GetNextBehavior<RefreshMock>()
                  .ActionLog = value;
            }
         }
      }

      protected class ItemSource {
         public ItemSource(string name = "A item") {
            Name = name;
         }

         public string Name { get; private set; }

         public override string ToString() {
            return Name;
         }
      }

      private class RefreshMock : Behavior, IRefreshControllerBehavior {
         public RefreshMock() {
            ActionLog = new StringBuilder();
         }

         public StringBuilder ActionLog { get; set; }

         public int RefreshInvocations { get; set; }

         public void Refresh(IBehaviorContext context) {
            ActionLog.Append(RefreshItemLog);
            RefreshInvocations++;
         }

         public void Refresh(IBehaviorContext context, IVMPropertyDescriptor property) {
         }
      }
   }
}