﻿namespace Inspiring.MvvmTest.ViewModels.Core.Common.Paths {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.Stubs;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class PathHelperTests : TestBase {
      [TestMethod]
      public void SelectsOnlyPropertyOf_PathWithSingleViewModelAndProperty_ReturnsSuccess() {
         var vm = CreateVM();
         var property = CreateProperty();
         var path = Path.Empty.Append(vm).Append(property);
         var r = path.SelectsOnlyPropertyOf(vm);

         Assert.IsTrue(r.Success);
         Assert.AreEqual(vm, r.VM);
         Assert.AreEqual(property, r.Property);
      }


      [TestMethod]
      public void SelectsOnlyPropertyOf_PathWithTwoViewModelsAndProperty_ReturnsFailure() {
         var firstVM = CreateVM();
         var secondVM = CreateVM();
         var path = Path.Empty.Append(firstVM).Append(secondVM).Append(CreateProperty());

         var r = path.SelectsOnlyPropertyOf(firstVM);
         Assert.IsFalse(r.Success);

         r = path.SelectsOnlyPropertyOf(secondVM);
         Assert.IsFalse(r.Success);
      }

      [TestMethod]
      public void SelectsOnlyPropertyOf_PathWithProperty_ReturnsFailure() {
         var path = Path.Empty.Append(CreateProperty());
         var r = path.SelectsOnlyPropertyOf(CreateVM());

         Assert.IsFalse(r.Success);
      }

      [TestMethod]
      public void SelectsOnlyPropertyOf_PathWithTwoViewModels_ReturnsFailure() {
         var firstVM = CreateVM();
         var secondVM = CreateVM();
         var path = Path.Empty.Append(firstVM).Append(secondVM);

         var r = path.SelectsOnlyPropertyOf(firstVM);
         Assert.IsFalse(r.Success);

         r = path.SelectsOnlyPropertyOf(secondVM);
         Assert.IsFalse(r.Success);
      }

      [TestMethod]
      public void SelectsOnly_ForPathWithSingleMatchingViewModel_Succeeds() {
         var vm = CreateVM();
         var path = Path.Empty.Append(vm);

         var r = path.SelectsOnly(vm);

         Assert.IsTrue(r.Success);
         Assert.AreEqual(vm, r.VM);
      }

      [TestMethod]
      public void SelectsOnly_ForPathWithSingleProperty_Fails() {
         var property = CreateProperty();
         var vm = CreateVM();
         var path = Path.Empty.Append(property);

         var r = path.SelectsOnly(vm);
         Assert.IsFalse(r.Success);
      }

      [TestMethod]
      public void SelectsOnly_ForPathWithSingleNonMatchingViewModel_Fails() {
         var vm = CreateVM();
         var otherVM = CreateVM();
         var path = Path.Empty.Append(vm);

         var r = path.SelectsOnly(otherVM);
         Assert.IsFalse(r.Success);
      }

      [TestMethod]
      public void SelectsOnly_ForPathWithTwoViewModels_Fails() {
         var firstVM = CreateVM();
         var secondVM = CreateVM();
         var path = Path.Empty.Append(firstVM).Append(secondVM);

         var r = path.SelectsOnly(firstVM);
         Assert.IsFalse(r.Success);

         r = path.SelectsOnly(secondVM);
         Assert.IsFalse(r.Success);
      }

      [TestMethod]
      public void SelectsOnlyCollectionOf_PathWithSingleViewModelAndCollection_ReturnsSuccess() {
         var vm = CreateVM();
         var collection = CreateCollection();
         var path = Path.Empty.Append(vm).Append(collection);
         var r = path.SelectsOnlyCollectionOf(vm);

         Assert.IsTrue(r.Success);
         Assert.AreEqual(vm, r.VM);
         Assert.AreEqual(collection, r.Collection);
      }


      [TestMethod]
      public void SelectsOnlyCollectionOf_PathWithTwoViewModelsAndCollection_ReturnsFailure() {
         var firstVM = CreateVM();
         var secondVM = CreateVM();
         var path = Path.Empty.Append(firstVM).Append(secondVM).Append(CreateCollection());

         var r = path.SelectsOnlyCollectionOf(firstVM);
         Assert.IsFalse(r.Success);

         r = path.SelectsOnlyCollectionOf(secondVM);
         Assert.IsFalse(r.Success);
      }

      [TestMethod]
      public void SelectsOnlyCollectionOf_PathWithCollection_ReturnsFailure() {
         var path = Path.Empty.Append(CreateCollection());
         var r = path.SelectsOnlyCollectionOf(CreateVM());

         Assert.IsFalse(r.Success);
      }

      [TestMethod]
      public void SelectsAncestor_ForTwoViewModel_Succeeds() {
         var path = Path.Empty
            .Append(CreateVM())
            .Append(CreateVM());

         Assert.IsTrue(path.SelectsAncestor());
      }

      [TestMethod]
      public void SelectAncestor_ForTwoViewModelsAndProperty_Succeeds() {
         var path = Path.Empty
            .Append(CreateVM())
            .Append(CreateVM())
            .Append(CreateProperty());

         Assert.IsTrue(path.SelectsAncestor());
      }

      [TestMethod]
      public void SelectsAncestor_ForPathWithOneViewModel_Fails() {
         var path = Path.Empty.Append(CreateVM());
         Assert.IsFalse(path.SelectsAncestor());
      }

      [TestMethod]
      public void SelectsAncestor_ForPathWithViewModelAndProperty_Fails() {
         var path = Path.Empty
            .Append(CreateVM())
            .Append(CreateProperty());

         Assert.IsFalse(path.SelectsAncestor());
      }


      private ViewModelStub CreateVM() {
         return new ViewModelStub();
      }

      private PropertyStub<string> CreateProperty() {
         return PropertyStub.Of<string>();
      }

      private IVMCollection CreateCollection() {
         return VMCollectionStub.Build();
      }
   }
}