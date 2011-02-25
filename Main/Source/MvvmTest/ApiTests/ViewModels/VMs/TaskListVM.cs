namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using System.Collections.Generic;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.MvvmTest.ApiTests.ViewModels.Domain;

   public sealed class TaskListVM : DefaultViewModelWithSourceBase<TaskListVMDescriptor, IEnumerable<Task>> {
      public static readonly TaskListVMDescriptor ClassDescriptor = VMDescriptorBuilder
         .OfType<TaskListVMDescriptor>()
         .For<TaskListVM>()
         .WithProperties((d, c) => {
            var vm = c.GetPropertyBuilder();

            d.Tasks = vm.Collection.Wraps(x => x.Source).With<TaskVM>(TaskVM.ClassDescriptor);
         })
         .Build();

      public TaskListVM()
         : base(ClassDescriptor) {
      }

      public IVMCollection<TaskVM> Tasks {
         get { return GetValue(Descriptor.Tasks); }
      }
   }

   public sealed class TaskListVMDescriptor : VMDescriptor {
      public IVMPropertyDescriptor<IVMCollection<TaskVM>> Tasks { get; set; }
   }
}
