namespace Inspiring.MvvmTest.ViewModels.Core.Common {
   using System.Linq;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ChangeArgsTests {
      [TestMethod]
      public void ValidationStateChange_SetsChangeTypeAndChangedPath() {
         var args = ChangeArgs.ValidationStateChanged();
         Assert.AreEqual(ChangeType.ValidationStateChanged, args.ChangeType);
         DomainAssert.AreEqual(Path.Empty, args.ChangedPath);

         var property = PropertyStub.Build();
         args = ChangeArgs.ValidationStateChanged(property);
         Assert.AreEqual(ChangeType.ValidationStateChanged, args.ChangeType);
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
      public void ItemsRemoved_SetsChangedPathToEmpty() {
         var collection = VMCollectionStub.Build();
         var oldItems = new[] { ViewModelStub.Build() };
         var args = ChangeArgs.ItemsRemoved(collection, oldItems);

         Assert.AreEqual(ChangeType.RemovedFromCollection, args.ChangeType);
         CollectionAssert.AreEqual(oldItems, args.OldItems.ToArray());
         Assert.IsFalse(args.NewItems.Any());
         DomainAssert.AreEqual(Path.Empty.Append(collection), args.ChangedPath);
      }
   }
}