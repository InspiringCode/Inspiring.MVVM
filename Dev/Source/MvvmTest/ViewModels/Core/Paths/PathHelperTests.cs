namespace Inspiring.MvvmTest.ViewModels.Core.Paths {
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.Stubs;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   // TODO: Is this needed?
   [TestClass]
   public class PathHelperTests : TestBase {
      [TestMethod]
      public void SelectsPropertyOf_PathWithSingleViewModelAndProperty_ReturnsSuccess() {
         var vm = CreateVM();
         var property = CreateProperty();
         var path = Path.Empty.Append(vm).Append(property);
         var r = path.SelectsPropertyOf(vm);

         Assert.IsTrue(r.Success);
         Assert.AreEqual(vm, r.VM);
         Assert.AreEqual(property, r.Property);
      }


      [TestMethod]
      public void SelectsPropertyOf_PathWithTwoViewModelsAndProperty_ReturnsFailure() {
         var firstVM = CreateVM();
         var secondVM = CreateVM();
         var path = Path.Empty.Append(firstVM).Append(secondVM).Append(CreateProperty());

         var r = path.SelectsPropertyOf(firstVM);
         Assert.IsFalse(r.Success);

         r = path.SelectsPropertyOf(secondVM);
         Assert.IsFalse(r.Success);
      }

      [TestMethod]
      public void SelectsPropertyOf_PathWithProperty_ReturnsFailure() {
         var path = Path.Empty.Append(CreateProperty());
         var r = path.SelectsPropertyOf(CreateVM());

         Assert.IsFalse(r.Success);
      }

      [TestMethod]
      public void SelectsPropertyOf_PathWithTwoViewModels_ReturnsFailure() {
         var firstVM = CreateVM();
         var secondVM = CreateVM();
         var path = Path.Empty.Append(firstVM).Append(secondVM);

         var r = path.SelectsPropertyOf(firstVM);
         Assert.IsFalse(r.Success);

         r = path.SelectsPropertyOf(secondVM);
         Assert.IsFalse(r.Success);
      }

      private ViewModelStub CreateVM() {
         return new ViewModelStub();
      }

      private PropertyStub<string> CreateProperty() {
         return PropertyStub.Of<string>();
      }
   }
}