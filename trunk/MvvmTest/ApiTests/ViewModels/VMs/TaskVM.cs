namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.MvvmTest.ApiTests.ViewModels.Domain;

   public class TaskVM : ViewModel<TaskVMDescriptor>, ICanInitializeFrom<Task> {
      public static readonly TaskVMDescriptor Descriptor = VMDescriptorBuilder
         .For<TaskVM>()
         .CreateDescriptor(c => {
            var vm = c.GetPropertyBuilder();
            var t = c.GetPropertyBuilder(x => x.SourceTask);

            return new TaskVMDescriptor {
               Title = t.Mapped(x => x.Title).Property(),
               Description = t.Calculated(
                  task => task.Description != null ? task.Description.Html : null,
                  (task, val) => task.Description = new RichText(val)
               ).Property(),
               ScreenTitle = vm.Local.Property<string>()
            };
         })
         .Build();

      public TaskVM()
         : base(Descriptor) {
      }

      public TaskVM(TaskVMDescriptor descriptor)
         : base(descriptor) {
      }

      public Task SourceTask { get; private set; }

      public string ScreenTitle {
         get { return GetValue(DescriptorBase.ScreenTitle); }
         set { SetValue(DescriptorBase.ScreenTitle, value); }
      }

      public string Title {
         get { return GetValue(DescriptorBase.Title); }
         set { SetValue(DescriptorBase.Title, value); }
      }

      public string Description {
         get { return GetValue(DescriptorBase.Description); }
         set { SetValue(DescriptorBase.Description, value); }
      }

      public void InitializeFrom(Task source) {
         SourceTask = source;
         ScreenTitle = "Edit task: " + source.Title;
      }
   }

   public sealed class TaskVMDescriptor : VMDescriptor {
      public VMProperty<string> Title { get; set; }
      public VMProperty<string> Description { get; set; }
      public VMProperty<string> ScreenTitle { get; set; }
   }
}
