namespace Inspiring.MvvmTest.ViewModels.Core.Validation.Behaviors {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class CollectionValidationSourceBehaviorTests : CollectionChangeHandlerBehaviorTestBase<IViewModel> {
      [TestInitialize]
      public void Setup() {
         Behavior = new CollectionValidationSourceBehavior<IViewModel>();
      }

      [TestMethod]
      public void ItemInserted_RevalidatesItem() {
         var item = new ViewModelSpy("Inserted item");
         Collection = CreateCollection(item);

         HandleItemInserted(item);
         Assert.IsTrue(item.WasValidated);
      }

      [TestMethod]
      public void ItemRemoved_RevalidatesItem() {
         var item = new ViewModelSpy("Removed item");
         Collection = CreateCollection();

         HandleItemRemoved(item);
         Assert.IsTrue(item.WasValidated);
      }

      [TestMethod]
      public void ItemSet_RevalidatesOldAndNewItem() {
         var oldItem = new ViewModelSpy("Old item");
         var newItem = new ViewModelSpy("New item");
         Collection = CreateCollection(newItem);

         HandleItemSet(oldItem, newItem);

         Assert.IsTrue(oldItem.WasValidated);
         Assert.IsTrue(newItem.WasValidated);
      }

      [TestMethod]
      public void ItemsCleared_ReavlidatesOldItems() {
         var oldItem = new ViewModelSpy("Old item");
         Collection = CreateCollection();

         HandleCollectionCleared(new[] { oldItem });
         Assert.IsTrue(oldItem.WasValidated);
      }

      [TestMethod]
      public void CollectionPopulated_DoesNotValidateOldOrNewItems() {
         var oldItem = new ViewModelSpy("Old item");
         var newItem = new ViewModelSpy("New item");

         Collection = CreateCollection(oldItem);
         HandleCollectionPopulated(new[] { oldItem });

         Assert.IsFalse(oldItem.WasValidated);
         Assert.IsFalse(newItem.WasValidated);
      }

      private class ViewModelSpy : ViewModelStub {
         private RevalidationSpy _spy;

         public ViewModelSpy(string description = null)
            : this(new RevalidationSpy(), description) {
         }

         private ViewModelSpy(RevalidationSpy spy, string description)
            : base(DescriptorStub.WithBehaviors(spy).Build(), description) {
            _spy = spy;
         }

         public bool WasValidated {
            get { return _spy.WasCalled; }
            set { _spy.WasCalled = value; }
         }
      }

      private class RevalidationSpy : Behavior, IRevalidationBehavior {
         public bool WasCalled { get; set; }

         public void Revalidate(IBehaviorContext context, CollectionResultCache cache) {
            WasCalled = true;
         }
      }
   }
}