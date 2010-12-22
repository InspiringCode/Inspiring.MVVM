namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using System.Collections.Generic;
   using Inspiring.MvvmTest.ApiTests.ViewModels.Domain;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class CollectionTests {
      private List<Task> SourceList { get; set; }
      private TaskListVM VM { get; set; }

      [TestInitialize]
      public void Setup() {
         SourceList = TaskRepository.GetTasks();
         VM = new TaskListVM();
         VM.InitializeFrom(SourceList);
      }

      [TestMethod]
      public void GetValue_AlwaysReturnsSameInstance() {
         var first = VM.Tasks;
         var second = VM.Tasks;

         Assert.AreSame(first, second);
      }

      [TestMethod]
      public void AddItem_ModifiesSourceCollection() {
         var newTask = new Task("New Task");
         var newTaskVM = new TaskVM();
         newTaskVM.InitializeFrom(newTask);

         VM.Tasks.Add(newTaskVM);

         Assert.IsTrue(SourceList.Contains(newTask));
      }

   }
}