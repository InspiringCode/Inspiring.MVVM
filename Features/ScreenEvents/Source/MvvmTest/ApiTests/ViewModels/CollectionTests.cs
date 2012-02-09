namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.MvvmTest.ApiTests.ViewModels.Domain;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;

   [TestClass]
   public class CollectionTests : TestBase {
      private List<Task> SourceList { get; set; }
      private TaskListVM VM { get; set; }

      [TestInitialize]
      public void Setup() {
         SourceList = TaskRepository.GetTasks();
         VM = new TaskListVM();
         VM.Source = SourceList;
      }

      [TestMethod]
      public void GetValue_AlwaysReturnsSameInstance() {
         var first = VM.WrapperCollection;
         var second = VM.WrapperCollection;

         Assert.AreSame(first, second);
      }

      [TestMethod]
      public void AddItem_ModifiesSourceCollection() {
         var newTask = new Task("New Task");
         var newTaskVM = new TaskVM();
         newTaskVM.InitializeFrom(newTask);

         VM.WrapperCollection.Add(newTaskVM);

         Assert.IsTrue(SourceList.Contains(newTask));
      }

      [TestMethod]
      public void GetValueOfWrapperCollection_Initially_RaisesChangeWithInitialPopulationReason() {
         Assert.AreEqual(0, VM.OnChangeInvocations.Count);
         VM.Load(x => x.WrapperCollection);

         ChangeArgs args = VM.OnChangeInvocations.SingleOrDefault();
         Assert.IsNotNull(args);
         Assert.AreEqual(InitialPopulationChangeReason.Instance, args.Reason);
      }
      
      [TestMethod]
      public void GetValueOfPopulatedCollection_Initially_RaisesChangeWithInitialPopulationReason() {
         Assert.AreEqual(0, VM.OnChangeInvocations.Count);
         VM.Load(x => x.PopulatedCollection);

         ChangeArgs args = VM.OnChangeInvocations.SingleOrDefault();
         Assert.IsNotNull(args);
         Assert.AreEqual(InitialPopulationChangeReason.Instance, args.Reason);
      }

      private sealed class TaskListVM : TestViewModel<TaskListVMDescriptor> {
         public static readonly TaskListVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<TaskListVMDescriptor>()
            .For<TaskListVM>()
            .WithProperties((d, c) => {
               var vm = c.GetPropertyBuilder();

               d.WrapperCollection = vm
                  .Collection
                  .Wraps(x => x.Source)
                  .With<TaskVM>(TaskVM.ClassDescriptor);

               d.PopulatedCollection = vm
                  .Collection
                  .PopulatedWith(_ => new[] { new TaskVM() })
                  .With(TaskVM.ClassDescriptor);
            })
            .Build();

         public TaskListVM()
            : base(ClassDescriptor) {
            Source = new[] { new Task() };
         }

         public IEnumerable<Task> Source { get; set; }

         public IVMCollection<TaskVM> WrapperCollection {
            get { return GetValue(Descriptor.WrapperCollection); }
         }
      }

      private sealed class TaskListVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<IVMCollection<TaskVM>> WrapperCollection { get; set; }
         public IVMPropertyDescriptor<IVMCollection<TaskVM>> PopulatedCollection { get; set; }
      }

      private class TaskVM : DefaultViewModelWithSourceBase<TaskVMDescriptor, Task> {
         public static readonly TaskVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<TaskVMDescriptor>()
            .For<TaskVM>()
            .WithProperties((d, c) => {
               var vm = c.GetPropertyBuilder();
               var t = c.GetPropertyBuilder(x => x.Source);

               d.Title = t.Property.MapsTo(x => x.Title);
            })
            .Build();

         public TaskVM()
            : base(ClassDescriptor) {
            InitializeFrom(new Task());
         }

         public string Title {
            get { return GetValue(Descriptor.Title); }
            set { SetValue(Descriptor.Title, value); }
         }
      }

      private sealed class TaskVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> Title { get; set; }
      }
   }
}