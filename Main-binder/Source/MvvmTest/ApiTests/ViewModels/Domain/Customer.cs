namespace Inspiring.MvvmTest.ApiTests.ViewModels.Domain {

   public sealed class Customer {
      public Customer(string title = null) {
         Title = title;
      }

      public string Title { get; set; }
   }
}
