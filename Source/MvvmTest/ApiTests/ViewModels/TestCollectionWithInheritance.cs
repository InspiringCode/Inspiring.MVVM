namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.MvvmTest.ApiTests.ViewModels.Domain;
   using Inspiring.MvvmTest.ApiTests.ViewModels.VMs;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;


   [TestClass]
   public class TestCollectionWithInheritance : TestBase {
      private List<Customer> SourceList { get; set; }
      private CollectionOfCustomerWithBaseVM VM { get; set; }

      [TestMethod]
      public void TestValidationWithInheritance() {
         Assert.Inconclusive("TODO: Think about inheritance and IViewModel implementation.");
         SourceList = CustomerRepository.GetCustomers();
         VM = new CollectionOfCustomerWithBaseVM();
         VM.InitializeFrom(SourceList);
         Assert.IsNotNull(VM.GetValue(CollectionOfCustomerWithBaseVM.ClassDescriptor.Customers));
         Assert.IsTrue(VM.IsValid);
      }

      [TestMethod]
      public void TestAccessInheritedProperty() {
         Assert.Inconclusive("TODO: Think about inheritance and IViewModel implementation.");
         SourceList = CustomerRepository.GetCustomers();
         VM = new CollectionOfCustomerWithBaseVM();
         VM.InitializeFrom(SourceList);
         VM.Children.Add(VM.GetValue(CollectionOfCustomerWithBaseVM.ClassDescriptor.Customers)[0]);
         Assert.IsNotNull(VM.GetValue(CollectionOfCustomerWithBaseVM.ClassDescriptor.Children));
         Assert.IsFalse(VM.GetValue(CollectionOfCustomerWithBaseVM.ClassDescriptor.Children).Single().IsExpanded);
      }


   }
}
