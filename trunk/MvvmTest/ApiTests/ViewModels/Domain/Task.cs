namespace Inspiring.MvvmTest.ApiTests.ViewModels.Domain {

   public sealed class Task {
      public Task() {
      }

      public Task(string title, string descriptionHtml = null) {
         Title = title;
         Description = new RichText(descriptionHtml);
      }

      public string Title { get; set; }
      public RichText Description { get; set; }
   }
}
