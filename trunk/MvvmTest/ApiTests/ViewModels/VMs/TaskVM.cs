namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.MvvmTest.ApiTests.ViewModels.Domain;
using Inspiring.Mvvm.ViewModels.Core;

   public class TaskVM : ViewModel<TaskVMDescriptor>, IVMCollectionItem<Task> {
      public static readonly TaskVMDescriptor Descriptor = VMDescriptorBuilder
         .OfType<TaskVMDescriptor>()
         .For<TaskVM>()
         .WithProperties((d, c) => {
            var vm = c.GetPropertyBuilder();
            var t = c.GetPropertyBuilder(x => x.SourceTask);

            d.Title = t.Property.MapsTo(x => x.Title);
            d.Description = t.Property.DelegatesTo(
                  task => task.Description != null ? task.Description.Html : null,
                  (task, val) => task.Description = new RichText(val)
               );
            d.ScreenTitle = vm.Property.Of<string>();
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

      Task IVMCollectionItem<Task>.Source {
         get { return SourceTask; }
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
