namespace Inspiring.MvvmTest.ApiTests.ViewModels.Undo {
   using System;
   using Inspiring.Mvvm.ViewModels;

   public sealed class EmployeeVM : DefaultViewModelWithSourceBase<EmployeeVMDescriptor, Employee> {
      public EmployeeVM()
         : base(CreateDescriptor(ProjectVM.ClassDescriptorWithoutUndoRoot)) {
      }

      public EmployeeVM(ProjectVMDescriptor projectVMDescriptor)
         : base(CreateDescriptor(projectVMDescriptor)) {
      }

      internal string Name {
         get { return GetValue(Descriptor.Name); }
         set { SetValue(Descriptor.Name, value); }
      }

      internal ProjectVM SelectedProject {
         get { return GetValue(Descriptor.SelectedProject); }
         set { SetValue(Descriptor.SelectedProject, value); }
      }

      internal IVMCollection<ProjectVM> Projects {
         get { return GetValue(Descriptor.Projects); }
         set { SetValue(Descriptor.Projects, value); }
      }

      internal new EmployeeVMDescriptor Descriptor {
         get { return base.Descriptor; }
      }

      internal void Revalidate() {
         Kernel.Revalidate(ValidationScope.FullSubtree, ValidationMode.CommitValidValues);
      }

      private static EmployeeVMDescriptor CreateDescriptor(ProjectVMDescriptor projectVMDescriptor) {
         return VMDescriptorBuilder
            .OfType<EmployeeVMDescriptor>()
            .For<EmployeeVM>()
            .WithProperties((d, c) => {
               var b = c.GetPropertyBuilder(x => x.Source);

               d.Name = b.Property.MapsTo(x => x.Name);
               d.Projects = b.Collection
                  .Wraps(x => x.Projects)
                  .With<ProjectVM>(projectVMDescriptor);
               d.SelectedProject = b.Property.Of<ProjectVM>();
            })
            .WithValidators(b => {
               b.Check(x => x.Name)
                  .HasValue(String.Empty);
               //b.CheckCollection(x => x.Projects, x => x.Title)
               //   .IsUnique(string.Empty);
            })
            .WithViewModelBehaviors(b => {
               b.IsUndoRoot();
               b.EnableUndo();
            })
            .Build();
      }
   }

   public sealed class EmployeeVMDescriptor : VMDescriptor {
      public IVMPropertyDescriptor<string> Name { get; set; }
      public IVMPropertyDescriptor<ProjectVM> SelectedProject { get; set; }
      public IVMPropertyDescriptor<IVMCollection<ProjectVM>> Projects { get; set; }
   }
}