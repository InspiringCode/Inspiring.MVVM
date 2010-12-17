﻿namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using System.Collections.Generic;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.ApiTests.ViewModels.Domain;

   public sealed class TaskListVM : ViewModel<TaskListVMDescriptor>, ICanInitializeFrom<IEnumerable<Task>> {
      public static readonly TaskListVMDescriptor Descriptor = VMDescriptorBuilder
         .For<TaskListVM>()
         .CreateDescriptor(c => {
            var vm = c.GetPropertyBuilder();

            return new TaskListVMDescriptor {
               Tasks = vm.Collection().Wraps(x => x.TasksSource).Of<TaskVM>(TaskVM.Descriptor)
            };
         })
         .Build();

      public TaskListVM()
         : base(Descriptor) {
      }

      public IEnumerable<Task> TasksSource { get; private set; }

      public IVMCollection<TaskVM> Tasks {
         get { return GetValue(Descriptor.Tasks); }
      }

      public void InitializeFrom(IEnumerable<Task> source) {
         TasksSource = source;
      }
   }

   public sealed class TaskListVMDescriptor : VMDescriptor {
      public VMProperty<IVMCollection<TaskVM>> Tasks { get; set; }
   }
}