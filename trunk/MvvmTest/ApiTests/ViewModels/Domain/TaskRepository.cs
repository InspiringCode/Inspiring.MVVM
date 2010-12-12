namespace Inspiring.MvvmTest.ApiTests.ViewModels.Domain {
   public static class TaskRepository {
      public static Task[] GetTasks() {
         return new Task[] {
            new Task("Task 1"),
            new Task("Task 2"),
            new Task("Task 3")
         };
      }
   }
}
