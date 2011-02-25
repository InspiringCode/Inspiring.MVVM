namespace Inspiring.MvvmTest.ApiTests.ViewModels.Domain {

   public sealed class Project {
      public Project(string title, Customer customer = null) {
         Title = title;
         Customer = customer;
      }

      public string Title { get; set; }

      public Customer Customer { get; set; }
   }
}
