namespace Inspiring.MvvmTest.ViewModels.Core.Validation {
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class CollectionResultCacheTests {

      [TestMethod]
      public void MyTestMethod() {
         var vm = new ItemListVM();
         var item = new ItemVM();
         vm.Items.Insert(0, item);

         new CollectionResultCache().GetCollectionValidationResults(
            ValidationStep.Value,
            item,
            ItemListVM.ClassDescriptor.Items
         );
      }
   }
}
