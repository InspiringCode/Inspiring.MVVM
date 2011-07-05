namespace Inspiring.MvvmTest.ViewModels.Core.Common {
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ChangeArgsTests {
      [TestMethod]
      public void ValidationStateChange_SetsChangeTypeAndChangedPath() {
         var args = ChangeArgs.ValidationResultChanged();
         Assert.AreEqual(ChangeType.ValidationResultChanged, args.ChangeType);
         DomainAssert.AreEqual(Path.Empty, args.ChangedPath);

         var property = PropertyStub.Build();
         args = ChangeArgs.ValidationResultChanged(property);
         Assert.AreEqual(ChangeType.ValidationResultChanged, args.ChangeType);
         DomainAssert.AreEqual(Path.Empty.Append(property), args.ChangedPath);
      }

      [TestMethod]
      public void PropertyChanged_SetsChangeTypeAndChangedPathToProperty() {
         var property = PropertyStub.Build();
         var args = ChangeArgs.PropertyChanged(property);

         Assert.AreEqual(ChangeType.PropertyChanged, args.ChangeType);
         DomainAssert.AreEqual(Path.Empty.Append(property), args.ChangedPath);
      }

      [TestMethod]
      public void ItemsAdded_SetsChangeTypeAndNewItemsAndChangedPathToCollection() {
         var collection = VMCollectionStub.Build();
         var newItems = new[] { ViewModelStub.Build() };
         var args = ChangeArgs.ItemsAdded(collection, newItems);

         Assert.AreEqual(ChangeType.AddedToCollection, args.ChangeType);
         CollectionAssert.AreEqual(newItems, args.NewItems.ToArray());
         Assert.IsFalse(args.OldItems.Any());
         DomainAssert.AreEqual(Path.Empty.Append(collection), args.ChangedPath);
      }


      [TestMethod]
      public void CollectionPopulated_SetsChangeTypeAndNewItemsAndChangedPathToCollection() {
         var collection = VMCollectionStub
            .WithItems(ViewModelStub.Build())
            .Build();

         var newItems = collection.ToArray();
         var args = ChangeArgs.CollectionPopulated(collection);

         Assert.AreEqual(ChangeType.CollectionPopulated, args.ChangeType);
         CollectionAssert.AreEqual(newItems, args.NewItems.ToArray());
         Assert.IsFalse(args.OldItems.Any());
         DomainAssert.AreEqual(Path.Empty.Append(collection), args.ChangedPath);
      }

      [TestMethod]
      public void ItemsRemoved_SetsChangedPathToEmpty() {
         var collection = VMCollectionStub.Build();
         var oldItems = new[] { ViewModelStub.Build() };
         var args = ChangeArgs.ItemsRemoved(collection, oldItems);

         Assert.AreEqual(ChangeType.RemovedFromCollection, args.ChangeType);
         CollectionAssert.AreEqual(oldItems, args.OldItems.ToArray());
         Assert.IsFalse(args.NewItems.Any());
         DomainAssert.AreEqual(Path.Empty.Append(collection), args.ChangedPath);
      }

      [TestMethod]
      public void ViewModelPropertyChanged_SetsChangeTypeAndChangedPathToProperty() {
         var property = PropertyStub.Build();
         var args = ChangeArgs.ViewModelPropertyChanged(property, null, null);

         Assert.AreEqual(ChangeType.PropertyChanged, args.ChangeType);
         DomainAssert.AreEqual(Path.Empty.Append(property), args.ChangedPath);
      }

      [TestMethod]
      public void ViewModelPropertyChanged_SetsOldAndNewItems() {
         var property = PropertyStub.Build();
         var newValue = ViewModelStub.Build();
         var args = ChangeArgs.ViewModelPropertyChanged(property, null, newValue);

         CollectionAssert.AreEqual(new IViewModel[0], args.OldItems.ToArray());
         CollectionAssert.AreEqual(new IViewModel[] { newValue }, args.NewItems.ToArray());
      }
   }
}