namespace Inspiring.MvvmTest.ApiTests.ViewModels.Domain {
   using System.Collections.Generic;

   public static class CustomerRepository {
      public static List<Customer> GetCustomers() {
         return new List<Customer> {
            new Customer("Cust 1"),
            new Customer("Cust 2"),
            new Customer("Cust 3")
         };
      }
   }
}
