namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using System.Collections.Generic;
   using Inspiring.MvvmTest.ApiTests.ViewModels.Domain;
   using Inspiring.MvvmTest.ApiTests.ViewModels.VMs;
   using Microsoft.VisualStudio.TestTools.UnitTesting;


   [TestClass]
   public class TestCollectionWithInheritance {
      private List<Customer> SourceList { get; set; }
      private CollectionOfCustomerWithBaseVM VM { get; set; }

      [TestMethod]
      public void TestValidationWithInheritance() {
         SourceList = CustomerRepository.GetCustomers();
         VM = new CollectionOfCustomerWithBaseVM();
         VM.InitializeFrom(SourceList);
         Assert.IsNotNull(VM.GetValue(CollectionOfCustomerWithBaseVM.ClassDescriptor.Customers));
         Assert.IsTrue(VM.IsValid);
      }


   }
}
