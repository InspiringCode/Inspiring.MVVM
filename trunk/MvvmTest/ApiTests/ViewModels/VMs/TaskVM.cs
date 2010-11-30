namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.MvvmTest.ApiTests.ViewModels.Domain;

   public class TaskVM : ViewModel<TaskVMDescriptor>, ICanInitializeFrom<Task> {
      public static readonly TaskVMDescriptor Descriptor = VMDescriptorBuilder
         .For<TaskVM>()
         .CreateDescriptor(c => {
            var vm = c.GetPropertyFactory();
            var t = c.GetPropertyFactory(x => x.Task);

            return new TaskVMDescriptor {
               Title = t.Mapped(x => x.Title),
               Description = t.Calculated(
                  task => task.Description.Html,
                  (task, val) => task.Description = new RichText(val)
               ),
               ScreenTitle = vm.Local<string>()
            };
         })
         .Build();

      public TaskVM(IServiceLocator serviceLocator = null)
         : base(Descriptor, serviceLocator) {
      }

      public TaskVM(TaskVMDescriptor descriptor)
         : base(descriptor) {
      }

      public Task Task { get; private set; }

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

      public void InitializeFrom(Task source) {
         Task = source;
         ScreenTitle = "Edit task: " + source.Title;
      }
   }

   public sealed class TaskVMDescriptor : VMDescriptor {
      public VMProperty<string> Title { get; set; }
      public VMProperty<string> Description { get; set; }
      public VMProperty<string> ScreenTitle { get; set; }
   }
}
