namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using Inspiring.MvvmTest.ApiTests.ViewModels.Domain;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class CollectionTests {
      private TaskListVM VM { get; set; }

      [TestInitialize]
      public void Setup() {
         VM = new TaskListVM();
         VM.InitializeFrom(TaskRepository.GetTasks());
      }

      [TestMethod]
      public void GetValue_AlwaysReturnsSameInstance() {
         var first = VM.Tasks;
         var second = VM.Tasks;

         Assert.AreSame(first, second);
      }
   }
}