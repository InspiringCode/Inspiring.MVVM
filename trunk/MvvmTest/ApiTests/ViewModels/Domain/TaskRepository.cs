namespace Inspiring.MvvmTest.ApiTests.ViewModels.Domain {
   using System.Collections.Generic;

   public static class TaskRepository {
      public static List<Task> GetTasks() {
         return new List<Task> {
            new Task("Task 1"),
            new Task("Task 2"),
            new Task("Task 3")
         };
      }
   }
}
