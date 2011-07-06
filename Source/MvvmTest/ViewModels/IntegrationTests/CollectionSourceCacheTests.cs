namespace Inspiring.MvvmTest.ViewModels.IntegrationTests {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class CollectionSourceCacheTests {
      [TestMethod]
      public void CollectionSourceCacheDisabled_WhenCollectionChanges_DoesInvokeSourceProvider() {
         int sourceProviderInvocations = 0;
         var sourceList = new List<Task>();

         Func<IEnumerable<Task>> sourceProvider = () => {
            sourceProviderInvocations++;
            return sourceList;
         };

         var vm = new TaskListVM(TaskListVM.CreateDescriptor(sourceProvider, cacheSourceCollection: false));

         var loadTrigger = vm.Tasks;

         vm.Tasks.Add(new TaskVM());
         vm.Tasks.Clear();

         Assert.IsTrue(sourceProviderInvocations >= 3);
      }

      [TestMethod]
      public void CollectionSourceCacheEnabled_WhenCollectionChanges_DoesNotInvokeSourceProvider() {
         int sourceProviderInvocations = 0;
         Func<IEnumerable<Task>> sourceProvider = () => {
            sourceProviderInvocations++;
            return new List<Task>();
         };

         var vm = new TaskListVM(TaskListVM.CreateDescriptor(sourceProvider, cacheSourceCollection: true));

         var loadTrigger = vm.Tasks;

         vm.Tasks.Add(new TaskVM());
         vm.Tasks.Clear();

         Assert.AreEqual(1, sourceProviderInvocations);
      }

      [TestMethod]
      public void CollectionSourceCacheEnabled_WhenCollectionIsRefreshed_InvokesSourceProvider() {

         List<Task> originalTasks = TaskRepository.GetTasks();
         IEnumerable<Task> tasks = originalTasks;

         Func<IEnumerable<Task>> sourceProvider = () => {
            return tasks;
         };

         var vm = new TaskListVM(TaskListVM.CreateDescriptor(sourceProvider, cacheSourceCollection: true));

         CollectionAssert.AreEqual(
            originalTasks,
            vm.Tasks.Select(x => x.Source).ToList()
         );

         List<Task> refreshedTasks = TaskRepository.GetTasks();
         tasks = refreshedTasks;

         vm.RefreshTasks();

         CollectionAssert.AreEqual(
           refreshedTasks,
           vm.Tasks.Select(x => x.Source).ToList()
        );
      }
   }

   class TaskListVM : DefaultViewModelWithSourceBase<TaskListVMDescriptor, IEnumerable<Task>> {

      public TaskListVM(TaskListVMDescriptor descriptor)
         : base(descriptor) {

      }

      public static TaskListVMDescriptor CreateDescriptor(Func<IEnumerable<Task>> sourceProvider, bool cacheSourceCollection) {
         return VMDescriptorBuilder
            .OfType<TaskListVMDescriptor>()
            .For<TaskListVM>()
            .WithProperties((d, c) => {
               var vm = c.GetPropertyBuilder();

               d.Tasks = vm
                  .Collection
                  .Wraps(x => sourceProvider(), cacheSourceCollection)
                  .With<TaskVM>(TaskVM.ClassDescriptor);
            })
            .Build();
      }


      public IVMCollection<TaskVM> Tasks {
         get { return GetValue(Descriptor.Tasks); }
      }

      public void RefreshTasks() {
         Refresh(Descriptor.Tasks);
      }
   }

   class TaskListVMDescriptor : VMDescriptor {
      public IVMPropertyDescriptor<IVMCollection<TaskVM>> Tasks { get; set; }
   }

   class TaskVM : DefaultViewModelWithSourceBase<TaskVMDescriptor, Task> {
      public static readonly TaskVMDescriptor ClassDescriptor = VMDescriptorBuilder
         .OfType<TaskVMDescriptor>()
         .For<TaskVM>()
         .WithProperties((d, c) => {
            var vm = c.GetPropertyBuilder();
            var t = c.GetPropertyBuilder(x => x.Source);

            d.Title = t.Property.MapsTo(x => x.Title);
            d.ScreenTitle = vm.Property.Of<string>();
         })
         .Build();

      public TaskVM()
         : base(ClassDescriptor) {
      }

      public TaskVM(TaskVMDescriptor descriptor)
         : base(descriptor) {
      }

      public string ScreenTitle {
         get { return GetValue(Descriptor.ScreenTitle); }
         set { SetValue(Descriptor.ScreenTitle, value); }
      }

      public string Title {
         get { return GetValue(Descriptor.Title); }
         set { SetValue(Descriptor.Title, value); }
      }

      public override void InitializeFrom(Task source) {
         base.InitializeFrom(source);
         ScreenTitle = "Edit task: " + source.Title;
      }
   }

   class TaskVMDescriptor : VMDescriptor {
      public IVMPropertyDescriptor<string> Title { get; set; }
      public IVMPropertyDescriptor<string> ScreenTitle { get; set; }
   }

   class Task {
      public Task() {
      }

      public Task(string title, string descriptionHtml = null) {
         Title = title;
      }

      public string Title { get; set; }
   }

   static class TaskRepository {
      public static List<Task> GetTasks() {
         return new List<Task> {
            new Task("Task 1"),
            new Task("Task 2"),
            new Task("Task 3")
         };
      }
   }
}