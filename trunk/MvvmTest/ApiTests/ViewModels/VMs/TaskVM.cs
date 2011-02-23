namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.MvvmTest.ApiTests.ViewModels.Domain;

   public class TaskVM : DefaultViewModelWithSourceBase<TaskVMDescriptor, Task> {
      public static readonly TaskVMDescriptor ClassDescriptor = VMDescriptorBuilder
         .OfType<TaskVMDescriptor>()
         .For<TaskVM>()
         .WithProperties((d, c) => {
            var vm = c.GetPropertyBuilder();
            var t = c.GetPropertyBuilder(x => x.Source);

            d.Title = t.Property.MapsTo(x => x.Title);
            d.Description = t.Property.DelegatesTo(
                  task => task.Description != null ? task.Description.Html : null,
                  (task, val) => task.Description = new RichText(val)
               );
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

      public string Description {
         get { return GetValue(Descriptor.Description); }
         set { SetValue(Descriptor.Description, value); }
      }

      public override void InitializeFrom(Task source) {
         base.InitializeFrom(source);
         ScreenTitle = "Edit task: " + source.Title;
      }
   }

   public sealed class TaskVMDescriptor : VMDescriptor {
      public IVMPropertyDescriptor<string> Title { get; set; }
      public IVMPropertyDescriptor<string> Description { get; set; }
      public IVMPropertyDescriptor<string> ScreenTitle { get; set; }
   }
}
